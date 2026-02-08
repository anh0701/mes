using System.Data;
using Dapper;

public class PlantRepository
{
    private readonly IDbConnection _db;
    
    public PlantRepository(IDbConnection db)
    {
        _db = db;
    }
    public async Task<PlantDto> InsertAsync(string plantCode, string plantName)
    {
        var sql = """
        INSERT INTO Plant (PlantCode, PlantName)
        OUTPUT 
            INSERTED.PlantId,
            INSERTED.PlantCode,
            INSERTED.PlantName
        VALUES (@PlantCode, @PlantName);
        """;

        return await _db.QuerySingleAsync<PlantDto>(
            sql,
            new { PlantCode = plantCode, PlantName = plantName }
        );
    }

}