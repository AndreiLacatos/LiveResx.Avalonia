# ValidationDemo

A form validation demo that shows **`.ToObservable()`** in action — reactive, live-translated error messages driven by ViewModel composition.

## What it shows

- Three language toggle (EN / DE / FR) using the `LiveResx.Avalonia` NuGet package
- `{loc:Translate}` markup extension for greeting text
- **Reactive validation pipeline** in the ViewModel:
  - `WhenAnyValue` → `Throttle(300ms)` → `Select` → `.ToObservable()` → `Switch` → `ToProperty`
- A TextBox bound to `UserName` — clearing the input shows a debounced translated error
- Switching language live-updates **both** the greeting and the validation message simultaneously

## Prerequisites

- .NET 10 SDK
- Avalonia 11.3+

## Run

```bash
dotnet run --project ValidationDemo
```

## How it's wired

| Layer | What |
|---|---|
| **Translations** | `.resx` files with `HelloWorld`, `Awesome`, and `FieldRequired` in three languages |
| **ValidationDemo** | Avalonia app referencing `LiveResx.Avalonia` + `Avalonia.ReactiveUI` |
| **Source Generator** | Detects `Avalonia.ReactiveUI` referencing → emits `.ToObservable()` extension on `DynamicTranslation` |
| **ViewModel** | Composes `UserName` changes through `Throttle` → `.ToObservable()` → `Switch` → drives `ErrorMessage` |

Type in the field to clear the error. Clear the field and stop typing — the error appears after 300ms. Switch the language — both strings update live.
