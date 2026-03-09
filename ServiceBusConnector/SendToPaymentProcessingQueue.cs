using Azure.Messaging.ServiceBus;

namespace ServiceBusConnector
{
    internal class SendToPaymentProcessingQueue
    {

        public static async Task Send(string msg)
        {
            string connectionString = "conntection_str_of_queue";
            string queueName = "payment-processing";

            // create a Service Bus client 
            var client = new ServiceBusClient(connectionString);

            // get the sender
            var sender = client.CreateSender(queueName);

            // create a message that we can send
            ServiceBusMessage message = new ServiceBusMessage(msg);

            // send the message
            await sender.SendMessageAsync(message);

            Console.WriteLine("Message sent!");

            // always close the sender and client
            await sender.CloseAsync();
            await client.DisposeAsync();
        }
    }
}
