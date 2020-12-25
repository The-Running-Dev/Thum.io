# [Thum.io.Screenshots](https://www.thum.io/)

- [Thum.io.Screenshots](#thumioscreenshots)
  - [CHANGELOG](#changelog)
  - [Setup](#setup)
  - [Take a Screenshot from the Command Line](#take-a-screenshot-from-the-command-line)
  - [How to Use from Your Own Project](#how-to-use-from-your-own-project)

## [CHANGELOG](CHANGELOG.md)

## Setup

1. [Signup](https://www.thum.io/signup)

2. Create an [API](https://www.thum.io/admin/keys) Key

3. Get the API Key Id and URL Key

4. Read the [Docs](https://www.thum.io/documentation/api/url) for parameters

## Take a Screenshot from the Command Line

1. Update the ```appsettings.json``` to set the ApiKey in the Console Application

```json
{
    "ScreenshotService": {
        "ApiKey": "{Id}-{Url Key}"
    }
}
```

- Take a screenshot of http://google.com and save it to disk:

```powershell
dotnet run -p Thum.io.Screenshots.Console -- screenshot "http://google.com" .\google.png
```

## How to Use from Your Own Project

The package is on the [NuGet Gallery](https://www.nuget.org/packages/Thum.io.Screenshots/)

1. Install the Thum.io.Screenshots NuGet package

```powershell
Install-Package Thum.io.Screenshots
```

```powershell
dotnet add package Thum.io.Screenshots
```

2. Update your ```appsettings.json``` to set the ApiKey

```json
{
    "ScreenshotService": {
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

4. Inject the IScreenshotService and use it

```csharp
public class App
{
    private readonly IScreenshotService _screenshotService;

    public App(IScreenshotService screenshotService)
    {
        _screenshotService = screenshotService ?? throw new ArgumentNullException(nameof(screenshotService));
    }

    public async Task TakeScreenshot(string url, string path, ImageModifierOptions options = null)
    {
        if (!string.IsNullOrEmpty(url) && !string.IsNullOrEmpty(path))
        {
            await _screenshotService.ToDisk(url, path, options);
        }

        return Task.CompletedTask;
    }
}
```