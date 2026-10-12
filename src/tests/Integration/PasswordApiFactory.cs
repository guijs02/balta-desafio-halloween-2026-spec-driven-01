using Application;
using Domain;
using Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Integration;

public sealed class PasswordApiFactory(bool failWrites = false) : WebApplicationFactory<Program>
{
    private readonly SqliteConnection connection = new("Data Source=:memory:");

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        connection.Open();

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<PasswordsDbContext>>();
            services.AddDbContext<PasswordsDbContext>(options => options.UseSqlite(connection));
            services.RemoveAll<IPasswordRepository>();

            if (failWrites)
            {
                services.AddSingleton<IPasswordRepository, FailingPasswordRepository>();
            }
            else
            {
                services.AddScoped<IPasswordRepository, EfPasswordRepository>();
            }
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (disposing)
        {
            connection.Dispose();
        }
    }

    private sealed class FailingPasswordRepository : IPasswordRepository
    {
        public Task AddAsync(Domain.PasswordRecord passwordRecord, CancellationToken cancellationToken)
        {
            throw new InvalidOperationException("Falha de persistência simulada.");
        }

        public Task<Domain.PasswordRecord?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            throw new InvalidOperationException("Falha de persistência simulada.");
        }
    }
}
