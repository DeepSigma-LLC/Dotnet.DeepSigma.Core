# DeepSigma.General

A broad .NET utility library that collects reusable building blocks for everyday application development, including extension methods, caching helpers, serialization utilities, cryptographic helpers, date/time utilities, and time-stepping components.

## Why this library exists

`DeepSigma.General` packages up common infrastructure and convenience helpers that tend to get rewritten across projects. Instead of scattering small utilities across multiple codebases, this library centralizes them into a single reusable package.

## What’s included

### Core language and collection helpers
- Numeric helpers such as `AbsoluteValue<T>`
- Collection abstractions such as `UniqueCollection` and `AbstractGenericUniqueCollection`
- Comparable/reference-type helpers
- Key chain and binding-list utilities

### Extension methods
The library includes a large set of extension methods for working with:
- `DateTime`, `DateOnly`, and `DayOfWeek`
- numeric types such as `decimal`, `double`, `int`, and generic `INumber`
- `byte[]`, `Guid`, `Dictionary`, `IEnumerable`, `Enum`, `Exception`, `Math`, `object`, and key/value pairs
- logging-related helpers

### Caching
- `LocalCache<TKey, TValue>` for simple in-memory caching with TTL-based expiration
- cache refresh support through delegate-based fetch methods
- bulk retrieval and cleanup helpers

### Serialization
- JSON serialization helpers
- binary serialization helpers
- deterministic serialization support

### Cryptography and hashing
- AES encryption/decryption helpers
- RSA key generation and encryption/decryption helpers
- ECDSA signing helpers
- hash utilities for MD5, SHA1, SHA256, SHA384, and SHA512

### Scheduling and time stepping
- `SelfAligningTimeStepper<T>` for generating aligned dates/times across daily, weekly, monthly, quarterly, semi-annual, annual, and intraday schedules
- configurable date-adjustment behavior for weekday/weekend/specific-day alignment
- `TimeSpanStepper` and related configuration types

### Additional utilities
- configuration helpers
- distributed-data and concurrency helpers
- encoding utilities
- inventory and logging helpers
- image, hashing, crypto, reflection, and periodicity utilities
- monadic helpers

## Target framework

This project currently targets:

- `.NET 10` (`net10.0`)

## Dependencies

The main library references:
- `Microsoft.Extensions.Configuration`
- `Microsoft.Extensions.Configuration.Binder`
- `Microsoft.Extensions.DependencyInjection`
- `Microsoft.Extensions.Logging`
- `Newtonsoft.Json`
- `OneOf`
- `Scrutor`

## Installation

### Option 1: project reference
If you are consuming this library from source, add a project reference:

```xml
<ItemGroup>
  <ProjectReference Include="..\DeepSigma.General\DeepSigma.General.csproj" />
</ItemGroup>
```

### Option 2: package reference
If you publish the generated package to a NuGet feed, consumers can add a package reference like this:

```xml
<ItemGroup>
  <PackageReference Include="DeepSigma.General" Version="3.0.0" />
</ItemGroup>
```
```
> Update the package name/version to match your published feed if they differ.

## Getting started

Import the namespaces you need:

```csharp
using DeepSigma.General;
using DeepSigma.General.Caching;
using DeepSigma.General.Extensions;
using DeepSigma.General.Serialization;
using DeepSigma.General.TimeStepper;
using DeepSigma.General.Utilities;
```

## Usage examples

### Absolute values with type safety

```csharp
using DeepSigma.General;

AbsoluteValue<int> exposure = -42;
Console.WriteLine(exposure.Value); // 42
```

### In-memory caching with TTL

```csharp
using DeepSigma.General.Caching;

var cache = new LocalCache<string, UserDto>(
    time_to_live: TimeSpan.FromMinutes(10),
    get_value_method: id => repository.GetUserById(id)
);

UserDto? user = cache.TryGetWithCacheRefresh("user-123");
```

### JSON serialization

```csharp
using DeepSigma.General.Serialization;

var payload = new { Id = 1, Name = "Alice" };
string json = JsonSerializer.GetSerializedString(payload);
var restored = JsonSerializer.GetDeserializedObject<dynamic>(json);
```

### Hashing

```csharp
using DeepSigma.General.Utilities;
using System.Security.Cryptography;

byte[] hash = HashUtilities.ComputeHash("test", HashAlgorithmName.SHA256);
```

### Date/time extensions

```csharp
using DeepSigma.General.Extensions;

DateTime today = DateTime.Today;
DateTime nextBusinessDay = today.NextWeekday();
string safeFileStamp = today.ToStringFileFormat();
```

### Self-aligning time steps

```csharp
using DeepSigma.General.Enums;
using DeepSigma.General.TimeStepper;

var periodicity = new PeriodicityConfiguration(
    Periodicity.Monthly,
    DaySelectionType.Weekday
);

var config = new TimeStepperConfiguration(
    periodicity,
    DateAdjustmentType.MoveBackward
);

var stepper = new SelfAligningTimeStepper<DateTimeCustom>(config);
var nextStep = stepper.GetNextTimeStep(DateTimeCustom.Parse("2024-03-15"));
```

## Testing

The repository includes a dedicated test project covering areas such as:
- binary serialization
- cryptographic helpers
- date/time helpers
- hash utilities
- self-aligning time-step generation
- extension methods and comparable/reference-type behavior

Run the test suite with:

```bash
dotnet test
```

## Repository structure

```text
DeepSigma.General/
├── Caching/
├── ChannelExamples/
├── Concurrency/
├── Configuration/
├── DateTimeUnification/
├── DistributedData/
├── Encode/
├── Enums/
├── Extensions/
├── Inventory/
├── Logging/
├── Monads/
├── Serialization/
├── TimeStepper/
├── Utilities/
└── DeepSigma.General.csproj

DeepSigma.General.Tests/
├── Models/
├── Tests/
└── DeepSigma.General.Tests.csproj
```

## When to use this project

This library is a good fit when you want:
- one shared package for cross-project helpers
- reusable date/time alignment and scheduling logic
- lightweight in-memory caching primitives
- convenience wrappers for hashing, crypto, and serialization
- a broad set of practical extension methods for application code

## License

This repository is licensed under the MIT License.


