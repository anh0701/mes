public class PlantService
{
    private readonly PlantRepository _repo;

    public PlantService(PlantRepository repo)
    {
        _repo = repo;
    }

    public async Task<PlantDto> CreateAsync (PlantRequest req)
    {
        return await _repo.InsertAsync(req.PlantCode, req.PlantName);
    }
}