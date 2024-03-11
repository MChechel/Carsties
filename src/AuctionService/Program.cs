using AuctionService.Data;
using Microsoft.EntityFrameworkCore;

using Microsoft.AspNetCore.Mvc;
using MassTransit;
using AuctionService.Consumers;
[assembly: ApiController]
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<AuctionDbContext>(opt =>
{
    opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));

});

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddMassTransit( x => {

    x.AddEntityFrameworkOutbox<AuctionDbContext>( o => {
        o.QueryDelay = TimeSpan.FromSeconds(10);

        o.UsePostgres();
        o.UseBusOutbox();
    });

    x.AddConsumersFromNamespaceContaining<AuctionCreatedFaultConsumer>();
    x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("auction", false));

    x.UsingRabbitMq((context, cfg) => {
        cfg.ConfigureEndpoints(context);
    });
});

var app = builder.Build();

app.UseAuthorization();

app.MapControllers();

 // Get a reference to the list of controller descriptors
        var controllerDescriptors = app.Services.GetRequiredService<IEnumerable<string>>();

        Console.WriteLine("List of loaded controllers:" + controllerDescriptors);
        foreach (var controllerDescriptor in controllerDescriptors)
        {
            Console.WriteLine($"- {controllerDescriptor}");
        }

try
{
    DbInitialier.InitDb(app);
}
catch (System.Exception e)
{
    Console.WriteLine(e);
}

app.Run();
