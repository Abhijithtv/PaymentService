using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace UserAzureFunctions.OutputBinding;

public class OutputBindingUsingParameter
{
    private readonly ILogger<OutputBindingUsingParameter> _logger;

    public OutputBindingUsingParameter(ILogger<OutputBindingUsingParameter> logger)
    {
        _logger = logger;
    }

    [FunctionName("OutputBindingUsingParameter")]
    public IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "api/v1/user-report-by-param")] HttpRequest req,
       [ServiceBus("user-report-gen-queue", Connection = "UserReportGenQueue_Conn_Str")] IAsyncCollector<string> asyncCollector)
    {
        var val = "got it from param bindin::::::" + req.Query["val"];
        asyncCollector.AddAsync(val);
        return new OkObjectResult("Welcome to Azure Functions!");
    }

}
