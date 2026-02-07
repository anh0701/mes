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
        VALUES (@PoCode, @ProductId, @PlantQty, 'Planned');
        """;

        await _db.ExecuteAsync(sql, req);
        return Ok();
    }

    [HttpPost("{poId}/release")]
    public async Task<IActionResult> Release(int poId)
    {
        var sql = """
        INSERT INTO ProductionOrderStep (POId, RoutingStepId, Status)
        SELECT @PoId, rs.RoutingStepId, 'Planned'
        FROM RoutingStep rs
        JOIN Routing r ON r.RoutingId = rs.RoutingId
        JOIN ProductionOrder po ON po.ProductId = r.ProductId
        WHERE po.POId = @PoId;

        UPDATE ProductionOrder
        SET Status = 'Released'
        WHERE POId = @PoId;
        """;

        await _db.ExecuteAsync(sql, new { PoId = poId });
        return Ok();
    }
}
