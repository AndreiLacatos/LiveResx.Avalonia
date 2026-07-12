# LiveResx.Avalonia

[![CI](https://github.com/AndreiLacatos/LiveResx.Avalonia/actions/workflows/ci.yml/badge.svg?branch=main)](https://github.com/AndreiLacatos/LiveResx.Avalonia/actions/workflows/ci.yml)
[![License: MIT](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

A lightweight localization library for Avalonia with **runtime language switching**, **strongly-typed translations**, and **zero ViewModel boilerplate**.

Unlike traditional `.resx` localization approaches, LiveResx.Avalonia keeps using standard `.resx` files while providing automatic UI updates when the application's culture changes.

## Why?

Avalonia currently does not provide a built-in localization solution comparable to WPF's dynamic resource system for `.resx` files.

Typical approaches usually involve one or more of the following:

* restarting or recreating windows after a language change,
* exposing every localized string through a ViewModel,
* injecting localization services everywhere,
* manually raising `PropertyChanged` notifications,
* relying on reflection-based localization frameworks.

LiveResx.Avalonia aims to provide a small, strongly-typed alternative that feels natural to Avalonia applications while keeping `.resx` files as the single source of truth.

## 🚀 Getting Started


### 📋 Prerequisites

- **Avalonia 11.0** or later
- **Standard `.resx` resources** with `PublicResXFileCodeGenerator` or `InternalResXFileCodeGenerator`

---

### 📦 Installation

```bash
dotnet add package LiveResx.Avalonia
```

Or via the Package Manager Console:

```powershell
Install-Package LiveResx.Avalonia
```

---

### 1️⃣ Create a resource file

Add a `.resx` file (e.g., `Resources.resx`) with the entries you want to localize:

| Name | Value |
|------|-------|
| `Greeting` | Hello, World! |

Set the **Custom Tool** property to `PublicResXFileCodeGenerator` (or `InternalResXFileCodeGenerator` for internal types).

---

### 2️⃣ Declare the XAML namespace

In your `Window` or `UserControl`, add the markup extension namespace:

```xml
xmlns:loc="clr-namespace:LiveResx.Avalonia"
```

---

### 3️⃣ Use translations in XAML

```xml
<TextBlock Text="{loc:Translate {x:Static loc:DynamicResources.Greeting}}" />
```

> **💡 Tip:** Every `DynamicResources.*` property is strongly typed and appears in IntelliSense after the source generator runs.

---

### 4️⃣ Switch culture at runtime

```csharp
using System.Globalization;
using LiveResx.Avalonia;

// Switch to German
DynamicLocalization.Instance.SwitchLocale(new CultureInfo("de"));

// Switch back to English
DynamicLocalization.Instance.SwitchLocale(new CultureInfo("en"));

// Read the current locale
CultureInfo current = DynamicLocalization.Instance.Locale;
```

All controls using `{loc:Translate ...}` update automatically — no configuration, base class, or service registration required.

### 5️⃣ Rx integration

If your project references `System.Reactive` or `ReactiveUI`, three extension methods are emitted automatically:

```csharp
// Observe translation value changes
IObservable<string> text = DynamicResources.Greeting.ToObservable();

// Observe locale switches
IObservable<CultureInfo> locale = DynamicLocalization.Instance.ObservableLocale();

// Observe custom resource value changes
IObservable<string> flag = DynamicLocalization.Instance
    .GetResource<string>("CountryFlag").ToObservable();
```

All emit the current value immediately on subscribe, then on each subsequent change.

### 6️⃣ Custom typed resources

For culture-aware values defined in code rather than `.resx`, see the [Advanced Usage](#advanced-usage) section.

## Features

* ✅ Uses standard `.resx` resource files
* ✅ Runtime language switching
* ✅ Automatic UI updates
* ✅ Strongly-typed translations generated at compile time
* ✅ IntelliSense and compile-time checking
* ✅ No localization service injection
* ✅ No ViewModel properties for localized strings
* ✅ No `INotifyPropertyChanged` boilerplate in application code
* ✅ `ToObservable()` / `ObservableLocale()` extensions — when `System.Reactive` or `ReactiveUI` is referenced
* ✅ `ILocalizedResource<T>` — define a class and the source generator auto-creates a `DynamicResources.*` property, registers it, and wires up culture switching
* ✅ `LocalizedResource<T>` — culture-aware typed values with `Invariant` or `ParentChain` fallback
* ✅ `RegisterResource<T>()` / `GetResource<T>()` / `TryGetResource<T>()` — runtime custom resource registration

## How it works

LiveResx.Avalonia consists of three parts:

* **Runtime library** – manages translations and language switching.
* **Source generator** – discovers `.resx` resources and generates strongly-typed translation objects.
* **Markup extension** – connects generated translations to Avalonia bindings.

Internally, every generated translation is represented by a small observable object. When the culture changes, the library updates all registered translations, causing Avalonia bindings to refresh automatically.

## Advanced Usage

### Source-generated custom resources (`ILocalizedResource<T>`)

When you need a culture-switchable resource that is defined in **application code** rather than a `.resx` file — for example, paths to flag images, enum values, or configuration objects — implement `ILocalizedResource<T>` on a class.

The source generator automatically discovers the class, creates a `LocalizedResource<T>` field, registers it with `DynamicLocalization`, and exposes it as a property on `DynamicResources`.

#### 1. Define the resource

```csharp
using System.Collections.Generic;
using System.Globalization;
using LiveResx.Avalonia;

internal sealed class CountryFlags : ILocalizedResource<string>
{
    public IReadOnlyDictionary<CultureInfo, string> Values { get; } =
        new Dictionary<CultureInfo, string>
        {
            [CultureInfo.InvariantCulture] = "/Assets/default.svg",
            [new CultureInfo("en")]          = "/Assets/uk.svg",
            [new CultureInfo("de")]          = "/Assets/de.svg",
        }.AsReadOnly();
}
```

#### 2. Use it in XAML

```xml
<Image Source="{Binding Value, Source={x:Static loc:DynamicResources.CountryFlags}}" />
```

> **Note:** `DynamicResources.CountryFlags` returns a `LocalizedResource<string>` instance.
> Binding to `Value` ensures the image source updates automatically when
> `DynamicLocalization.SwitchLocale` is called, because `LocalizedResource<T>.Value`
> raises `PropertyChanged`.

#### 3. Imperative usage

```csharp
// Read the current value (snapshot of the active culture)
string currentFlag = DynamicResources.CountryFlags.Value;

// Switch locale — all bindings and .Value accessors update automatically
DynamicLocalization.Instance.SwitchLocale(new CultureInfo("de"));
```

#### 4. Reactive extensions

When `System.Reactive` or `ReactiveUI` is referenced, you can observe value changes:

```csharp
IObservable<string> flag = DynamicResources.CountryFlags.ToObservable();
```

The observable emits the current value immediately on subscribe, then on each culture switch.

---

### Manual custom resources (`LocalizedResource<T>`)

If you prefer to create and manage resource instances in code instead of using the source generator:

```csharp
// Create a culture-aware resource with a fallback strategy
var flag = new LocalizedResource<string>(
    "CountryFlag",
    new Dictionary<CultureInfo, string>
    {
        [CultureInfo.InvariantCulture] = "/Assets/default.svg",
        [new CultureInfo("en")] = "/Assets/uk.svg",
        [new CultureInfo("de")] = "/Assets/de.svg"
    },
    FallbackBehavior.Invariant);

// Register it — it will refresh automatically on SwitchLocale
DynamicLocalization.Instance.RegisterResource(flag);

// Retrieve and use
string currentFlag = DynamicLocalization.Instance.GetResource<string>("CountryFlag").Value;

// Safe retrieval when unsure about type
if (DynamicLocalization.Instance.TryGetResource<string>("CountryFlag", out var resource))
{
    // Use resource.Value
}
```

This is useful when the resource values are computed at runtime, loaded from a database, or when you need multiple instances of the same type.

Both approaches support `ToObservable<T>()` when `System.Reactive` or `ReactiveUI` is referenced.
