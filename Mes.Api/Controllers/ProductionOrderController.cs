using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Data;

[ApiController]
[Route("api/production-orders")]
[Tags("03 - Production Order")]
public class ProductionOrderController : ControllerBase
{
    private readonly IDbConnection _db;

    public ProductionOrderController(IDbConnection db)
    {
        _db = db;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreatePoRequest req)
    {
        var sql = """
        INSERT INTO ProductionOrder (POCode, ProductId, PlantQty, Status)
        OUTPUT INSERTED.POId
        VALUES (@PoCode, @ProductId, @PlantQty, 'Planned');
        """;

        var poId = await _db.ExecuteScalarAsync<int>(sql, req);
        return Ok(new { poId });

    }

    [HttpPost("{poId}/release")]
    public async Task<IActionResult> Release(int poId)
    {
        using var tx = _db.BeginTransaction();

        var status = await _db.ExecuteScalarAsync<string>(
            "SELECT Status FROM ProductionOrder WHERE POId = @PoId",
            new { PoId = poId },
            tx
        );

        if (status == null)
            return NotFound("Production order not found");

        if (status != "Planned")
            return BadRequest("Only Planned order can be released");

        var insertStepsSql = """
            INSERT INTO ProductionOrderStep (POId, RoutingStepId, Status)
            SELECT @PoId, rs.RoutingStepId, 'Planned'
            FROM RoutingStep rs
            JOIN Routing r ON r.RoutingId = rs.RoutingId
            JOIN ProductionOrder po ON po.ProductId = r.ProductId
            WHERE po.POId = @PoId;
        """;

        var affected = await _db.ExecuteAsync(insertStepsSql, new { PoId = poId }, tx);

        if (affected == 0)
        {
            tx.Rollback();
            return BadRequest("No routing steps found for this product");
        }

        await _db.ExecuteAsync(
            "UPDATE ProductionOrder SET Status = 'Released' WHERE POId = @PoId",
            new { PoId = poId },
            tx
        );

        tx.Commit();
        return Ok();
    }

}
