using System.Net;
using System.Net.Http.Json;
using Application;

namespace Integration;

public sealed class PasswordEndpointsTests
{
    [Fact]
    public async Task PostPasswords_ReturnsPersistedPasswordAndGuid()
    {
        using var factory = new PasswordApiFactory();
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/passwords", new { });

        response.EnsureSuccessStatusCode();
        var password = await response.Content.ReadFromJsonAsync<PasswordResponse>();

        Assert.NotNull(password);
        Assert.NotEqual(Guid.Empty, password.Id);
        Assert.True(Domain.PasswordPolicy.IsValid(password.Password));
    }

    [Fact]
    public async Task GetPasswords_ReturnsThePreviouslyGeneratedPassword()
    {
        using var factory = new PasswordApiFactory();
        using var client = factory.CreateClient();
        var generated = await GeneratePasswordAsync(client);

        var response = await client.GetAsync($"/passwords/{generated.Id}");
        var retrieved = await response.Content.ReadFromJsonAsync<PasswordResponse>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(generated, retrieved);
    }

    [Fact]
    public async Task MultipleGenerations_KeepEarlierPasswordsAvailable()
    {
        using var factory = new PasswordApiFactory();
        using var client = factory.CreateClient();
        var first = await GeneratePasswordAsync(client);
        var second = await GeneratePasswordAsync(client);

        Assert.NotEqual(first.Id, second.Id);
        Assert.Equal(first, await client.GetFromJsonAsync<PasswordResponse>($"/passwords/{first.Id}"));
        Assert.Equal(second, await client.GetFromJsonAsync<PasswordResponse>($"/passwords/{second.Id}"));
    }

    [Theory]
    [InlineData("/passwords/not-a-guid")]
    [InlineData("/passwords/00000000-0000-0000-0000-000000000000")]
    public async Task GetPasswords_InvalidGuid_ReturnsBadRequestProblemDetails(string path)
    {
        using var factory = new PasswordApiFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync(path);
        var problem = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains("Requisição inválida", problem);
        Assert.Contains("O identificador informado é inválido.", problem);
        Assert.Contains("\"status\":400", problem);
    }

    [Fact]
    public async Task GetPasswords_WithoutGuid_ReturnsBadRequestProblemDetails()
    {
        using var factory = new PasswordApiFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/passwords");
        var problem = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains("O identificador informado é inválido.", problem);
        Assert.Contains("\"status\":400", problem);
    }

    [Fact]
    public async Task GetPasswords_UnknownGuid_ReturnsNotFoundProblemDetails()
    {
        using var factory = new PasswordApiFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync($"/passwords/{Guid.NewGuid()}");
        var problem = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Contains("Não encontrado", problem);
        Assert.Contains("Nenhuma senha foi encontrada para o identificador informado.", problem);
        Assert.Contains("\"status\":404", problem);
    }

    [Fact]
    public async Task PostPasswords_WhenPersistenceFails_ReturnsServerErrorWithoutSecret()
    {
        using var factory = new PasswordApiFactory(failWrites: true);
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/passwords", new { });
        var problem = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        Assert.Contains("Não foi possível concluir a operação.", problem);
        Assert.DoesNotContain("\"password\"", problem);
        Assert.DoesNotContain("\"id\"", problem);
    }

    private static async Task<PasswordResponse> GeneratePasswordAsync(HttpClient client)
    {
        var response = await client.PostAsJsonAsync("/passwords", new { });
        response.EnsureSuccessStatusCode();

        return (await response.Content.ReadFromJsonAsync<PasswordResponse>())!;
    }
}
