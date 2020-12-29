# [Thum.io](https://www.thum.io/) .NET Package (Unofficial)

- [Thum.io .NET Package (Unofficial)](#thumio-net-package-unofficial)
  - [Setup](#setup)
  - [Use the CLI](#use-the-cli)
  - [How in Your Own Code](#how-in-your-own-code)
  - [CHANGELOG](#changelog)

## Setup

1. [Signup](https://www.thum.io/signup)

2. Create an [API](https://www.thum.io/admin/keys) Key

3. Get the API Key Id and URL Key

4. Read the [Docs](https://www.thum.io/documentation/api/url) for parameters

5. Use the CLI

## Use the CLI

1. Set your API key in the CLI by running without any arguments:

```
dotnet run -p Thum.io.CLI
```

- Take a screenshot of http://google.com and save it to disk:

```powershell
dotnet run -p Thum.io.CLI -- screenshot "http://google.com" .\google.png
```

- Get help

```powershell
dotnet run -p Thum.io.CLI -- -h
```

## How in Your Own Code

The package is on the [NuGet Gallery](https://www.nuget.org/packages/Thum.io.Screenshots/)

1. Install the Thum.io NuGet package

```powershell
Install-Package Thum.io
```

```powershell
dotnet add package Thum.io
```

2. Update your ```appsettings.json``` to set the ApiKey

```json
{
    "ScreenShotService": {
        "ApiKey": "{Id}-{Url Key}"
    }
}
```

3. Use the ```AddThumIoScreenshots``` extension to register the required services,
   passing it your built configuration object.

```csharp
// Build the configuration if you need to
var config = new ConfigurationBuilder()
    .SetBasePath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))
    .AddJsonFile("appsettings.json")
    .AddEnvironmentVariables()
    .AddCommandLine(args)
    .Build();

// Register the required services
services.AddThumIoScreenshots(config);
```

4. Inject the IScreenShotService and use it

```csharp
public class App
{
    private readonly IScreenShotService _screenShotService;

    public App(IScreenShotService screenShotService)
    {
        _screenShotService = screenShotService ?? throw new ArgumentNullException(nameof(screenShotService));
    }

    public async Task TakeScreenshot(string url, string path, ImageModifierOptions options = null)
    {
        if (!string.IsNullOrEmpty(url) && !string.IsNullOrEmpty(path))
        {
            await _screenShotService.ToDisk(url, path, options);
        }

        return Task.CompletedTask;
    }
}
```

## [CHANGELOG](CHANGELOG.md)