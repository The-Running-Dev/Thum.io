# [Thum.io.Screenshots](https://www.thum.io/)

## Setup

1. [Signup](https://www.thum.io/signup)

2. Create an [API](https://www.thum.io/admin/keys) Key

3. Copy the API Key Id and URL Key

4. Update ```appsettings.json``` in the Console application

```json
{
    "ScreenshotService": {
        "Url": "https://image.thum.io",
        "ApiKey": "{Id}-{Url Key}",
        "Parameters": "width/1200/crop/600/noanimate"
    }
}
```

5. Read the [Docs](https://www.thum.io/documentation/api/url) for additional parameters

## Console Application

- Take a screenshot of Google and save it to disk:

```powershell
dotnet run -p Thum.io.Screenshots.Console -- screenshot "http://google.com" .\google.png
```