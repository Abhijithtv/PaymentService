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

        [HttpGet("debug")]
        public IActionResult LogDebug([FromQuery] string str)
        {
            logService.LogDebug(str);
            return Ok(true);
        }

        [HttpGet("critical")]
        public IActionResult LogCritical([FromQuery] string str)
        {
            logService.LogCritical(str);
            return Ok(true);
        }
    }
}
