# Development Guide

This guide covers setting up your development environment for the Obstor .NET SDK.

## Prerequisites: Installing the .NET SDK

The SDK targets `net6.0` through `net10.0`. Install a recent .NET SDK (8.0 or later recommended).

### Windows

Download and run the installer from [dotnet.microsoft.com/download](https://dotnet.microsoft.com/download), or use winget:

```bash
winget install Microsoft.DotNet.SDK.10
```

### macOS

```bash
brew install dotnet
```

### Linux (Debian/Ubuntu)

```bash
sudo apt-get update && sudo apt-get install -y dotnet-sdk-10.0
```

For other distributions see the [official install docs](https://learn.microsoft.com/dotnet/core/install/linux).

### Verify the installation

```bash
dotnet --version
```

---

## IDE Setup

### Visual Studio Code

1. Install the [C# Dev Kit](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csdevkit) extension (includes OmniSharp/Roslyn language server, test explorer, and debugger).
2. Open the repository root folder — VS Code will detect `obstor-dotnet.sln` automatically.
3. When prompted, select the solution file to activate IntelliSense across all projects.

The repository already contains the settings to debug the example projects using VSCode.

### JetBrains Rider

1. Open Rider and choose **Open** → select `obstor-dotnet.sln`.
2. Rider will restore NuGet packages and index the solution automatically.
3. The built-in test runner (under **View → Tool Windows → Unit Tests**) discovers xUnit tests without additional configuration.
4. To run an example project, right-click it in the **Solution Explorer** and choose **Run** or **Debug**.

No plugins are required. Rider ships with first-class .NET support out of the box. Note that JetBrains Rider also provides a much better interface to debug individual or a set of tests.

---

## Example Projects

Both examples connect to a local Obstor server. Start one with Docker before running them:

```bash
docker run --rm -p 9000:9000 ghcr.io/obstor/obstor:latest server /data
```

The default credentials used in the examples are `obstoradmin` / `obstoradmin`.

### Obstor.Examples.Simple

**Path:** `Obstor.Examples.Simple/Program.cs`

A minimal console application that uses `ObstorClientBuilder` directly (no dependency injection). It demonstrates:

- Creating a client with static credentials
- Checking whether a bucket exists and creating it if not

Run it:

```bash
dotnet run --project Obstor.Examples.Simple
```

### Obstor.Examples.Host

**Path:** `Obstor.Examples.Host/Program.cs`

A console application that uses `Microsoft.Extensions.Hosting` and the `AddObstor` DI extension. It demonstrates:

- Registering the Obstor client in an `IHost` service container
- Structured logging via `Microsoft.Extensions.Logging`
- Creating a bucket and writing 100 objects in parallel
- Reading an object back as a stream
- Listing objects with a prefix and page size
- Subscribing to real-time bucket notifications via `IObservable<T>`

Run it:

```bash
dotnet run --project Obstor.Examples.Host
```

To target a specific .NET version (the projects multi-target `net6.0`–`net10.0`):

```bash
dotnet run --project Obstor.Examples.Host --framework net8.0
```
