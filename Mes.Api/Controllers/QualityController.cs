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

    [HttpPost("inspection")]
    public async Task<IActionResult> Inspect(CreateInspectionRequest req)
    {
        var sql = """
        INSERT INTO QualityInspection
        (ExecutionId, Result, Status, InspectionTime)
        VALUES (@ExecutionId, @Result, @Status, GETDATE());
        """;

        await _db.ExecuteAsync(sql, req);
        return Ok();
    }
}
