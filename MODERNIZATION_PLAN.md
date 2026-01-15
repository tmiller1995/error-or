# TylerSoftware.ErrorOr Modernization Plan

## Executive Summary

This document outlines a comprehensive plan to modernize the ErrorOr discriminated union library for .NET 10, rename it to **TylerSoftware.ErrorOr**, and align with Microsoft's official library guidance and best practices.

---

## Current State Analysis

### Project Overview
- **Current Package Name:** ErrorOr
- **Current Version:** 2.0.1
- **Target Frameworks:** `netstandard2.0`, `net8.0`
- **Core Type:** `ErrorOr<TValue>` - A readonly record struct implementing discriminated union pattern
- **Test Coverage:** Comprehensive (17 test files, ~1,800+ lines)

### Current Architecture
- `ErrorOr<TValue>` - Readonly partial record struct
- `Error` - Readonly record struct for error representation
- `ErrorType` - Enum with 7 error types (Failure, Unexpected, Validation, Conflict, NotFound, Unauthorized, Forbidden)
- Result sentinel types (Success, Created, Updated, Deleted)
- Extensive fluent API (Match, Switch, Then, FailIf, Else)

### Dependencies
- `Microsoft.Bcl.HashCode` 1.1.1 (netstandard2.0 only)
- `Nullable` 1.3.1 (for nullable reference types on older frameworks)

---

## Phase 1: Package Rebranding

### 1.1 Update Package Identity

**File:** `src/ErrorOr.csproj`

| Property | Current Value | New Value |
|----------|---------------|-----------|
| `PackageId` | `ErrorOr` | `TylerSoftware.ErrorOr` |
| `Authors` | `Amichai Mantinband` | `Tyler Software` |
| `PackageProjectUrl` | `https://github.com/amantinband/error-or` | Update to your repository URL |
| `RepositoryUrl` | `https://github.com/amantinband/error-or` | Update to your repository URL |
| `Version` | `2.0.1` | `3.0.0` (major version bump for rebrand) |

### 1.2 Update Namespace (Optional but Recommended)

Consider changing the root namespace from `ErrorOr` to `TylerSoftware.ErrorOr` for consistency with the package name. This is a **breaking change** that requires a major version bump.

**Migration Path for Users:**
```csharp
// Before
using ErrorOr;

// After
using TylerSoftware.ErrorOr;
```

### 1.3 Update Documentation
- Update `README.md` with new package name and installation instructions
- Update `CHANGELOG.md` with rebrand information
- Update any internal documentation references

---

## Phase 2: Target Framework Modernization

### 2.1 Recommended Target Framework Strategy

Based on Microsoft's official library guidance, update target frameworks:

```xml
<TargetFrameworks>netstandard2.0;net8.0;net9.0;net10.0</TargetFrameworks>
```

**Rationale:**
- `netstandard2.0` - Maximum compatibility (.NET Framework 4.6.1+, Mono, Xamarin, Unity)
- `net8.0` - LTS release with widespread adoption
- `net9.0` - Current stable with C# 13 features
- `net10.0` - Next LTS release with C# 14 features

### 2.2 Conditional Compilation for Framework-Specific Features

Enable modern .NET features while maintaining backward compatibility:

```xml
<PropertyGroup Condition="$([MSBuild]::IsTargetFrameworkCompatible('$(TargetFramework)', 'net8.0'))">
  <IsAotCompatible>true</IsAotCompatible>
</PropertyGroup>
```

---

## Phase 3: AOT and Trimming Compatibility

### 3.1 Enable Trimming Analysis

Add to project file:
```xml
<PropertyGroup>
  <IsTrimmable Condition="$([MSBuild]::IsTargetFrameworkCompatible('$(TargetFramework)', 'net8.0'))">true</IsTrimmable>
</PropertyGroup>
```

### 3.2 Enable AOT Compatibility

```xml
<PropertyGroup Condition="$([MSBuild]::IsTargetFrameworkCompatible('$(TargetFramework)', 'net8.0'))">
  <IsAotCompatible>true</IsAotCompatible>
</PropertyGroup>
```

**Benefits:**
- `IsAotCompatible` automatically enables:
  - `IsTrimmable`
  - `EnableTrimAnalyzer`
  - `EnableSingleFileAnalyzer`
  - `EnableAotAnalyzer`

### 3.3 Review Code for Trim/AOT Warnings

The current implementation uses:
- Readonly record structs (AOT-friendly)
- No reflection (AOT-friendly)
- No dynamic code generation (AOT-friendly)

**Expected:** Minimal to no changes required for AOT compatibility.

