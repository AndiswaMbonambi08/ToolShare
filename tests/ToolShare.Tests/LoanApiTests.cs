using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using ToolShare.Data;

namespace ToolShare.Tests;

public class LoanApiTests
{
    private static object LoanBody(int days = 7)
        => new { toolId = SeedData.DrillId, borrowerId = SeedData.LeratoId, dueDate = DateTime.UtcNow.AddDays(days) };

    private static HttpRequestMessage Checkout(object body, string? key)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/loans") { Content = JsonContent.Create(body) };
        if (key is not null) request.Headers.Add("Idempotency-Key", key);
        return request;
    }

    private static string NewKey() => Guid.NewGuid().ToString();

    private static async Task<JsonElement> ReadJson(HttpResponseMessage response)
        => await response.Content.ReadFromJsonAsync<JsonElement>();

    [Fact]
    public async Task GetTools_ReturnsSeededTools()
    {
        await using var factory = new WebApplicationFactory<global::Program>();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/tools");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(2, (await ReadJson(response)).GetArrayLength());
    }

    [Fact]
    public async Task Checkout_Valid_Returns201WithLocationAndTextStatus()
    {
        await using var factory = new WebApplicationFactory<global::Program>();
        using var client = factory.CreateClient();

        var response = await client.SendAsync(Checkout(LoanBody(), NewKey()));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);
        Assert.Equal("CheckedOut", (await ReadJson(response)).GetProperty("status").GetString());
    }

    [Fact]
    public async Task Checkout_MissingKey_Returns400ProblemJson()
    {
        await using var factory = new WebApplicationFactory<global::Program>();
        using var client = factory.CreateClient();

        var response = await client.SendAsync(Checkout(LoanBody(), key: null));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Checkout_ToolAlreadyOut_Returns409()
    {
        await using var factory = new WebApplicationFactory<global::Program>();
        using var client = factory.CreateClient();

        await client.SendAsync(Checkout(LoanBody(), NewKey()));
        var second = await client.SendAsync(Checkout(LoanBody(), NewKey()));

        Assert.Equal(HttpStatusCode.Conflict, second.StatusCode);
    }

    [Fact]
    public async Task Checkout_ReplaySameKey_ReturnsSameLoan()
    {
        await using var factory = new WebApplicationFactory<global::Program>();
        using var client = factory.CreateClient();
        var body = LoanBody();
        var key = NewKey();

        var first = await ReadJson(await client.SendAsync(Checkout(body, key)));
        var replayResponse = await client.SendAsync(Checkout(body, key));
        var replay = await ReadJson(replayResponse);

        Assert.Equal(HttpStatusCode.Created, replayResponse.StatusCode);
        Assert.Equal(first.GetProperty("id").GetGuid(), replay.GetProperty("id").GetGuid());
    }

    [Fact]
    public async Task Checkout_SameKeyDifferentBody_Returns422()
    {
        await using var factory = new WebApplicationFactory<global::Program>();
        using var client = factory.CreateClient();
        var key = NewKey();

        await client.SendAsync(Checkout(LoanBody(days: 7), key));
        var response = await client.SendAsync(Checkout(LoanBody(days: 14), key));

        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
    }

    [Fact]
    public async Task Checkout_EmptyIds_Returns400WithFieldErrors()
    {
        await using var factory = new WebApplicationFactory<global::Program>();
        using var client = factory.CreateClient();
        var body = new { toolId = Guid.Empty, borrowerId = Guid.Empty, dueDate = DateTime.UtcNow.AddDays(7) };

        var response = await client.SendAsync(Checkout(body, NewKey()));
        var json = await ReadJson(response);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.True(json.GetProperty("errors").TryGetProperty("toolId", out _));
        Assert.True(json.GetProperty("errors").TryGetProperty("borrowerId", out _));
    }

    [Fact]
    public async Task Return_Twice_SecondReturns409()
    {
        await using var factory = new WebApplicationFactory<global::Program>();
        using var client = factory.CreateClient();

        var loan = await ReadJson(await client.SendAsync(Checkout(LoanBody(), NewKey())));
        var id = loan.GetProperty("id").GetGuid();

        var first = await client.PostAsync($"/api/loans/{id}/return", null);
        var second = await client.PostAsync($"/api/loans/{id}/return", null);

        Assert.Equal(HttpStatusCode.NoContent, first.StatusCode);
        Assert.Equal(HttpStatusCode.Conflict, second.StatusCode);
    }

    [Fact]
    public async Task Delete_ActiveLoan_Returns409_ThenReturnedLoan_Returns204()
    {
        await using var factory = new WebApplicationFactory<global::Program>();
        using var client = factory.CreateClient();

        var loan = await ReadJson(await client.SendAsync(Checkout(LoanBody(), NewKey())));
        var id = loan.GetProperty("id").GetGuid();

        var deleteActive = await client.DeleteAsync($"/api/loans/{id}");
        await client.PostAsync($"/api/loans/{id}/return", null);
        var deleteReturned = await client.DeleteAsync($"/api/loans/{id}");

        Assert.Equal(HttpStatusCode.Conflict, deleteActive.StatusCode);
        Assert.Equal(HttpStatusCode.NoContent, deleteReturned.StatusCode);
    }
}