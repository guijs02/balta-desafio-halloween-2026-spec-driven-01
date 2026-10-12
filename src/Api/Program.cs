using Application;
using Domain;
using Infrastructure;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("Passwords")
    ?? "Data Source=" + Path.Combine(builder.Environment.ContentRootPath, "passwords.db");

builder.Services.AddScoped<IPasswordGenerator, PasswordGenerator>();
builder.Services.AddScoped<IPasswordService, PasswordService>();
builder.Services.AddScoped<IPasswordRepository, EfPasswordRepository>();
builder.Services.AddDbContext<PasswordsDbContext>(options => options.UseSqlite(connectionString));

var app = builder.Build();

app.UseExceptionHandler(exceptionHandler =>
{
    exceptionHandler.Run(async context =>
    {
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await Results.Problem(
            title: "Falha ao processar a solicitação",
            detail: "Não foi possível concluir a operação.",
            statusCode: StatusCodes.Status500InternalServerError)
            .ExecuteAsync(context);
    });
});

app.MapPost("/passwords", async (
    IPasswordService passwordService,
    CancellationToken cancellationToken) =>
{
    var password = await passwordService.GenerateAsync(cancellationToken);
    return Results.Ok(password);
});

app.MapGet("/passwords/{id}", async (
    string id,
    IPasswordService passwordService,
    CancellationToken cancellationToken) =>
{
    if (!Guid.TryParse(id, out var passwordId) || passwordId == Guid.Empty)
    {
        return Results.Problem(
            type: "https://tools.ietf.org/html/rfc9110#section-15.5.1",
            title: "Requisição inválida",
            detail: "O identificador informado é inválido.",
            statusCode: StatusCodes.Status400BadRequest);
    }

    var password = await passwordService.GetByIdAsync(passwordId, cancellationToken);

    return password is null
        ? Results.Problem(
            type: "https://tools.ietf.org/html/rfc9110#section-15.5.5",
            title: "Não encontrado",
            detail: "Nenhuma senha foi encontrada para o identificador informado.",
            statusCode: StatusCodes.Status404NotFound)
        : Results.Ok(password);
});

app.MapGet("/passwords", () => Results.Problem(
    type: "https://tools.ietf.org/html/rfc9110#section-15.5.1",
    title: "Requisição inválida",
    detail: "O identificador informado é inválido.",
    statusCode: StatusCodes.Status400BadRequest));

await using (var scope = app.Services.CreateAsyncScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<PasswordsDbContext>();
    await dbContext.Database.EnsureCreatedAsync();
}

app.Run();

public partial class Program
{
}