---

## Phase 4: Source Link and Debugging Experience

### 4.1 Enable Source Link

Add to project file:
```xml
<PropertyGroup>
  <PublishRepositoryUrl>true</PublishRepositoryUrl>
  <EmbedUntrackedSources>true</EmbedUntrackedSources>
  <IncludeSymbols>true</IncludeSymbols>
  <SymbolPackageFormat>snupkg</SymbolPackageFormat>
</PropertyGroup>

<ItemGroup>
  <PackageReference Include="Microsoft.SourceLink.GitHub" Version="8.0.0" PrivateAssets="All"/>
</ItemGroup>
```

### 4.2 Enable Deterministic Builds

```xml
<PropertyGroup>
  <Deterministic>true</Deterministic>
  <ContinuousIntegrationBuild Condition="'$(GITHUB_ACTIONS)' == 'true'">true</ContinuousIntegrationBuild>
</PropertyGroup>
```

### 4.3 Debugger Attributes

Consider adding debugger display attributes for better debugging experience:

```csharp
[DebuggerDisplay("{IsError ? \"Error: \" + FirstError.Code : \"Value: \" + Value}")]
public readonly partial record struct ErrorOr<TValue>
```

---

## Phase 5: NuGet Package Improvements

### 5.1 Enhanced Package Metadata

```xml
<PropertyGroup>
  <!-- Identity -->
  <PackageId>TylerSoftware.ErrorOr</PackageId>
  <Version>3.0.0</Version>
  <Authors>Tyler Software</Authors>
  <Company>Tyler Software</Company>

  <!-- Description -->
  <Description>A simple, fluent discriminated union of an error or a result. Supports .NET Standard 2.0, .NET 8, .NET 9, and .NET 10 with full AOT/trimming compatibility.</Description>
  <PackageTags>Result;Results;ErrorOr;Error;Handling;DiscriminatedUnion;Railway;Functional;Monad</PackageTags>

  <!-- Legal -->
  <PackageLicenseExpression>MIT</PackageLicenseExpression>
  <Copyright>Copyright (c) Tyler Software 2024-2026</Copyright>

  <!-- Repository -->
  <RepositoryType>git</RepositoryType>
  <RepositoryUrl>https://github.com/tmiller1995/error-or</RepositoryUrl>
  <PackageProjectUrl>https://github.com/tmiller1995/error-or</PackageProjectUrl>

  <!-- Assets -->
  <PackageIcon>icon-square.png</PackageIcon>
  <PackageReadmeFile>README.md</PackageReadmeFile>

  <!-- Release Notes -->
  <PackageReleaseNotes>See CHANGELOG.md for release notes.</PackageReleaseNotes>
</PropertyGroup>
```

### 5.2 Symbol Package Publishing

Update CI/CD to publish symbol packages:
```yaml
- name: Package
  run: dotnet pack -c Release src/ErrorOr.csproj

- name: Publish NuGet
  run: dotnet nuget push .\artifacts\*.nupkg --api-key ${{ secrets.NUGET_API_KEY }} --source https://api.nuget.org/v3/index.json --skip-duplicate

- name: Publish Symbols
  run: dotnet nuget push .\artifacts\*.snupkg --api-key ${{ secrets.NUGET_API_KEY }} --source https://api.nuget.org/v3/index.json --skip-duplicate
```

---

## Phase 6: C# 13/14 Language Feature Adoption

### 6.1 C# 13 Features (net9.0+)

**`ref struct` Interface Implementation:**
Not directly applicable to `ErrorOr<TValue>` (not a ref struct), but enables better `Span<T>` interop.

**`allows ref struct` Generic Constraint:**
Consider adding overloads that can accept `Span<T>` or `ReadOnlySpan<T>` for value processing.

### 6.2 C# 14 Features (net10.0+)

**Extension Members (Static Extension Methods):**
Could simplify extension method organization.

**`field` Backed Properties:**
Not applicable (using record struct with auto-properties).

**Null-conditional Assignment:**
Could simplify some internal code paths.

### 6.3 Conditional Language Features

Use conditional compilation for framework-specific implementations:

```csharp
#if NET9_0_OR_GREATER
// C# 13+ specific implementations
#endif

#if NET10_0_OR_GREATER
// C# 14+ specific implementations
#endif
```

---

## Phase 7: Performance Optimizations

### 7.1 .NET 10 JIT Improvements Leveraged Automatically

The following JIT improvements in .NET 10 will automatically benefit `ErrorOr<TValue>`:

