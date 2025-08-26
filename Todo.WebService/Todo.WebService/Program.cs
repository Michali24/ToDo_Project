//using AutoMapper;
//using Microsoft.EntityFrameworkCore;
//using Todo.Mapping;
//using Microsoft.AspNetCore.Authentication.JwtBearer;
//using Microsoft.IdentityModel.Tokens;
using System.Text;
using ToDo.Core.Interfaces.Services;
using ToDo.Service.Services;
using ToDo.Core.Settings;
using ToDo.Core.Interfaces.Repositories;



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
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
