using System.Data;
using Dapper;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/downtime")]
[Tags("05 - Downtime")]
public class DowntimeController : ControllerBase
{
    private readonly IDbConnection _db;

    public DowntimeController(IDbConnection db)
    {
        _db = db;
    }

    [HttpPost("start")]
    public async Task<IActionResult> Start(StartDowntimeRequest req)
    {
        using var tx = _db.BeginTransaction();

        var checkSql = """
        SELECT TOP 1 Status
        FROM MachineStatus
        WHERE MachineId = @MachineId
        ORDER BY StartTime DESC
        """;

        var currentStatus = await _db.ExecuteScalarAsync<string>(
            checkSql, req, tx);

        if (currentStatus == "Down")
            return BadRequest("Machine is already down");

        var endStatusSql = """
        UPDATE MachineStatus
        SET EndTime = GETDATE()
        WHERE MachineId = @MachineId
          AND EndTime IS NULL;
        """;

        await _db.ExecuteAsync(endStatusSql, req, tx);

        var insertStatusSql = """
        INSERT INTO MachineStatus (MachineId, Status)
        VALUES (@MachineId, 'Down');
        """;

        await _db.ExecuteAsync(insertStatusSql, req, tx);

        var insertDowntimeSql = """
        INSERT INTO MachineDowntime (MachineId, ReasonId)
        VALUES (@MachineId, @ReasonId);

        SELECT CAST(SCOPE_IDENTITY() AS INT);
        """;

        var downtimeId = await _db.ExecuteScalarAsync<int>(
            insertDowntimeSql, req, tx);

        tx.Commit();
        return Ok(new { DowntimeId = downtimeId });
    }

    [HttpPost("{downtimeId}/end")]
    public async Task<IActionResult> End(int downtimeId)
    {
        using var tx = _db.BeginTransaction();

        var endDowntimeSql = """
        UPDATE MachineDowntime
        SET EndTime = GETDATE()
        WHERE DowntimeId = @DowntimeId;
        """;

        await _db.ExecuteAsync(endDowntimeSql,
            new { DowntimeId = downtimeId }, tx);

        var endStatusSql = """
        UPDATE MachineStatus
        SET EndTime = GETDATE()
        WHERE Status = 'Down'
          AND EndTime IS NULL
          AND MachineId = (
            SELECT MachineId
            FROM MachineDowntime
            WHERE DowntimeId = @DowntimeId
          );
        """;

        await _db.ExecuteAsync(endStatusSql,
            new { DowntimeId = downtimeId }, tx);

        var insertIdleSql = """
        INSERT INTO MachineStatus (MachineId, Status)
        SELECT MachineId, 'Idle'
        FROM MachineDowntime
        WHERE DowntimeId = @DowntimeId;
        """;

        await _db.ExecuteAsync(insertIdleSql,
            new { DowntimeId = downtimeId }, tx);

        tx.Commit();
        return Ok();
    }
}
