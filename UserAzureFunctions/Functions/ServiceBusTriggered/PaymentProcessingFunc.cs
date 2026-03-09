using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace UserAzureFunctions.Functions.ServiceBusTriggered;

public class PaymentProcessingFunc
{
    private readonly ILogger<PaymentProcessingFunc> _logger;

    public PaymentProcessingFunc(ILogger<PaymentProcessingFunc> logger)
    {
        _logger = logger;
    }

    [Function(nameof(PaymentProcessingFunc))]
    public async Task Run(
        [ServiceBusTrigger("payment-processing", Connection = "Payment_Processing_Queue_Conn_Str")]
        ServiceBusReceivedMessage message,
        ServiceBusMessageActions messageActions)
    {
        _logger.LogInformation("Message ID: {id}", message.MessageId);
        _logger.LogInformation("Message Body: {body}", message.Body);
        _logger.LogInformation("Message Content-Type: {contentType}", message.ContentType);


        _logger.LogError("Message ID: {id}", message.MessageId);
        _logger.LogError("Message Body: {body}", message.Body);
        _logger.LogError("Message Content-Type: {contentType}", message.ContentType);

        _logger.LogCritical("Message ID: {id}", message.MessageId);
        _logger.LogCritical("Message Body: {body}", message.Body);
        _logger.LogCritical("Message Content-Type: {contentType}", message.ContentType);

        _logger.LogWarning("Message ID: {id}", message.MessageId);
        _logger.LogWarning("Message Body: {body}", message.Body);
        _logger.LogWarning("Message Content-Type: {contentType}", message.ContentType);

        // Complete the message
        await messageActions.CompleteMessageAsync(message);
    }
}