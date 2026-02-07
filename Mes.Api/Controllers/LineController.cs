using System.Data;
using Dapper;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/lines")]
[Tags("01 - Line")]
public class LineController: ControllerBase
{
    private readonly IDbConnection _db;

    public LineController(IDbConnection db)
    {
        _db = db;
    }

    [HttpPost]
    public async Task<IActionResult> Create(LineRequest req)
    {
        var sql = """
        INSERT INTO Line (PlantId, LineCode, LineName)
        VALUES (@PlantId, @LineCode, @LineName);
        """;

        await _db.ExecuteAsync(sql, req);
        return Ok();
    }
}