- **Improved struct argument code generation** - Better register utilization for `ErrorOr<TValue>` parameters
- **Array interface devirtualization** - Faster `List<Error>` operations
- **Enhanced loop inversion** - Better performance in error collection processing
- **Stack allocation improvements** - More efficient memory usage for closures

### 7.2 Code-Level Optimizations

**Consider `CollectionsMarshal` for .NET 8+:**
```csharp
#if NET8_0_OR_GREATER
// Use CollectionsMarshal.AsSpan for List<Error> when appropriate
#endif
```

**Consider `FrozenDictionary` for metadata:**
```csharp
#if NET8_0_OR_GREATER
// Use FrozenDictionary for immutable error metadata
#endif
```

### 7.3 Benchmarking

Add a benchmarks project:
```
benchmarks/
  ErrorOr.Benchmarks.csproj
  InstantiationBenchmarks.cs
  ChainOperationsBenchmarks.cs
  MatchBenchmarks.cs
```

---

## Phase 8: CI/CD Modernization

### 8.1 Updated Build Workflow

```yaml
name: Build

on:
  workflow_dispatch:
  push:
    branches: [main]
  pull_request:
    branches: [main]

jobs:
  build:
    runs-on: ubuntu-latest
    strategy:
      matrix:
        dotnet-version: ['8.0.x', '9.0.x', '10.0.x']

    steps:
      - uses: actions/checkout@v4
        with:
          fetch-depth: 0  # Required for Source Link

      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: ${{ matrix.dotnet-version }}

      - name: Restore dependencies
        run: dotnet restore

      - name: Build
        run: dotnet build -c Release --no-restore

      - name: Test
        run: dotnet test -c Release --no-restore --verbosity normal --collect:"XPlat Code Coverage"

      - name: Upload coverage to Codecov
        uses: codecov/codecov-action@v4
        with:
          token: ${{ secrets.CODE_COV_TOKEN }}
```

### 8.2 Updated Publish Workflow

```yaml
name: Publish TylerSoftware.ErrorOr to NuGet

on:
  workflow_dispatch:
  push:
    tags:
      - 'v*'

jobs:
  publish:
    runs-on: ubuntu-latest

    steps:
      - uses: actions/checkout@v4
        with:
          fetch-depth: 0

      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '10.0.x'

      - name: Build
        run: dotnet build -c Release

      - name: Test
        run: dotnet test -c Release --no-build

      - name: Package
        run: dotnet pack -c Release --no-build src/ErrorOr.csproj

      - name: Publish NuGet Package
        run: |
          dotnet nuget push ./artifacts/*.nupkg --api-key ${{ secrets.NUGET_API_KEY }} --source https://api.nuget.org/v3/index.json --skip-duplicate
          dotnet nuget push ./artifacts/*.snupkg --api-key ${{ secrets.NUGET_API_KEY }} --source https://api.nuget.org/v3/index.json --skip-duplicate
```

### 8.3 Add AOT Testing Workflow

```yaml
name: AOT Compatibility

on:
  push:
    branches: [main]
  pull_request:
    branches: [main]

jobs:
  aot-test:
    runs-on: ubuntu-latest

    steps:
      - uses: actions/checkout@v4

      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '10.0.x'

      - name: Create AOT test project
        run: |
          dotnet new console -n AotTest -o tests/AotTest
          cd tests/AotTest
          dotnet add reference ../../src/ErrorOr.csproj

      - name: Publish AOT
        run: dotnet publish tests/AotTest -c Release -r linux-x64 --self-contained -p:PublishAot=true
```

---

## Phase 9: Test Project Updates

### 9.1 Multi-Target Test Project

```xml
<PropertyGroup>
  <TargetFrameworks>net8.0;net9.0;net10.0</TargetFrameworks>
</PropertyGroup>
```

### 9.2 Update Test Dependencies

```xml
<ItemGroup>
  <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.12.0" />
  <PackageReference Include="xunit" Version="2.9.0" />
  <PackageReference Include="xunit.runner.visualstudio" Version="2.9.0" />
  <PackageReference Include="FluentAssertions" Version="7.0.0" />
  <PackageReference Include="coverlet.collector" Version="6.0.2" />
</ItemGroup>
```

### 9.3 Add AOT/Trimming Tests

Create dedicated tests to verify AOT and trimming compatibility:
```csharp
public class AotCompatibilityTests
{
    [Fact]
    public void ErrorOr_ShouldWorkWithoutReflection()
    {
        // Test all public APIs without reflection
    }
}
```

---

## Phase 10: Documentation Updates

### 10.1 README.md Updates

