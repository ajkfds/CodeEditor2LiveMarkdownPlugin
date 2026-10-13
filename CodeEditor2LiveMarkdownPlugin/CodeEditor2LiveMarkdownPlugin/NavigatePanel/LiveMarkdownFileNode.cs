using Avalonia.Controls;
using Avalonia.Threading;
using pluginLiveMarkdown.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pluginLiveMarkdown.NavigatePanel
{
    public class LiveMarkdownFileNode : CodeEditor2.NavigatePanel.TextFileNode
    {
        public LiveMarkdownFileNode(Data.LiveMarkdownFile file) : base(file)
        {

        }

        public override void UpdateVisual()
        {
            string text = "null";
            CodeEditor2.Data.TextFile? textFile = TextFile;
            if (textFile != null) text = textFile.Name;
            Text = text;

            if (textFile == null)
            {
                Image = AjkAvaloniaLibs.Libs.Icons.GetSvgBitmap(
                    "CodeEditor2/Assets/Icons/questionDocument.svg",
                    CodeEditor2.Global.Color_Gray
                    );
                Nodes.Clear();
                return;
            }

            Image = AjkAvaloniaLibs.Libs.Icons.GetSvgBitmap(
                "CodeEditor2/Assets/Icons/markdown.svg",
                CodeEditor2.Global.Color_Gray
                );
            CodeEditor2.Controller.NavigatePanel.UpdateVisualPost();
        }

        public static new Action<ContextMenu>? CustomizeSpecificNodeContextMenu;
        protected override Action<ContextMenu>? customizeSpecificNodeContextMenu => CustomizeSpecificNodeContextMenu;
    }
}
