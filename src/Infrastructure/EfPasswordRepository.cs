using Application;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure;

public sealed class EfPasswordRepository(PasswordsDbContext dbContext) : IPasswordRepository
{
    public async Task AddAsync(PasswordRecord passwordRecord, CancellationToken cancellationToken)
    {
        await dbContext.PasswordRecords.AddAsync(passwordRecord, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<PasswordRecord?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return dbContext.PasswordRecords
            .AsNoTracking()
            .SingleOrDefaultAsync(record => record.Id == id, cancellationToken);
    }
}
