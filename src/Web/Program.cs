using Application.Extensions;
using Microsoft.OpenApi.Models;
using Serilog;
using Web.Filters;

var builder = WebApplication.CreateBuilder(args);

builder.Host
    .UseSerilog((hostingContext, loggerConfig) =>
        loggerConfig.ReadFrom.Configuration(hostingContext.Configuration)
    );
builder.Services.AddControllers(c => c.Filters.Add(typeof(ExceptionFilter)));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Conversion Rates API", Version = "v1" }));

builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddQuartzJobs();
builder.Services.AddMediatr();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.AddRequestsLogging();
app.MapControllers();

app.Run();
