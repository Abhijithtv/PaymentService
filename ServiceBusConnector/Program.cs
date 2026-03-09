namespace ServiceBusConnector
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            do
            {
                Console.WriteLine("Enter msg to sent-");
                var msg = Console.ReadLine();
                await SendToPaymentProcessingQueue.Send(msg);
                Console.WriteLine("Press Y to continue");
                var cmd = Console.ReadLine();
                if (cmd[0] != 'Y')
                {
                    break;
                }
            } while (true);
        }
    }
}
