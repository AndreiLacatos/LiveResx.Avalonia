# LiveResx.Avalonia

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
DynamicLocalization.Instance.SwitchCulture(new CultureInfo("de"));

// Switch back to English
DynamicLocalization.Instance.SwitchCulture(new CultureInfo("en"));
```

All controls using `{loc:Translate ...}` update automatically — no configuration, base class, or service registration required.

## Features

* ✅ Uses standard `.resx` resource files
* ✅ Runtime language switching
* ✅ Automatic UI updates
* ✅ Strongly-typed translations generated at compile time
* ✅ IntelliSense and compile-time checking
* ✅ No localization service injection
* ✅ No ViewModel properties for localized strings
* ✅ No `INotifyPropertyChanged` boilerplate in application code

## How it works

LiveResx.Avalonia consists of three parts:

* **Runtime library** – manages translations and language switching.
* **Source generator** – discovers `.resx` resources and generates strongly-typed translation objects.
* **Markup extension** – connects generated translations to Avalonia bindings.

Internally, every generated translation is represented by a small observable object. When the culture changes, the library updates all registered translations, causing Avalonia bindings to refresh automatically.
