using System.Text.Json.Serialization;
using FluentValidation;
using ToolShare.Data;
using ToolShare.Domain;
using ToolShare.Endpoints;
using ToolShare.ErrorHandling;
using ToolShare.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<ToolShareExceptionHandler>();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.ConfigureHttpJsonOptions(o => o.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddSingleton<IRepository<Member>, InMemoryMemberRepository>();
builder.Services.AddSingleton<IRepository<Tool>, InMemoryToolRepository>();
builder.Services.AddSingleton<ILoanRepository, InMemoryLoanRepository>();
builder.Services.AddSingleton<IRepository<Loan>>(sp => sp.GetRequiredService<ILoanRepository>());
builder.Services.AddSingleton<IIdempotencyStore, InMemoryIdempotencyStore>();
builder.Services.AddSingleton<ILoanService, LoanService>();
builder.Services.AddHostedService<DataSeeder>();

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapMemberEndpoints();
app.MapToolEndpoints();
app.MapLoanEndpoints();

app.Run();

public partial class Program { }