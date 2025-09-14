using ToDo.Core.Interfaces.Services;
using ToDo.Core.Settings;
using ToDo.Service.Services;

var builder = WebApplication.CreateBuilder(args);

// ‰‚„¯˙ RabbitMQ Ó‰Œappsettings
builder.Services.Configure<RabbitMqSettings>(
    builder.Configuration.GetSection("RabbitMq"));

//Dependency Injection
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IItemService, ItemService>();
builder.Services.AddScoped<IRabbitMqService, RabbitMqService>();

//Add services to the container.
builder.Services.AddControllers();
//suport to Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
//Create Automatic Documentation
builder.Services.AddSwaggerGen();
//Build the application
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
