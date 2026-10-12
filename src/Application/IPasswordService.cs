namespace Application;

public interface IPasswordService
{
    Task<PasswordResponse> GenerateAsync(CancellationToken cancellationToken);

    Task<PasswordResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
}
