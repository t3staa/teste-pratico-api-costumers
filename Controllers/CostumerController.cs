using Microsoft.AspNetCore.Mvc;

namespace teste_pratico.Controllers;

[ApiController]
[Route("[controller]")]
public class CostumersController : ControllerBase
{
    private readonly ILogger<CostumersController> _logger;

    public CostumersController(ILogger<CostumersController> logger)
    {
        _logger = logger;
    }
}
