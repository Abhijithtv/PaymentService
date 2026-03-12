using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using UserAzureFunctions.OutputBinding.Models;

namespace UserAzureFunctions.OutputBinding.MultiDestination;

public class MultiDestinationFuncByEntity
{
    private readonly ILogger<MultiDestinationFuncByEntity> _logger;

    public MultiDestinationFuncByEntity(ILogger<MultiDestinationFuncByEntity> logger)
    {
        _logger = logger;
    }

    [Function("MultiDestinationFuncByEntity")]
    public async Task<MultiDesinationAndOutputWithClient> Run([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequestData req)
    {
        var msg = "from-" + nameof(MultiDestinationFuncByEntity) + "::::::" + req.Query["val"];
        var clientResp = req.CreateResponse();
        await clientResp.WriteStringAsync(msg);
        return new MultiDesinationAndOutputWithClient()
        {
            HttpResponse = clientResp,
            UserAuditResponse = msg,
            UserReportResponse = msg,
        };
    }
}