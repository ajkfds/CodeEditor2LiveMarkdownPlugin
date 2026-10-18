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

        // The preview control is an Avalonia control and must be created on the
        // UI thread. File creation runs on a background thread (see
        // Folder.CreateFileAsync), so building the control in a field initializer
        // would bind it to the wrong dispatcher and cause
        // "The calling thread cannot access this object" exceptions later.
        // Create it lazily on first (UI thread) access instead.
        private Views.PreviewControl? previewControl = null;
        public Views.PreviewControl PreviewControl
        {
            get
            {
                if (previewControl == null)
                {
                    if (!Dispatcher.UIThread.CheckAccess())
                    {
                        return Dispatcher.UIThread.Invoke(() => PreviewControl);
                    }
                    previewControl = new Views.PreviewControl();
                }
                return previewControl;
            }
        }
    }
}
