using MassTransit;
using Polly;
using Polly.Extensions.Http;
using SearchService;
using SearchService.Consumers;
using SearchService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddControllers();


builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());


builder.Services.AddHttpClient<AuctionSvcHttpClient>()
    .AddPolicyHandler(GetPolicy());

builder.Services.AddMassTransit( x => {

    x.AddConsumersFromNamespaceContaining<AuctionCreatedConsumer>();

    x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("search", false));


    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.ReceiveEndpoint("search-auction-created", e => 
        {
            e.UseMessageRetry( r => r.Interval(5,5));

            e.ConfigureConsumer<AuctionCreatedConsumer>(context);
        });
        cfg.ConfigureEndpoints(context);
    });
});



var app = builder.Build();


app.UseHttpsRedirection();


// app.UseAuthorization();

app.MapControllers();

app.Lifetime.ApplicationStarted.Register(async () => {

    try{
        
        Console.WriteLine("before InitDb");
        await DbInitializer.InitDb(app);
    }
    catch(Exception e){
        Console.WriteLine("before ERROR");
        Console.WriteLine(e);
    }

});

app.Run();


static IAsyncPolicy<HttpResponseMessage> GetPolicy()=> HttpPolicyExtensions
        .HandleTransientHttpError()
        .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
        .WaitAndRetryForeverAsync(_ => TimeSpan.FromSeconds(3));


