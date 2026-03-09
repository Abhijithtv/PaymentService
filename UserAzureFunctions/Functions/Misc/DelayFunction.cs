using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace UserAzureFunctions.Functions.Misc;

public class DelayFunction
{
    private readonly ILogger<DelayFunction> _logger;

    public DelayFunction(ILogger<DelayFunction> logger)
    {
        _logger = logger;
    }

    [Function("DelayFunction")]
    public async Task<IActionResult> RunAsync([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "api/v1/delay")] HttpRequest req)
    {
        _logger.LogInformation("(My Info)C# HTTP trigger function processed a request.  Abhijith");
        _logger.LogDebug("(My debug)C# HTTP trigger function processed a request.  Abhijith");
        _logger.LogError("(My Error)C# HTTP trigger function processed a request.  Abhijith");
        _logger.LogCritical("(My Critical)C# HTTP trigger function processed a request.  Abhijith");
        _logger.LogTrace("(My Trace)C# HTTP trigger function processed a request.  Abhijith");
        _logger.LogWarning("(My Warning)C# HTTP trigger function processed a request.  Abhijith");

        await Task.Delay(1000);
        return new OkObjectResult("Welcome to Azure Functions!");
    }
}