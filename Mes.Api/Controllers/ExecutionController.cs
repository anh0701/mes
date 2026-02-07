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
        var sql = """
        INSERT INTO ProductionExecution
        (POId, RoutingStepId, MachineId, InputQty)
        VALUES (@PoId, @RoutingStepId, @MachineId, @InputQty);

        UPDATE ProductionOrderStep
        SET Status = 'Running'
        WHERE POId = @PoId AND RoutingStepId = @RoutingStepId;
        """;

        await _db.ExecuteAsync(sql, req);
        return Ok();
    }

    [HttpPost("{executionId}/finish")]
    public async Task<IActionResult> Finish(
        int executionId,
        int outputQty,
        int ngQty)
    {
        var sql = """
        UPDATE ProductionExecution
        SET EndTime = GETDATE(),
            OutputQty = @OutputQty,
            NGQty = @NgQty
        WHERE ExecutionId = @ExecutionId;
        """;

        await _db.ExecuteAsync(sql, new
        {
            ExecutionId = executionId,
            OutputQty = outputQty,
            NgQty = ngQty
        });

        return Ok();
    }
}
