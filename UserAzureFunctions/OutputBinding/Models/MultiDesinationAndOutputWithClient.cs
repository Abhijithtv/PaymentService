using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace UserAzureFunctions.OutputBinding.Models
{
    public class MultiDesinationAndOutputWithClient
    {
        [ServiceBusOutput("user-report-gen-queue", Connection = "UserReportGenQueue_Conn_Str")]
        public string UserReportResponse { get; set; }

        [HttpResult]
        public HttpResponseData HttpResponse { get; set; }

        [ServiceBusOutput("user-audit-queue", Connection = "UserAuditQueue_Conn_Str")]
        public string UserAuditResponse { get; set; }


    }
}
