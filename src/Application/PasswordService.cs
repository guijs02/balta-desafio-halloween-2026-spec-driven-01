using Domain;

namespace Application;

public sealed class PasswordService(
    IPasswordGenerator passwordGenerator,
    IPasswordRepository passwordRepository) : IPasswordService
{
    public async Task<PasswordResponse> GenerateAsync(CancellationToken cancellationToken)
    {
        var passwordRecord = PasswordRecord.Create(passwordGenerator.Generate());
        await passwordRepository.AddAsync(passwordRecord, cancellationToken);

        return new PasswordResponse(passwordRecord.Id, passwordRecord.Value);
    }

    public async Task<PasswordResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var passwordRecord = await passwordRepository.GetByIdAsync(id, cancellationToken);

        return passwordRecord is null
            ? null
            : new PasswordResponse(passwordRecord.Id, passwordRecord.Value);
    }
}
