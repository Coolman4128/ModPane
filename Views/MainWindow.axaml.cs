using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform;

namespace ModPane.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        var theme = Application.Current!.RequestedThemeVariant;
        bool isDark = theme == Avalonia.Styling.ThemeVariant.Dark;

        var icon = isDark ? new Uri("avares://ModPane/Assets/LogoDarkIcon.ico") : new Uri("avares://ModPane/Assets/LogoLightIcon.ico");
        Icon = new WindowIcon(AssetLoader.Open(icon));

        ExtendClientAreaToDecorationsHint = true;
        ExtendClientAreaChromeHints = Avalonia.Platform.ExtendClientAreaChromeHints.NoChrome;
        ExtendClientAreaTitleBarHeightHint = -1;

        TransparencyLevelHint = new[] {

            WindowTransparencyLevel.AcrylicBlur,
        };

        TitleBar.PointerPressed += (s, e) =>
{
    if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
        BeginMoveDrag(e);
};

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