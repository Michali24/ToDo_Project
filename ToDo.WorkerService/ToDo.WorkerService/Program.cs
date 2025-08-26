//using ToDo.WorkerService;

//var builder = Host.CreateApplicationBuilder(args);
//builder.Services.AddHostedService<Worker>();

//var host = builder.Build();
//host.Run();


using Microsoft.EntityFrameworkCore;
using ToDo.Core.Interfaces.Repositories;
using ToDo.Core.Interfaces.Services;
using ToDo.Data;
using ToDo.Data.Repositories;
using ToDo.Service.Services;
using ToDo.WorkerService.Consumers;

var builder = Host.CreateApplicationBuilder(args);

// 🗄️ EF DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// 🧱 Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IItemRepository, ItemRepository>();

// 🧠 Services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IItemService, ItemService>();

// ⏳ Consumers 
builder.Services.AddHostedService<UserConsumer>();
builder.Services.AddHostedService<ItemConsumer>();

var host = builder.Build();
host.Run();

