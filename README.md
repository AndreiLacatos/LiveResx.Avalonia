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

For non-string resources (images, enums, config objects) that also need to switch with culture:

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
var currentFlag = DynamicLocalization.Instance.GetResource<string>("CountryFlag").Value;

// Safe retrieval when unsure about type
if (DynamicLocalization.Instance.TryGetResource<string>("CountryFlag", out var resource))
{
    // Use resource.Value
}
```

Custom resources also support `ToObservable<T>()` when `System.Reactive` or `ReactiveUI` is referenced.

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
* ✅ `LocalizedResource<T>` — culture-aware typed values with `Invariant` or `ParentChain` fallback
* ✅ `RegisterResource<T>()` / `GetResource<T>()` / `TryGetResource<T>()` — runtime custom resource registration

## How it works

LiveResx.Avalonia consists of three parts:

* **Runtime library** – manages translations and language switching.
* **Source generator** – discovers `.resx` resources and generates strongly-typed translation objects.
* **Markup extension** – connects generated translations to Avalonia bindings.

Internally, every generated translation is represented by a small observable object. When the culture changes, the library updates all registered translations, causing Avalonia bindings to refresh automatically.
