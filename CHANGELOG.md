# Changelog

All notable changes to this project are documented in this file.

## [3.1.0] - 2026-01-15

### Added

- **ElseDo/ElseDoAsync Methods**: Execute side effects on error state without transforming the result
  - `ElseDo(Action<List<Error>>)` - execute action when in error state, return original ErrorOr
  - `ElseDoAsync(Func<List<Error>, Task>)` - async variant
  - Extensions for `Task<ErrorOr<T>>` included
  - Based on [amantinband/error-or#117](https://github.com/amantinband/error-or/pull/117), addresses [#139](https://github.com/amantinband/error-or/issues/139)

```csharp
errorOr
    .ThenDo(val => Console.WriteLine("Success!"))
    .ElseDo(errors => Console.WriteLine($"Failed with {errors.Count} errors"));
```

- **ToErrorOrAsync Extension**: Convert async results directly to `Task<ErrorOr<T>>`
  - Based on [amantinband/error-or#122](https://github.com/amantinband/error-or/pull/122)

```csharp
return await repository.GetByIdAsync(id)
    .ToErrorOrAsync()
    .FailIf(x => x is null, Errors.NotFound);
```

- **Error Aggregation Methods**: Combine multiple ErrorOr instances or append errors
  - `Combine<T>(params ErrorOr<T>[])` - returns first value or all errors combined
  - `CombineAll<T>(params ErrorOr<T>[])` - returns all values as list or all errors combined
  - `AppendErrors(params Error[])` - append additional errors to an ErrorOr
  - Based on [amantinband/error-or#125](https://github.com/amantinband/error-or/pull/125), addresses [#120](https://github.com/amantinband/error-or/issues/120)

```csharp
var combined = ErrorOrExtensions.Combine(result1, result2, result3);
var withMoreErrors = result.AppendErrors(Error.Validation("Additional error"));
```

- **ValueObject Property on IErrorOr Interface**: Access value without knowing generic type
  - Enables logging/serialization scenarios where type is unknown
  - Based on [amantinband/error-or#136](https://github.com/amantinband/error-or/pull/136), addresses [#121](https://github.com/amantinband/error-or/issues/121)

```csharp
IErrorOr result = GetSomeResult();
if (!result.IsError)
    logger.Log(result.ValueObject);
```

- **Collection Expression Support**: Use C# 12 collection expressions to create ErrorOr from errors
  - Based on [amantinband/error-or#133](https://github.com/amantinband/error-or/pull/133)

```csharp
ErrorOr<int> result = [Error.Validation(), Error.NotFound()];
```

- **Additional Factory Methods**: Create ErrorOr from single error or error collections
  - `ErrorOrFactory.From<T>(Error)` - from single error
  - `ErrorOrFactory.From<T>(List<Error>)` - from error list
  - `ErrorOrFactory.From<T>(Error[])` - from error array
  - `ErrorOrFactory.FromAsync<T>(...)` - async variants
  - Based on [amantinband/error-or#129](https://github.com/amantinband/error-or/pull/129), [#134](https://github.com/amantinband/error-or/pull/134)

```csharp
ErrorOr<int> result = ErrorOrFactory.From<int>(Error.Unexpected());
ErrorOr<int> multiple = ErrorOrFactory.From<int>([error1, error2]);
```

### Changed

- **Null Check Optimization**: Removed redundant null check in error collection constructor
  - Based on [amantinband/error-or#128](https://github.com/amantinband/error-or/pull/128), [#135](https://github.com/amantinband/error-or/pull/135)

### Fixed

- **README Documentation**: Fixed custom error type example to use correct accessor visibility
  - Based on [amantinband/error-or#118](https://github.com/amantinband/error-or/pull/118)

---

## [3.0.0] - 2026-01-12

### Breaking Changes

- **Package Rename**: Package has been renamed from `ErrorOr` to `TylerSoftware.ErrorOr`
  - Update your package reference: `dotnet add package TylerSoftware.ErrorOr`
  - No namespace changes - still use `using ErrorOr;`

### Added

- **.NET 9.0 Support**: Added `net9.0` target framework with C# 13 features
- **.NET 10.0 Support**: Added `net10.0` target framework with C# 14 features
- **AOT Compatibility**: Full Native AOT compilation support for `net8.0+` targets
- **Trimming Support**: Library is now fully trimming compatible for `net8.0+` targets
- **Source Link**: Integrated Source Link for improved debugging experience
- **Symbol Packages**: `.snupkg` symbol packages now published to NuGet.org
- **Deterministic Builds**: Enabled deterministic builds for reproducibility

### Changed

- Updated CI/CD workflows to test against .NET 8, 9, and 10
- Updated test project to multi-target .NET 8, 9, and 10
- Updated GitHub Actions to v4
- Modernized project file structure with better organization

### Infrastructure

- Added `IsAotCompatible` property for AOT analyzer support
- Added `Microsoft.SourceLink.GitHub` for source debugging
- Enabled `ContinuousIntegrationBuild` for CI environments
- Updated test dependencies to latest versions

---

## [2.0.0] - 2024-03-26

### Added

- `FailIf`

```csharp
public ErrorOr<TValue> FailIf(Func<TValue, bool> onValue, Error error)
```

```csharp
ErrorOr<int> errorOr = 1;
errorOr.FailIf(x => x > 0, Error.Failure());
```

### Breaking Changes

- `Then` that receives an action is now called `ThenDo`

```diff
-public ErrorOr<TValue> Then(Action<TValue> action)
+public ErrorOr<TValue> ThenDo(Action<TValue> action)
```

```diff
-public static async Task<ErrorOr<TValue>> Then<TValue>(this Task<ErrorOr<TValue>> errorOr, Action<TValue> action)
+public static async Task<ErrorOr<TValue>> ThenDo<TValue>(this Task<ErrorOr<TValue>> errorOr, Action<TValue> action)
```

- `ThenAsync` that receives an action is now called `ThenDoAsync`

```diff
-public async Task<ErrorOr<TValue>> ThenAsync(Func<TValue, Task> action)
+public async Task<ErrorOr<TValue>> ThenDoAsync(Func<TValue, Task> action)
```

```diff
-public static async Task<ErrorOr<TValue>> ThenAsync<TValue>(this Task<ErrorOr<TValue>> errorOr, Func<TValue, Task> action)
+public static async Task<ErrorOr<TValue>> ThenDoAsync<TValue>(this Task<ErrorOr<TValue>> errorOr, Func<TValue, Task> action)
```

## [1.10.0] - 2024-02-14

### Added

- `ErrorType.Forbidden`
- README to NuGet package

## [1.9.0] - 2024-01-06

### Added

- `ToErrorOr`
