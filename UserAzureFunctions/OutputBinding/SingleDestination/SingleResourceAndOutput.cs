using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace UserAzureFunctions.OutputBinding.SingleDestination;

public class SingleResourceAndOutput
{
    private readonly ILogger<SingleResourceAndOutput> _logger;

    public SingleResourceAndOutput(ILogger<SingleResourceAndOutput> logger)
    {
        _logger = logger;
    }

    [Function("SingleResourceAndOutput")]
    [ServiceBusOutput("user-report-gen-queue", Connection = "UserReportGenQueue_Conn_Str")]
    public string Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "api/v1/user-report")] HttpRequest req)
    {
        _logger.LogInformation("req body" + req.Query["val"]);
        return "got it::::::" + req.Query["val"];
    }
}