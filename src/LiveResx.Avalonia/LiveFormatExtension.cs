using System;
using System.Collections.Generic;
using Avalonia.Data;
using Avalonia.Markup.Xaml;
using Avalonia.Metadata;

namespace LiveResx.Avalonia
{
    public sealed class LiveFormatExtension : MarkupExtension
    {
        public DynamicTranslation Key { get; set; }

        [Content]
        public IList<IBinding> Bindings { get; } = new List<IBinding>();

        public override object ProvideValue(IServiceProvider _)
        {
            var mb = new MultiBinding();

            foreach (var binding in Bindings)
            {
                mb.Bindings.Add(binding);
            }

            mb.Bindings.Add(new Binding(nameof(DynamicTranslation.Text))
            {
                Source = Key,
            });
            mb.Converter = new LiveFormatConverter(Key);
            return mb;
        }
    }
}
