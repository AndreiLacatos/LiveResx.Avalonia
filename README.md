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

## Features

* ✅ Uses standard `.resx` resource files
* ✅ Runtime language switching
* ✅ Automatic UI updates
* ✅ Strongly-typed translations generated at compile time
* ✅ IntelliSense and compile-time checking
* ✅ No localization service injection
* ✅ No ViewModel properties for localized strings
* ✅ No `INotifyPropertyChanged` boilerplate in application code

## Example

Create your translations using standard `.resx` files:

```text
Resources.resx

HelloWorld = Hello, World!
```

Use them directly from XAML with full IntelliSense and compile-time validation:

```xml
<TextBlock
    Text="{loc:Translate {x:Static loc:DynamicResources.HelloWorld}}" />
```

Switching the application's language is just a single method call:

```csharp
DynamicLocalization.Instance.SwitchCulture(
    new CultureInfo("de"));
```

Every localized control updates immediately—no window recreation, no ViewModel properties, and no manual `PropertyChanged` notifications.

## How it works

LiveResx.Avalonia consists of three parts:

* **Runtime library** – manages translations and language switching.
* **Source generator** – discovers `.resx` resources and generates strongly-typed translation objects.
* **Markup extension** – connects generated translations to Avalonia bindings.

Internally, every generated translation is represented by a small observable object. When the culture changes, the library updates all registered translations, causing Avalonia bindings to refresh automatically.