- Update installation instructions for new package name
- Add .NET version compatibility matrix
- Add AOT/trimming compatibility section
- Update contributing guidelines
- Add migration guide from `ErrorOr` to `TylerSoftware.ErrorOr`

### 10.2 Create MIGRATION.md

```markdown
# Migration Guide

## From ErrorOr 2.x to TylerSoftware.ErrorOr 3.x

### Package Reference
```diff
- <PackageReference Include="ErrorOr" Version="2.0.1" />
+ <PackageReference Include="TylerSoftware.ErrorOr" Version="3.0.0" />
```

### Namespace (if changed)
```diff
- using ErrorOr;
+ using TylerSoftware.ErrorOr;
```
```

### 10.3 Update CHANGELOG.md

Add comprehensive changelog entry for v3.0.0 covering:
- Package rename
- New target frameworks
- AOT/trimming support
- Performance improvements
- Breaking changes (if any)

---

## Implementation Checklist

### Phase 1: Package Rebranding ✅
- [x] Update `PackageId` to `TylerSoftware.ErrorOr`
- [x] Update `Authors` and `Company`
- [x] Update repository URLs
- [x] Update `Version` to `3.0.0`
- [x] (Optional) Update namespace to `TylerSoftware.ErrorOr` — **Skipped**: Kept as `ErrorOr` for easier migration

### Phase 2: Target Frameworks ✅
- [x] Add `net9.0` target
- [x] Add `net10.0` target
- [x] Test against all target frameworks

### Phase 3: AOT/Trimming ✅
- [x] Add `IsTrimmable` property (automatic via `IsAotCompatible`)
- [x] Add `IsAotCompatible` property
- [x] Run trim analyzers and fix warnings
- [x] Run AOT analyzers and fix warnings

### Phase 4: Source Link ✅
- [x] Add `Microsoft.SourceLink.GitHub` package
- [x] Enable deterministic builds
- [x] Enable symbol package generation
- [x] Add debugger display attributes

### Phase 5: NuGet Improvements ✅
- [x] Update package metadata
- [x] Add `PackageLicenseExpression`
- [x] Add `Copyright`
- [x] Add additional `PackageTags`

### Phase 6: Language Features (Not Started - Low Priority)
- [ ] Review C# 13 features for net9.0
- [ ] Review C# 14 features for net10.0
- [ ] Add conditional compilation where beneficial

### Phase 7: Performance (Not Started - Low Priority)
- [ ] Create benchmarks project
- [ ] Establish baseline benchmarks
- [ ] Apply framework-specific optimizations

### Phase 8: CI/CD ✅
- [x] Update GitHub Actions to v4
- [x] Add multi-framework matrix testing
- [x] Add AOT compatibility testing workflow
- [x] Update publish workflow for symbols
- [x] **Added**: NuGet Trusted Publishing (OIDC-based authentication)

### Phase 9: Tests ✅
- [x] Multi-target test project
- [x] Update test dependencies
- [x] Add AOT compatibility tests

### Phase 10: Documentation ✅
- [x] Update README.md
- [x] Create MIGRATION.md
- [x] Update CHANGELOG.md

---

## Complete Updated Project File

