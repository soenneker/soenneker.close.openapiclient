[![](https://img.shields.io/nuget/v/soenneker.close.openapiclient.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.close.openapiclient/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.close.openapiclient/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.close.openapiclient/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/soenneker.close.openapiclient.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.close.openapiclient/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.close.openapiclient/codeql.yml?label=CodeQL&style=for-the-badge)](https://github.com/soenneker/soenneker.close.openapiclient/actions/workflows/codeql.yml)

# Soenneker.Close.OpenApiClient

A Kiota-generated .NET client for the Close CRM API.

## Installation

```bash
dotnet add package Soenneker.Close.OpenApiClient
```

## Recommended setup

For dependency injection, authentication configuration, and client reuse, install the companion utility:

```bash
dotnet add package Soenneker.Close.OpenApiClientUtil
```

```csharp
using Microsoft.Extensions.DependencyInjection;
using Soenneker.Close.OpenApiClientUtil.Registrars;

services.AddCloseOpenApiClientUtilAsSingleton();
```

Configure the Close API key under `Close:ApiKey`, then inject `ICloseOpenApiClientUtil` and call `Get` to obtain the generated client.

## Direct construction

Close API keys use HTTP Basic authentication with the key as the username and an empty password:

```csharp
using System.Net.Http.Headers;
using System.Text;
using Microsoft.Kiota.Abstractions.Authentication;
using Microsoft.Kiota.Http.HttpClientLibrary;
using Soenneker.Close.OpenApiClient;
using Soenneker.Close.OpenApiClient.Me;

string credentials = Convert.ToBase64String(
    Encoding.UTF8.GetBytes($"{apiKey}:"));

var httpClient = new HttpClient();
httpClient.DefaultRequestHeaders.Authorization =
    new AuthenticationHeaderValue("Basic", credentials);

var authentication = new AnonymousAuthenticationProvider();
var adapter = new HttpClientRequestAdapter(
    authentication,
    httpClient: httpClient);

var close = new CloseOpenApiClient(adapter);

MeGetResponse? currentUser = await close.Me.GetAsync(
    request =>
    {
        request.QueryParameters.Fields = "id,name,organization_id";
    },
    cancellationToken);
```

`AnonymousAuthenticationProvider` is used because the dedicated `HttpClient` already carries the authorization header. Do not put Close credentials on an HTTP client that can send requests to unrelated hosts. See [Close's API-key authentication documentation](https://developer.close.com/api/overview/api-key-authentication).

## Navigating the API

The generated root client mirrors Close's API paths. Common entry points include:

- `close.Me` for the authenticated user.
- `close.Lead`, `close.Contact`, and `close.Opportunity` for CRM records.
- `close.Activity`, `close.Task`, and `close.Event` for work and activity data.
- `close.Sequence`, `close.Email_template`, and `close.Sms_template` for outreach automation.
- `close.Webhook` for webhook operations.

Names follow the OpenAPI description, including singular and underscore-separated request-builder properties.

## Practical notes

- Keep the `HttpClient`, request adapter, and generated client long-lived. The companion utility manages that lifecycle in dependency-injection applications.
- Endpoint methods accept a request-configuration callback and cancellation token.
- Generated return values may be nullable when the schema permits an empty response. Check results before dereferencing them.
- Some endpoints return streams or other disposable content; dispose those results after use.
- Service failures are surfaced through generated error models or Kiota exceptions according to each endpoint's schema mapping.
- Public request builders and models can change when the source OpenAPI description changes.
- Files under `src/Soenneker.Close.OpenApiClient` are generated. Keep application behavior and custom models in a separate project.
- Never log or commit Close API keys, OAuth tokens, or authorization headers.
