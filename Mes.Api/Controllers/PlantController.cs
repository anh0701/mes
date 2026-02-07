using System.Data;
using Dapper;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/plants")]
[Tags("01 - Plant")]
public class PlantController : ControllerBase
{
    private readonly IDbConnection _db;
    
    public PlantController (IDbConnection db)
    {
        _db = db;
    }

    [HttpPost]
    public async Task<IActionResult> Create(PlantRequest req)
    {
        var sql = """
        INSERT INTO Plant (PlantCode, PlantName)
        VALUES (@PlantCode, @PlantName);
        """;

        await _db.ExecuteAsync(sql, req);
        return Ok();
    }
}