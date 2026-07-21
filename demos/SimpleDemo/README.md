# SimpleDemo

A minimal Avalonia application demonstrating **LiveResx.Avalonia** — runtime language switching with strongly-typed `.resx` translations and zero ViewModel boilerplate.

## What it shows

- Three language toggle (EN / DE / FR) using the `LiveResx.Avalonia` NuGet package
- `{loc:Translate}` markup extension that updates translations live when the culture changes
- Standard `.resx` resource files as the single source of truth

## Prerequisites

- .NET 10 SDK
- Avalonia 12.0+

## Run

```bash
dotnet run --project SimpleDemo
```

## How it's wired

| Layer | What |
|---|---|
| **Translations** | A class library with `.resx` files (EN, DE, FR) and the generated `Resources.Designer.cs` (`PublicResXFileCodeGenerator`) |
| **SimpleDemo** | The Avalonia app that references the `LiveResx.Avalonia` NuGet package and the `Translations` project |

Click a button → `DynamicLocalization.Instance.SwitchLocale(culture)` → all `{loc:Translate}` bindings refresh instantly.
