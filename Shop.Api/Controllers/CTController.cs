using Microsoft.AspNetCore.Mvc;

namespace Shop.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CTController(ILogger<CTController> _logger):ControllerBase
    {
        [NonAction]
        public async Task<IActionResult> TestWithoutCT()

        {

            _logger.LogInformation("TestWithoutCT called");

            await Task.Delay(5000); 

            _logger.LogInformation("Action 1 completed");

            await Task.Delay(5000); 

            _logger.LogInformation("Action 2 completed");

            _logger.LogInformation("TestWithoutCT completed");

            return Ok("TestWithoutCT");

        }

        [NonAction]
        public async Task<IActionResult> TestWithCT(CancellationToken cancellationToken)

        {

            _logger.LogInformation("TestWithCT called");

            await Task.Delay(5000, cancellationToken); 

            _logger.LogInformation("Action 1 completed");

            await Task.Delay(5000, cancellationToken); 

            _logger.LogInformation("Action 2 completed");

            _logger.LogInformation("TestWithCT completed");

            return Ok("TestWithoutCT");

        }
    }
}



