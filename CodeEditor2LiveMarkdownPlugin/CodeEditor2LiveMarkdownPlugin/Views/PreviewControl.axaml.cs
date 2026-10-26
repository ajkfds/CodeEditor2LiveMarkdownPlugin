using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.Threading;
using System;
using System.Threading.Tasks;

namespace pluginLiveMarkdown.Views;

public partial class PreviewControl : UserControl
{
    public PreviewControl()
    {
        InitializeComponent();
        ApplyPreviewStyles(markdownScope.Styles);
    }

    private AjkAvaloniaLibs.Controls.LiveMarkdownControl markdown = new AjkAvaloniaLibs.Controls.LiveMarkdownControl()
    {
        Margin = new Thickness(0),
        HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch,
        VerticalAlignment = Avalonia.Layout.VerticalAlignment.Stretch,
        FontFamily = new Avalonia.Media.FontFamily("Cascadia Mono,Consolas,Menlo,Monospace,DejaVu Sans Mono,Liberation Mono,Noto Sans Mono,Source Code Pro"),
    };

    // The markdown renderer draws text with the theme foreground color. The main
    // application runs with RequestedThemeVariant="Dark", so a plain white
    // background would leave the text white (invisible). Wrap the control in a
    // ThemeVariantScope bound to the Light theme variant so the markdown content
    // is rendered with dark text on the white background.
    private ThemeVariantScope markdownScope = new ThemeVariantScope()
    {
        RequestedThemeVariant = ThemeVariant.Light,
        HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch,
        VerticalAlignment = Avalonia.Layout.VerticalAlignment.Stretch,
        Resources = CreateLightPaletteOverrides(),
    };

    /// <summary>
    /// Palette overrides that make the diagram renderers (Mermaid / WaveDrom /
    /// DrawIo) and the markdown decorations follow the white preview background.
    /// </summary>
    /// <remarks>
    /// The LiveMarkdown styles resolve their colors through
    /// <c>{DynamicResource ...}</c> lookups of the keys declared in
    /// <c>avares://LiveMarkdown.Avalonia/Defaults.axaml</c>
    /// (ForegroundColor / BorderColor / CardBackgroundColor / ...). Those
    /// defaults are tuned for a dark background and are merged as a plain
    /// ResourceDictionary at application level (see CodeEditor2/App.axaml), so a
    /// <see cref="ThemeVariantScope"/> alone cannot switch them: the theme variant
    /// only affects ThemeDictionaries. Overriding the same keys locally on the
    /// scope keeps the change scoped to the preview and lets the diagram text and
    /// shapes render dark-on-light instead of staying in the dark palette.
    /// </remarks>
    private static ResourceDictionary CreateLightPaletteOverrides()
    {
        return new ResourceDictionary()
        {
            // Body / node / edge label text.
            ["ForegroundColor"] = Color.Parse("#FF1F2328"),
            // Node, cluster and edge-label outlines.
            ["BorderColor"] = Color.Parse("#FFD0D7DE"),
            // Node / cluster / edge label fills.
            ["CardBackgroundColor"] = Color.Parse("#FFF6F8FA"),
            // Secondary fills (group headers, inline code, quote blocks).
            ["SecondaryCardBackgroundColor"] = Color.Parse("#FFEAEEF2"),
            // Inline code and quote accents (light orange would wash out on white).
            ["CodeInlineColor"] = Color.Parse("#FF953800"),
            ["QuoteBorderColor"] = Color.Parse("#FFD0D7DE"),
        };
    }

    /// <summary>
    /// Extra styles that cannot be expressed through resource overrides.
    /// </summary>
    /// <remarks>
    /// <c>MermaidPresenter.ArrowFill</c> / <c>LineStroke</c> /
    /// <c>ThickLineStroke</c> / <c>DottedLineStroke</c> are not referenced by
    /// <c>avares://LiveMarkdown.Avalonia.Mermaid/Styles.axaml</c>, so they keep
    /// the control defaults (white). White arrow heads and connector lines would
    /// disappear on the white preview background, so they are pointed at the same
    /// brushes as the node and edge-label outlines.
    /// </remarks>
    private static void ApplyPreviewStyles(Avalonia.Styling.Styles styles)
    {
        var outlineBrush = new SolidColorBrush(Color.Parse("#FFB6BFCA"));
        var presenterStyle = new Avalonia.Styling.Style(
            x => x.OfType<LiveMarkdown.Avalonia.MermaidPresenter>());
        presenterStyle.Setters.Add(new Avalonia.Styling.Setter(
            LiveMarkdown.Avalonia.MermaidPresenter.ArrowFillProperty, outlineBrush));
        presenterStyle.Setters.Add(new Avalonia.Styling.Setter(
            LiveMarkdown.Avalonia.MermaidPresenter.LineStrokeProperty, outlineBrush));
        presenterStyle.Setters.Add(new Avalonia.Styling.Setter(
            LiveMarkdown.Avalonia.MermaidPresenter.ThickLineStrokeProperty, outlineBrush));
        presenterStyle.Setters.Add(new Avalonia.Styling.Setter(
            LiveMarkdown.Avalonia.MermaidPresenter.DottedLineStrokeProperty, outlineBrush));
        styles.Add(presenterStyle);
    }

    // White background host for the preview.
    private Border backgroundBorder = new Border()
    {
        Background = new SolidColorBrush(Colors.White),
        HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch,
        VerticalAlignment = Avalonia.Layout.VerticalAlignment.Stretch,
    };

    private CodeEditor2.Data.TextFile? targetFile = null;
    private ulong lastVersion = 0;
    private bool initialized = false;

    protected override void OnInitialized()
    {
        base.OnInitialized();
        if (Content == null)
        {
            markdownScope.Child = markdown;
            backgroundBorder.Child = markdownScope;
            Content = backgroundBorder;
        }
        initialized = true;
    }

    /// <summary>
    /// Updates the preview with the current text of the file.
    /// Call on the UI thread.
    /// </summary>
    public async Task UpdateFromTextFileAsync(CodeEditor2.Data.TextFile file)
    {
        if (file.CodeDocument == null) return;
        ulong version = file.CodeDocument.Version;
        if (initialized && targetFile == file && version == lastVersion) return; // no change

        targetFile = file;
        lastVersion = version;
        string text = file.CodeDocument.CreateString();
        await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
        {
            markdown.Text = text;
        });
    }
}
