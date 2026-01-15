# Changelog

All notable changes to this project are documented in this file.

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
