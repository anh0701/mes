using System.Data;
using Dapper;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/machines")]
[Tags("02 - Machine & Line")]
public class MachineController : ControllerBase
{
    private readonly IDbConnection _db;

    public MachineController(IDbConnection db)
    {
        _db = db;
    }

    [HttpPost("{machineId}/status")]
    public async Task<IActionResult> ChangeStatus(int machineId, string status)
    {
        var sql = """
        UPDATE MachineStatus
        SET EndTime = GETDATE()
        WHERE MachineId = @MachineId AND EndTime IS NULL;

        INSERT INTO MachineStatus (MachineId, Status)
        VALUES (@MachineId, @Status);
        """;

        await _db.ExecuteAsync(sql, new
        {
            MachineId = machineId,
            Status = status
        });

        return Ok();
    }
}
