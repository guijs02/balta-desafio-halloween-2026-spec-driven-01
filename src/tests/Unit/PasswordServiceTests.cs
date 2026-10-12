using Application;
using Domain;

namespace Unit;

public sealed class PasswordServiceTests
{
    [Fact]
    public async Task GenerateAsync_PersistsRecordBeforeReturningResponse()
    {
        var repository = new InMemoryPasswordRepository();
        var service = new PasswordService(new PasswordGenerator(), repository);

        var response = await service.GenerateAsync(CancellationToken.None);

        Assert.True(repository.Records.ContainsKey(response.Id));
        Assert.Equal(response.Password, repository.Records[response.Id].Value);
    }

    [Fact]
    public async Task GenerateAsync_PropagatesPersistenceFailureWithoutReturningSuccess()
    {
        var service = new PasswordService(new PasswordGenerator(), new FailingPasswordRepository());

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.GenerateAsync(CancellationToken.None));
    }

    [Fact]
    public async Task GenerateAsync_KeepsPreviousRecordsWhenGeneratingAgain()
    {
        var repository = new InMemoryPasswordRepository();
        var service = new PasswordService(new PasswordGenerator(), repository);

        var firstResponse = await service.GenerateAsync(CancellationToken.None);
        var secondResponse = await service.GenerateAsync(CancellationToken.None);

        Assert.NotEqual(firstResponse.Id, secondResponse.Id);
        Assert.Equal(2, repository.Records.Count);
        Assert.Equal(firstResponse.Password, repository.Records[firstResponse.Id].Value);
    }

    private sealed class InMemoryPasswordRepository : IPasswordRepository
    {
        public Dictionary<Guid, PasswordRecord> Records { get; } = [];

        public Task AddAsync(PasswordRecord passwordRecord, CancellationToken cancellationToken)
        {
            Records.Add(passwordRecord.Id, passwordRecord);
            return Task.CompletedTask;
        }

        public Task<PasswordRecord?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            Records.TryGetValue(id, out var passwordRecord);
            return Task.FromResult(passwordRecord);
        }
    }

    private sealed class FailingPasswordRepository : IPasswordRepository
    {
        public Task AddAsync(PasswordRecord passwordRecord, CancellationToken cancellationToken)
        {
            throw new InvalidOperationException("Falha de persistência simulada.");
        }

        public Task<PasswordRecord?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            throw new InvalidOperationException("Falha de persistência simulada.");
        }
    }
}
