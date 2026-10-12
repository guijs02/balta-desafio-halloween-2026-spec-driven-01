using Domain;

namespace Application;

public interface IPasswordRepository
{
    Task AddAsync(PasswordRecord passwordRecord, CancellationToken cancellationToken);

    Task<PasswordRecord?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
}
