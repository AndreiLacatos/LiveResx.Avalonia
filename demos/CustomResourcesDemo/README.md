# CustomResourcesDemo

An Avalonia application demonstrating **user-defined (custom) typed resources** with LiveResx.Avalonia — combining standard `.resx` translations with `ILocalizedResource<T>` implementors defined in code.

## What it shows

- Three language toggle (EN / DE / FR) using the `LiveResx.Avalonia` NuGet package
- **Standard `.resx` resources** consumed with `{loc:Translate}` markup extension
- **Custom string resource** (`ILocalizedResource<string>`) consumed via `{Binding Value, Source={x:Static ...}}`
- **Custom Color resource** (`ILocalizedResource<Avalonia.Media.Color>`) consumed through a reactive ViewModel pipeline using `.ToObservable()`
- All resources update instantly when the culture switches

## Prerequisites

- .NET 10 SDK
- Avalonia 11.3+

## Run

```bash
dotnet run --project CustomResourcesDemo
```

## How it's wired

| Layer | What |
|---|---|
| **Translations** | A class library with `.resx` files (EN, DE, FR) and auto-generated `Resources.Designer.cs` |
| **CustomResourcesDemo** | The Avalonia app that references `LiveResx.Avalonia` and the `Translations` project; also contains `ILocalizedResource<T>` implementors (`AccentColor`, `AppTagline`) that are discovered by the source generator |

### Custom Resources

| Class | Type | Values |
|---|---|---|
| `AccentColor` | `ILocalizedResource<Color>` | EN: `#E94560`, DE: `#FF6B35`, FR: `#7C3AED` |
| `AppTagline` | `ILocalizedResource<string>` | Per-culture tagline strings |

### Consumption patterns

| Resource | Pattern |
|---|---|
| `.resx` strings (HelloWorld, AppName) | `{loc:Translate {x:Static loc:DynamicResources.HelloWorld}}` |
| Custom string resource (AppTagline) | `{Binding Value, Source={x:Static loc:DynamicResources.AppTagline}}` |
| Custom Color resource (AccentColor) | ViewModel subscribes via `DynamicResources.AccentColor.ToObservable()` — drives `AccentBrush` and `AccentHex` reactive properties |

## Why `ILocalizedResource<T>`?

Use custom typed resources when you need:

- **Non-string value types** — `Color`, `FontFamily`, `Thickness`, enums, or complex model objects
- **Resources computed at runtime** — values that depend on external data or configuration
- **Per-culture values that don't come from `.resx`** — e.g., theming colors, feature flags, or API-driven translations

The source generator discovers all `ILocalizedResource<T>` implementations in the compilation and exposes them as typed `LocalizedResource<T>` properties on `DynamicResources`, just like `.resx` keys.
