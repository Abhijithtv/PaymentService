namespace PaymentAPI.Services
{
    public class LogService(ILogger<LogService> logger) : ILogService
    {
        public void LogInfo(string info) => logger.LogInformation(info);

        public void LogWarning(string warning) => logger.LogWarning(warning);

        public void LogError(string error) => logger.LogError(error);
    }
}
