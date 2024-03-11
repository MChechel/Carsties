using AuctionService.Data;
using Microsoft.EntityFrameworkCore;

using Microsoft.AspNetCore.Mvc;
[assembly: ApiController]
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddDbContext<AuctionDbContext>(opt =>
{
    opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));

});

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
// builder.Services.AddEndpointsApiExplorer();

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
