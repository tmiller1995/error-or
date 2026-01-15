# Migration Guide

This guide helps you migrate from the original `ErrorOr` package to `TylerSoftware.ErrorOr`.

## From ErrorOr 2.x to TylerSoftware.ErrorOr 3.x

### Package Reference

Update your package reference in your project file:

```diff
- <PackageReference Include="ErrorOr" Version="2.0.1" />
+ <PackageReference Include="TylerSoftware.ErrorOr" Version="3.0.0" />
```

Or using the .NET CLI:

```bash
# Remove old package
dotnet remove package ErrorOr

# Add new package
dotnet add package TylerSoftware.ErrorOr
```

### Namespace

**No changes required!** The namespace remains `ErrorOr`, so your existing `using` statements will continue to work:

```csharp
using ErrorOr;
```

### API Compatibility

`TylerSoftware.ErrorOr` 3.x is **fully API-compatible** with `ErrorOr` 2.x. All types, methods, and extension methods work exactly the same way.

### New Features in 3.x

After migrating, you'll automatically benefit from:

1. **.NET 9 & 10 Support** – Native support for the latest .NET versions
2. **AOT Compatibility** – Use in Native AOT applications (MAUI, Blazor WebAssembly, etc.)
3. **Trimming Support** - Smaller deployment sizes when using trimming
4. **Source Link** – Step into the source code while debugging
5. **Symbol Packages** – Better debugging experience with `.snupkg` files

### Target Framework Compatibility

| Your Project          | Recommended TylerSoftware.ErrorOr TFM |
|-----------------------|---------------------------------------|
| .NET Framework 4.6.1+ | `netstandard2.0`                      |
| .NET Core 2.0+        | `netstandard2.0`                      |
| .NET 5/6/7            | `netstandard2.0`                      |
| .NET 8                | `net8.0`                              |
| .NET 9                | `net9.0`                              |
| .NET 10               | `net10.0`                             |

NuGet will automatically select the best target framework for your project.

## Troubleshooting

### Build Errors After Migration

If you encounter build errors after migration:

1. **Clean and rebuild** your solution:
   ```bash
   dotnet clean
   dotnet restore
   dotnet build
   ```

2. **Clear NuGet cache** if packages seem corrupted:
   ```bash
   dotnet nuget locals all --clear
   dotnet restore
   ```

### IntelliSense Not Working

If IntelliSense stops working after migration:

1. Close and reopen your IDE
2. Delete the `.vs` folder (Visual Studio) or `.idea` folder (Rider)
3. Rebuild the solution

### Questions or Issues

If you encounter any issues during migration, please [open an issue](https://github.com/tmiller1995/error-or/issues) on GitHub.
