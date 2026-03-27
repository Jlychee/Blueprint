using Api.Application.Common;
using Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();


builder
    .LoadEnvFiles()
    .AddSwagger()
    .AddApplicationServices()
    .AddDatabase()
    .AddInfrastructureServices();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.Run();