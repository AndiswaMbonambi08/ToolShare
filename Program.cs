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

builder.Services.AddSingleton<IRepository<Member>, InMemoryMemberRepository>();
builder.Services.AddSingleton<IRepository<Tool>, InMemoryToolRepository>();
builder.Services.AddSingleton<ILoanRepository, InMemoryLoanRepository>();
builder.Services.AddSingleton<IRepository<Loan>>(sp => sp.GetRequiredService<ILoanRepository>());
builder.Services.AddSingleton<IIdempotencyStore, InMemoryIdempotencyStore>();
builder.Services.AddSingleton<ILoanService, LoanService>();

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapMemberEndpoints();
app.MapToolEndpoints();
app.MapLoanEndpoints();

using (var scope = app.Services.CreateScope())
{
    var memberRepo = scope.ServiceProvider.GetRequiredService<IRepository<Member>>();
    var toolRepo = scope.ServiceProvider.GetRequiredService<IRepository<Tool>>();

    var thabo = new Member("Thabo Nkosi", "thabo@example.com");
    var lerato = new Member("Lerato Dube", "lerato@example.com");
    await memberRepo.AddAsync(thabo);
    await memberRepo.AddAsync(lerato);

    await toolRepo.AddAsync(new Tool("Drill", "Power Tools", thabo.Id));
    await toolRepo.AddAsync(new Tool("Ladder", "General", lerato.Id));
}

app.Run();

public partial class Program { }