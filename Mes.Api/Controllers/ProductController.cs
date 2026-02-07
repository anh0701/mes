using System.Data;
using Dapper;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/products")]
[Tags("01 - Product Master")]
public class ProductController : ControllerBase
{
    private readonly IDbConnection _db;

    public ProductController(IDbConnection db)
    {
        _db = db;
    }

    [HttpPost("add")]
    public async Task<IActionResult> add(ProductRequest req)
    {
        var sql = """
        INSERT INTO Product
        (ProductCode, ProductName)
        VALUES (@ProductCode, @ProductName);
        """;

        await _db.ExecuteAsync(sql, req);
        return Ok();
    }
}