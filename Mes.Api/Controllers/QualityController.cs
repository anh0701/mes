using System.Data;
using Dapper;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/quality")]
[Tags("05 - Quality Control")]
public class QualityController : ControllerBase
{
    private readonly IDbConnection _db;

    public QualityController(IDbConnection db)
    {
        _db = db;
    }

    // [HttpPost("inspection")]
    // public async Task<IActionResult> Inspect(CreateInspectionRequest req)
    // {
    //     var sql = """
    //     INSERT INTO QualityInspection
    //     (ExecutionId, Result, Status, InspectionTime)
    //     VALUES (@ExecutionId, @Result, @Status, GETDATE());
    //     """;

    //     await _db.ExecuteAsync(sql, req);
    //     return Ok();
    // }

    [HttpPost("pass")]
    public async Task<IActionResult> Pass(PassInspectionRequest req)
    {
        using var tx = _db.BeginTransaction();

        var checkSql = """
        SELECT EndTime
        FROM ProductionExecution
        WHERE ExecutionId = @ExecutionId
        """;

        var endTime = await _db.ExecuteScalarAsync<DateTime?>(
            checkSql, req, tx);

        if (endTime == null)
            return BadRequest("Execution not finished");

        var sql = """
        INSERT INTO QualityInspection (ExecutionId, Status)
        VALUES (@ExecutionId, 'Pass');
        """;

        await _db.ExecuteAsync(sql, req, tx);

        tx.Commit();
        return Ok();
    }


    [HttpPost("fail")]
    public async Task<IActionResult> Fail(FailInspectionRequest req)
    {
        using var tx = _db.BeginTransaction();

        var inspectionSql = """
        INSERT INTO QualityInspection (ExecutionId, Status)
        VALUES (@ExecutionId, 'Fail');

        SELECT CAST(SCOPE_IDENTITY() AS INT);
        """;

        var inspectionId = await _db.ExecuteScalarAsync<int>(
            inspectionSql, req, tx);

        var defectSql = """
        INSERT INTO QualityDefect (InspectionId, DefectId, Quantity)
        VALUES (@InspectionId, @DefectId, @Quantity);
        """;

        foreach (var d in req.Defects)
        {
            await _db.ExecuteAsync(defectSql, new
            {
                InspectionId = inspectionId,
                d.DefectId,
                d.Quantity
            }, tx);
        }

        tx.Commit();
        return Ok(new { InspectionId = inspectionId });
    }

}