Here is the complete recommended `ErrorOr.csproj`:

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <!-- Target Frameworks -->
  <PropertyGroup>
    <TargetFrameworks>netstandard2.0;net8.0;net9.0;net10.0</TargetFrameworks>
    <LangVersion>latest</LangVersion>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <GenerateDocumentationFile>true</GenerateDocumentationFile>
  </PropertyGroup>

  <!-- AOT and Trimming Compatibility -->
  <PropertyGroup Condition="$([MSBuild]::IsTargetFrameworkCompatible('$(TargetFramework)', 'net8.0'))">
    <IsAotCompatible>true</IsAotCompatible>
  </PropertyGroup>

  <!-- Source Link and Debugging -->
  <PropertyGroup>
    <PublishRepositoryUrl>true</PublishRepositoryUrl>
    <EmbedUntrackedSources>true</EmbedUntrackedSources>
    <IncludeSymbols>true</IncludeSymbols>
    <SymbolPackageFormat>snupkg</SymbolPackageFormat>
    <Deterministic>true</Deterministic>
    <ContinuousIntegrationBuild Condition="'$(GITHUB_ACTIONS)' == 'true'">true</ContinuousIntegrationBuild>
  </PropertyGroup>

  <!-- Package Identity -->
  <PropertyGroup>
    <PackageId>TylerSoftware.ErrorOr</PackageId>
    <Version>3.0.0</Version>
    <Authors>Tyler Software</Authors>
    <Company>Tyler Software</Company>
    <Copyright>Copyright (c) Tyler Software 2024-2026</Copyright>
  </PropertyGroup>

  <!-- Package Description -->
  <PropertyGroup>
    <Description>A simple, fluent discriminated union of an error or a result. Supports .NET Standard 2.0, .NET 8, .NET 9, and .NET 10 with full AOT and trimming compatibility.</Description>
    <PackageTags>Result;Results;ErrorOr;Error;Handling;DiscriminatedUnion;Railway;Functional;Monad;DomainDriven</PackageTags>
    <PackageReleaseNotes>See CHANGELOG.md for release notes.</PackageReleaseNotes>
  </PropertyGroup>

  <!-- Package Legal -->
  <PropertyGroup>
    <PackageLicenseExpression>MIT</PackageLicenseExpression>
  </PropertyGroup>

  <!-- Package Repository -->
  <PropertyGroup>
    <RepositoryType>git</RepositoryType>
    <RepositoryUrl>https://github.com/tmiller1995/error-or</RepositoryUrl>
    <PackageProjectUrl>https://github.com/tmiller1995/error-or</PackageProjectUrl>
  </PropertyGroup>

  <!-- Package Assets -->
  <PropertyGroup>
    <PackageIcon>icon-square.png</PackageIcon>
    <PackageReadmeFile>README.md</PackageReadmeFile>
    <PackageOutputPath>../artifacts/</PackageOutputPath>
  </PropertyGroup>

  <!-- Package Files -->
  <ItemGroup>
    <None Include="../assets/icon-square.png" Pack="true" Visible="false" PackagePath="" />
    <None Include="../README.md" Pack="true" PackagePath="" />
  </ItemGroup>

  <!-- Code Style -->
  <ItemGroup>
    <None Include="Stylecop.json" />
    <AdditionalFiles Include="Stylecop.json" />
  </ItemGroup>

  <!-- Dependencies -->
  <ItemGroup>
    <!-- HashCode polyfill for netstandard2.0 -->
    <PackageReference Include="Microsoft.Bcl.HashCode" Version="1.1.1" Condition="'$(TargetFramework)' == 'netstandard2.0'" />

    <!-- Nullable reference types for older frameworks -->
    <PackageReference Include="Nullable" Version="1.3.1">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers</IncludeAssets>
    </PackageReference>

    <!-- Source Link -->
    <PackageReference Include="Microsoft.SourceLink.GitHub" Version="8.0.0" PrivateAssets="All" />
  </ItemGroup>

</Project>
```

---

## Breaking Changes Summary

### Version 3.0.0 Breaking Changes

1. **Package Rename**: `ErrorOr` -> `TylerSoftware.ErrorOr`
2. **Namespace Change** (if implemented): `ErrorOr` -> `TylerSoftware.ErrorOr`
3. **Minimum .NET Framework**: Remains `netstandard2.0` (no change)

### Non-Breaking Additions

1. New target frameworks: `net9.0`, `net10.0`
2. AOT compatibility
3. Trimming compatibility
4. Source Link support
5. Symbol packages

---

## Timeline Recommendation

| Phase | Description | Priority |
|-------|-------------|----------|
| 1 | Package Rebranding | High |
| 2 | Target Framework Updates | High |
| 3 | AOT/Trimming Compatibility | High |
| 4 | Source Link Integration | Medium |
| 5 | NuGet Package Improvements | Medium |
| 8 | CI/CD Modernization | Medium |
| 9 | Test Updates | Medium |
| 10 | Documentation | Medium |
| 6 | Language Features | Low |
| 7 | Performance Optimizations | Low |

---

## References

- [.NET Library Guidance](https://learn.microsoft.com/en-us/dotnet/standard/library-guidance/)
- [Cross-platform Targeting](https://learn.microsoft.com/en-us/dotnet/standard/library-guidance/cross-platform-targeting)
- [NuGet Best Practices](https://learn.microsoft.com/en-us/nuget/create-packages/package-authoring-best-practices)
- [AOT Compatibility](https://learn.microsoft.com/en-us/dotnet/core/deploying/native-aot/)
- [Prepare Libraries for Trimming](https://learn.microsoft.com/en-us/dotnet/core/deploying/trimming/prepare-libraries-for-trimming)
- [Source Link](https://learn.microsoft.com/en-us/dotnet/standard/library-guidance/sourcelink)
- [What's New in .NET 10](https://learn.microsoft.com/en-us/dotnet/core/whats-new/dotnet-10/overview)
- [What's New in C# 14](https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-14)
