using System.Net;
using Xunit.Abstractions;

namespace StoreManager.API.IntegrationTests;

public class ChainControllerTests : BaseIntegrationTest
{
    private readonly HttpClient _client;
    private readonly ITestOutputHelper _output;

    public ChainControllerTests(StoreManagerWebApplicationFactory factory, ITestOutputHelper output) : base(factory)
    {
        _client = factory.CreateClient();
        _output = output;
    }
}
