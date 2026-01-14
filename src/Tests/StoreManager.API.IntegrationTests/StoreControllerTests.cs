using Xunit.Abstractions;
using System.Net;

namespace StoreManager.API.IntegrationTests;

public class StoreControllerTests : BaseIntegrationTest
{
    private readonly HttpClient _client;
    private readonly ITestOutputHelper _output;

    public StoreControllerTests(StoreManagerWebApplicationFactory factory, ITestOutputHelper output) : base(factory)
    {
        _client = factory.CreateClient();
        _output = output;
    }
}
