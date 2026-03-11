using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace UserAzureFunctions.OutputBinding.Models
{
    public class SingleResourceAndOutputWithClientRespEntity
    {
        [ServiceBusOutput("user-report-gen-queue", Connection = "UserReportGenQueue_Conn_Str")]
        public string Val { get; set; }

        [HttpResult]
        public HttpResponseData Response { get; set; }
    }
}
