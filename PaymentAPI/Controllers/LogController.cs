using Microsoft.AspNetCore.Mvc;
using PaymentAPI.Services;

namespace PaymentAPI.Controllers
{
    [Route("api/v1/logger")]
    public class LogController(ILogService logService) : Controller
    {
        [HttpGet("log")]
        public IActionResult LogInfo([FromQuery] string info)
        {
            logService.LogInfo(info);
            return Ok(true);
        }

        [HttpGet("error")]
        public IActionResult LogError([FromQuery] string error)
        {
            logService.LogError(error);
            return Ok(true);
        }

        [HttpGet("warning")]
        public IActionResult LogWarning([FromQuery] string warning)
        {
            logService.LogWarning(warning);
            return Ok(true);
        }
    }
}
