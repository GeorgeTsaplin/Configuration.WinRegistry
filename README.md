# Configuration.WinRegistry
Windows Registry configuration provider implementation for Microsoft.Extensions.Configuration.

## Usage

```csharp
var configBuilder = new ConfigurationBuilder();

var config = configBuilder.AddRegistrySection(() => Microsoft.Win32.Registry.LocalMachine, "{windows registry section path}")
    .Build();
```