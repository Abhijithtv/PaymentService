using Microsoft.AspNetCore.Mvc;

namespace PaymentAPI.Controllers
{
    [Route("api/v1/env")]
    public class EnviromentController(IConfiguration configuration) : Controller
    {
        [HttpGet("job-worker")]
        public IActionResult GetJobWorkers()
        {
            var res = Environment.GetEnvironmentVariable("JobWorkers");
            return Ok(res);
        }

        [HttpGet("owner")]
        public IActionResult GetOwner()
        {
            var res = Environment.GetEnvironmentVariable("owner");
            return Ok(res);
        }

        [HttpGet("slot-name")]
        public IActionResult GetSlotName()
        {
            var res = Environment.GetEnvironmentVariable("slot-name");
            return Ok(res);
        }

        [HttpGet("mt-connection")]
        public IActionResult GetMTConnections()
        {
            var res = configuration.GetConnectionString("mtdb");
            return Ok(res);
        }


        [HttpGet("error-connection")]
        public IActionResult GetErrorConnections()
        {
            var res = configuration.GetConnectionString("errordb");
            return Ok(res);
        }

        [HttpGet("version")]
        public IActionResult GetVersion()
        {
            var res = configuration.GetSection("Appsettings").GetValue<string>("version");
            return Ok(res);
        }

        [HttpGet("age")]
        public IActionResult GetAge()
        {
            var res = configuration.GetSection("Appsettings").GetValue<int>("age");
            return Ok(res);
        }
    }
}
