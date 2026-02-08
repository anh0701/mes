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

    [HttpPost]
    public async Task<IActionResult> Create(MachineRequest req)
    {
        var sql = """
        INSERT INTO Machine (LineId, MachineCode, MachineName)
        VALUES (@LineId, @MachineCode, @MachineName);
        """;

        await _db.ExecuteAsync(sql, req);
        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> GetMachines()
    {
        var sql = """
            SELECT * FROM Machine;
        """;

        var data = await _db.ExecuteAsync(sql);
        return Ok(new {data});
    }

    [HttpPost("{machineId}/line")]
    public async Task<IActionResult> ChangeLine(int machineId, int lineId)
    {
        using var tx = _db.BeginTransaction();

        var status = await _db.ExecuteScalarAsync<string>(
            "SELECT MachineCode FROM Machine WHERE MachineId = @MachineId;",
            new { MachineId = machineId },
            tx
        );

        if (status == null)
            return NotFound("Machine not found");

        var closeOldLineSql = """
        UPDATE MachineLineHistory
        SET ToTime = SYSDATETIME()
        WHERE MachineId = @MachineId
        AND ToTime IS NULL;
        """;
        var affected = await _db.ExecuteAsync(
            closeOldLineSql, 
            new { MachineId = machineId }, 
            tx
        );

        if (affected == 0)
        {
            tx.Rollback();
            return BadRequest("Can't close old line of this machine!");
        }

        var assignNewLineSql = """
        INSERT INTO MachineLineHistory(MachineId, LineId, FromTime)
        VALUES (@MachineId, @NewLineId, SYSDATETIME());
        """;

        await _db.ExecuteAsync(
            assignNewLineSql,
            new { MachineId = machineId, NewLineId = lineId },
            tx
        );
        
        tx.Commit();
        return Ok();
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
