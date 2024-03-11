
using Contracts;
using MassTransit;

namespace AuctionService.Consumers;

public class AuctionCreatedFaultConsumer : IConsumer<Fault<AuctionCreated>>
{
    public Task Consume(ConsumeContext<Fault<AuctionCreated>> context)
    {
        
        var exception = context.Message.Exceptions.First();
        Console.WriteLine(" --> Consuming faulty createing. Error type: " + exception.ExceptionType);
        
        if(exception.ExceptionType == "System.ArgumentException")
        {
            context.Message.Message.Model = "FooBar";
            context.Publish(context.Message.Message);
        }
        Console.WriteLine(" Not an argument exception - update error dashboard somehow");
        throw new NotImplementedException();
    }
}