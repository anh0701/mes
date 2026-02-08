using System.Data;
using Dapper;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/plants")]
[Tags("01 - Plant")]
public class PlantController : ControllerBase
{
    private readonly PlantService _service;
    
    public PlantController (PlantService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> Create(PlantRequest req)
    {
        var dto = await _service.CreateAsync(req);
        return Ok(dto);
    }
}