using Avalonia.Threading;
using CodeEditor2.CodeEditor;
using CodeEditor2.CodeEditor.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pluginLiveMarkdown.Data
{
    public class LiveMarkdownFile : CodeEditor2.Data.TextFile
    {
        public static new async Task<LiveMarkdownFile> CreateAsync(string relativePath, CodeEditor2.Data.Project project)
        {
            string name;
            if (relativePath.Contains(System.IO.Path.DirectorySeparatorChar))
            {
                name = relativePath.Substring(relativePath.LastIndexOf(System.IO.Path.DirectorySeparatorChar) + 1);
            }
            else
            {
                name = relativePath;
            }
            LiveMarkdownFile fileItem = new LiveMarkdownFile()
            {
                Project = project,
                RelativePath = relativePath,
                Name = name
            };

            await fileItem.FileCheckAsync();
            if (fileItem.CodeDocument == null) System.Diagnostics.Debugger.Break();
            return fileItem;
        }

        protected override CodeEditor2.NavigatePanel.NavigatePanelNode CreateNode()
        {
            return new NavigatePanel.LiveMarkdownFileNode(this);
        }

        // live update: entry parse on edit triggers preview update
        public override DocumentParser CreateDocumentParser(DocumentParser.ParseModeEnum parseMode, System.Threading.CancellationToken? token)
        {
            if (parseMode == DocumentParser.ParseModeEnum.EditParse) StartPreviewUpdate();
            return new Parser.Parser(this, parseMode, token);
        }

        // show the live markdown preview in the code view sub pane
        public override Avalonia.Controls.Control? GetSubPaneContent()
        {
            if (!Dispatcher.UIThread.CheckAccess())
            {
                return Dispatcher.UIThread.Invoke(() => GetSubPaneContent());
            }
            StartPreviewUpdate();
            return PreviewControl;
        }

        private void StartPreviewUpdate()
        {
            Dispatcher.UIThread.Post(
            new Action(async () =>
            {
                try
                {
                    await PreviewControl.UpdateFromTextFileAsync(this);
                }
                catch (Exception)
                {
                }
            })
            );
        }

        public Views.PreviewControl PreviewControl { get; } = new Views.PreviewControl();
    }
}
