
using Avalonia;
using Avalonia.Controls.Embedding.Offscreen;
using Avalonia.Media;
using Avalonia.Styling;

namespace ModPane.Behaviors;

public static class SvgCssHelper
{
    public static readonly AttachedProperty<Color> ComputedLightCssProperty =
        AvaloniaProperty.RegisterAttached<AvaloniaObject, Color>(
            "ComputedLightCss",
            typeof(SvgCssHelper));

    public static readonly AttachedProperty<Color> ComputedDarkCssProperty =
        AvaloniaProperty.RegisterAttached<AvaloniaObject, Color>(
            "ComputedDarkCss",
            typeof(SvgCssHelper));

    public static readonly AttachedProperty<bool> CssFillProperty =
        AvaloniaProperty.RegisterAttached<AvaloniaObject, bool>(
            "CssFill",
            typeof(SvgCssHelper));

    public static bool GetCssFill(AvaloniaObject element) =>
        element.GetValue(CssFillProperty);

    public static void SetCssFill(AvaloniaObject element, bool value) =>
        element.SetValue(CssFillProperty, value);

    public static void SetComputedLightCss(AvaloniaObject element, Color value) =>
        element.SetValue(ComputedLightCssProperty, value);

    public static Color GetComputedLightCss(AvaloniaObject element)
    {
        return element.GetValue(ComputedLightCssProperty);
    }

    public static void SetComputedDarkCss(AvaloniaObject element, Color value) =>
        element.SetValue(ComputedDarkCssProperty, value);

    public static Color GetComputedDarkCss(AvaloniaObject element)
    {
        return element.GetValue(ComputedDarkCssProperty);
    }
}
