using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pluginLiveMarkdown.FileTypes
{
    public class LiveMarkdownFile : CodeEditor2.FileTypes.FileType
    {
        public override string ID { get => "LiveMarkdownFile"; }

        public override bool IsThisFileType(string relativeFilePath, CodeEditor2.Data.Project project)
        {
            if (
                relativeFilePath.ToLower().EndsWith(".lmd")
            )
            {
                return true;
            }
            return false;
        }

        public override async Task<CodeEditor2.Data.File> CreateFile(string relativeFilePath, CodeEditor2.Data.Project project)
        {
            return await Data.LiveMarkdownFile.CreateAsync(relativeFilePath, project);
        }
        public override IImage GetIconImage()
        {
            return AjkAvaloniaLibs.Libs.Icons.GetSvgBitmap(
                    "CodeEditor2/Assets/Icons/markdown.svg",
                    Avalonia.Media.Color.FromArgb(100, 200, 200, 200)
                );
        }
    }
}
