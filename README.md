# Obstor .NET SDK

A modern C# client library for [Obstor](https://obstor.net) and S3-compatible object storage services.

## Requirements

- .NET 8.0, 9.0, or 10.0
- A Obstor or S3-compatible server

## Quick Start

### Direct usage

```csharp
var client = new ObstorClientBuilder("https://obstor.example.com")
    .WithStaticCredentials("accessKey", "secretKey")
    .Build();
```

### Dependency Injection

```csharp
services
    .AddObstor("http://localhost:9000")
    .WithStaticCredentials("obstoradmin", "obstoradmin");
```

## Examples

Two example projects are included:

- `Obstor.Examples.Simple/` — minimal console app using the direct builder
- `Obstor.Examples.Host/` — DI-based example using `IHost`, demonstrating bucket creation, object upload/download, listing, and bucket notifications

To run an example, start a local Obstor instance first:

```bash
docker run --rm -p 9000:9000 ghcr.io/obstor/obstor:latest server /data
```

Then:

```bash
dotnet run --project Obstor.Examples.Simple
# or
dotnet run --project Obstor.Examples.Host
```
