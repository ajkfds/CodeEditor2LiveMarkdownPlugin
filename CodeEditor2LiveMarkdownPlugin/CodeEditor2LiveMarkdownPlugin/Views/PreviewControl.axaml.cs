using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using System;
using System.Threading.Tasks;

namespace pluginLiveMarkdown.Views;

public partial class PreviewControl : UserControl
{
    public PreviewControl()
    {
        InitializeComponent();
    }

    private AjkAvaloniaLibs.Controls.LiveMarkdownControl markdown = new AjkAvaloniaLibs.Controls.LiveMarkdownControl()
    {
        Margin = new Thickness(0),
        HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch,
        VerticalAlignment = Avalonia.Layout.VerticalAlignment.Stretch,
        FontFamily = new Avalonia.Media.FontFamily("Cascadia Mono,Consolas,Menlo,Monospace,DejaVu Sans Mono,Liberation Mono,Noto Sans Mono,Source Code Pro"),
    };

    private CodeEditor2.Data.TextFile? targetFile = null;
    private ulong lastVersion = 0;
    private bool initialized = false;

    protected override void OnInitialized()
    {
        base.OnInitialized();
        if (Content == null)
        {
            Content = markdown;
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
