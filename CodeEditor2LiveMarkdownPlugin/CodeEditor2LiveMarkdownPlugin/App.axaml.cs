using Avalonia;
using Avalonia.Markup.Xaml;

namespace CodeEditor2LiveMarkdownPlugin
{
    // Legacy stub: this project is a CodeEditor2 plugin library.
    // The host application registers LiveMarkdown styles in its own App.axaml.
    public partial class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
