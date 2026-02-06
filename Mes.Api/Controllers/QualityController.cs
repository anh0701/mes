using System.Data;
using Dapper;
using Microsoft.AspNetCore.Mvc;

public record CreateInspectionRequest(
    int ExecutionId,
    string Result,
    string Status
);

[ApiController]
[Route("api/quality")]
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
