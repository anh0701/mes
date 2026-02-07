using System.Data;
using Dapper;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/executions")]
[Tags("04 - Execution")]
public class ExecutionController : ControllerBase
{
    private readonly IDbConnection _db;

    public ExecutionController(IDbConnection db)
    {
        _db = db;
    }

    [HttpPost("start")]
    public async Task<IActionResult> Start(StartExecutionRequest req)
    {
        using var tx = _db.BeginTransaction();
        var checkSql = """
        SELECT Status
        FROM ProductionOrderStep
        WHERE POId = @PoId AND RoutingStepId = @RoutingStepId
        """;

        var status = await _db.ExecuteScalarAsync<string>(
            checkSql, req, tx);

        if (status != "Planned")
            return BadRequest("Step is not in Planned status");

        var insertSql = """
        INSERT INTO ProductionExecution
        (POId, RoutingStepId, MachineId, InputQty)
        VALUES (@PoId, @RoutingStepId, @MachineId, @InputQty);
        """;

        await _db.ExecuteAsync(insertSql, req, tx);

        var updateStepSql = """
        UPDATE ProductionOrderStep
        SET Status = 'Running'
        WHERE POId = @PoId AND RoutingStepId = @RoutingStepId;
        """;
        await _db.ExecuteAsync(updateStepSql, req, tx);

        tx.Commit();
        // var sql = """
        // INSERT INTO ProductionExecution
        // (POId, RoutingStepId, MachineId, InputQty)
        // VALUES (@PoId, @RoutingStepId, @MachineId, @InputQty);

        // UPDATE ProductionOrderStep
        // SET Status = 'Running'
        // WHERE POId = @PoId AND RoutingStepId = @RoutingStepId;
        // """;

        // await _db.ExecuteAsync(sql, req);
        return Ok();
    }

    [HttpPost("{executionId}/finish")]
    public async Task<IActionResult> Finish(
        int executionId,
        int outputQty,
        int ngQty)
    {
        using var tx = _db.BeginTransaction();

        var finishSql = """
        UPDATE ProductionExecution
        SET EndTime = GETDATE(),
            OutputQty = @OutputQty,
            NGQty = @NgQty
        WHERE ExecutionId = @ExecutionId;
        """;

        await _db.ExecuteAsync(finishSql, new
        {
            ExecutionId = executionId,
            OutputQty = outputQty,
            NgQty = ngQty
        }, tx);

        var updateStepSql = """
        UPDATE ProductionOrderStep
        SET Status = 'Completed'
        WHERE RoutingStepId = (
            SELECT RoutingStepId
            FROM ProductionExecution
            WHERE ExecutionId = @ExecutionId
        )
        AND POId = (
            SELECT POId
            FROM ProductionExecution
            WHERE ExecutionId = @ExecutionId
        );
        """;

        await _db.ExecuteAsync(updateStepSql,
            new { ExecutionId = executionId }, tx);

        tx.Commit();

        return Ok();
    }
}
