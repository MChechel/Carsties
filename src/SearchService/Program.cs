using MongoDB.Driver;
using MongoDB.Entities;
using Polly;
using Polly.Extensions.Http;
using SearchService;
using SearchService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddControllers();
builder.Services.AddHttpClient<AuctionSvcHttpClient>()
    .AddPolicyHandler(GetPolicy());

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


