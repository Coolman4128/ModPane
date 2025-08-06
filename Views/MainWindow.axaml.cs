using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform;
using Avalonia.Styling;
using Avalonia.Media.Imaging;
using Avalonia.Svg.Skia;
using ModPane.Behaviors;
using Avalonia.LogicalTree;
using Avalonia.VisualTree;
using ModPane.Converters;
using System.Collections.Generic;
using Avalonia.Media;

namespace ModPane.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        foreach (var child in this.GetLogicalDescendants())
        {
            if (child is Avalonia.Svg.Skia.Svg svg)
            {
                var colorList = new List<Color>
                {
                    SvgCssHelper.GetComputedLightCss(svg),
                    SvgCssHelper.GetComputedDarkCss(svg)
                };
                var fillOrStroke = SvgCssHelper.GetCssFill(svg);
                var css = new BrushToSvgCssConverter().Convert(colorList, null!, fillOrStroke, null!);
                svg.SetValue(Avalonia.Svg.Skia.Svg.CssProperty, css);
            }
        }


        // Set initial icon based on current theme
        UpdateIcon();

        // Subscribe to theme changes
        if (Application.Current != null)
        {
            Application.Current.ActualThemeVariantChanged += OnThemeChanged;
        }

        if (OperatingSystem.IsWindows())
        {
            ExtendClientAreaToDecorationsHint = true;
            ExtendClientAreaChromeHints = Avalonia.Platform.ExtendClientAreaChromeHints.NoChrome;
            ExtendClientAreaTitleBarHeightHint = -1;

            TransparencyLevelHint = new[] {
            WindowTransparencyLevel.None
        };
        }
        else
        {
            SystemDecorations = SystemDecorations.None;
        }


        TitleBar.PointerPressed += (s, e) =>
{
    if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
        BeginMoveDrag(e);
};

    }

    private void OnThemeChanged(object? sender, EventArgs e)
    {
        UpdateIcon();
        foreach (var child in this.GetLogicalDescendants())
        {
            if (child is Avalonia.Svg.Skia.Svg svg)
            {
                var colorList = new List<Color>
                {
                    SvgCssHelper.GetComputedLightCss(svg),
                    SvgCssHelper.GetComputedDarkCss(svg)
                };
                var fillOrStroke = SvgCssHelper.GetCssFill(svg);
             
                var css = new BrushToSvgCssConverter().Convert(colorList, null!, fillOrStroke, null!);
                svg.SetValue(Avalonia.Svg.Skia.Svg.CssProperty, css);
            }
        }
    }

    private void UpdateIcon()
    {
        var currentTheme = Application.Current?.ActualThemeVariant;
        bool isDark = currentTheme == ThemeVariant.Dark;

        // Update window icon
        var iconPath = isDark ? "avares://ModPane/Assets/LogoDarkIcon.ico" : "avares://ModPane/Assets/LogoLightIcon.ico";
        var icon = new Uri(iconPath);
        Icon = new WindowIcon(AssetLoader.Open(icon));

        // Update sidebar logo
        var sidebarLogo = this.FindControl<Image>("SidebarLogo");
        if (sidebarLogo != null)
        {
            var logoUri = isDark ? new Uri("avares://ModPane/Assets/LogoDarkIcon.ico") : new Uri("avares://ModPane/Assets/LogoLightIcon.ico");
            sidebarLogo.Source = new Bitmap(AssetLoader.Open(logoUri));
        }
    }

    private void MinimizeClick(object? sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Minimized;
    }

    private void MaximizeClick(object? sender, RoutedEventArgs e)
    {
        WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
    }

    private void CloseClick(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}