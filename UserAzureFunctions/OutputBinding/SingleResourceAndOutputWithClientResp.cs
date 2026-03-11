using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using UserAzureFunctions.OutputBinding.Models;

namespace UserAzureFunctions.OutputBinding;

public class SingleResourceAndOutputWithClientResp
{
    private readonly ILogger<SingleResourceAndOutputWithClientResp> _logger;

    public SingleResourceAndOutputWithClientResp(ILogger<SingleResourceAndOutputWithClientResp> logger)
    {
        _logger = logger;
    }

    [Function("SingleResourceAndOutputWithClientResp")]
    public async Task<SingleResourceAndOutputWithClientRespEntity> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "api/v1/user-report-entity")] HttpRequestData req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");

        HttpResponseData response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteStringAsync("HTTP response: Message sent");

        return new SingleResourceAndOutputWithClientRespEntity()
        {
            Val = "got it from entity::::::" + req.Query["val"],
            Response = response
        };
    }
}