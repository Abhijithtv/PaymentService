using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace UserAzureFunctions.Functions;

public class ErrorFunction
{
    private readonly ILogger<ErrorFunction> _logger;

    public ErrorFunction(ILogger<ErrorFunction> logger)
    {
        _logger = logger;
    }

    [Function("ErrorFunction")]
    public IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "api/v1/error")] HttpRequest req)
    {
        throw new Exception("Error function invokement created the Error-hurray!!!");
    }
}