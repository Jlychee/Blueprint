using System.Reflection;
using Api.Application.Common;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder
    .LoadEnvFiles()
    .AddApplicationServices()
    .AddSwagger();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.Run();