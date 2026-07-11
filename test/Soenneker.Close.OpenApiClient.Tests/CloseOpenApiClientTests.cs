using Soenneker.Tests.HostedUnit;

namespace Soenneker.Close.OpenApiClient.Tests;

[ClassDataSource<Host>(Shared = SharedType.PerTestSession)]
public sealed class CloseOpenApiClientTests : HostedUnitTest
{
    public CloseOpenApiClientTests(Host host) : base(host)
    {
    }

    [Test]
    public void Default()
    {

    }
}
