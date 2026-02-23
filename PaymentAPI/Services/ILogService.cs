namespace PaymentAPI.Services
{
    public interface ILogService
    {
        void LogError(string error);
        void LogInfo(string info);
        void LogWarning(string warning);
    }
}