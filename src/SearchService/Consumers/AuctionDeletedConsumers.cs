namespace SearchService.Consumers;

using AutoMapper;
using Contracts;
using MassTransit;
using MongoDB.Entities;
using SearchService;

public class AuctionDeletedConsumers : IConsumer<AuctionCreated>
{
    private readonly IMapper _mapper;

    public AuctionDeletedConsumers(IMapper mapper){
        _mapper = mapper;
    }
    public async Task Consume(ConsumeContext<AuctionCreated> context)
    {
        Console.WriteLine(" --> Consume auction deleted: " + context.Message.Id);

        
        var result = await DB.DeleteAsync<Item>(context.Message.Id);

        if(!result.IsAcknowledged)
            throw new MessageException(typeof(AuctionUpdated), "Problem deleting mongoDB");
    }


}