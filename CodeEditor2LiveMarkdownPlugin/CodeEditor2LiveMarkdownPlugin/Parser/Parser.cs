using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeEditor2.CodeEditor.Parser;

namespace pluginLiveMarkdown.Parser
{
    public class Parser : DocumentParser
    {
        [SetsRequiredMembers]
        public Parser(Data.LiveMarkdownFile file, DocumentParser.ParseModeEnum parseMode, System.Threading.CancellationToken? token) : base(file, parseMode, token)
        {
            this.Document = new CodeEditor2.CodeEditor.CodeDocument(file); // use markdown codeDocument
            if (file.CodeDocument == null) return;
            this.Document.CopyTextOnlyFrom(file.CodeDocument);
            this.ParseMode = parseMode;
            this.TextFile = file as CodeEditor2.Data.TextFile;

            ParsedDocument = new CodeEditor2.CodeEditor.ParsedDocument(file, file.RelativePath, file.CodeDocument.Version, parseMode);
        }

        public override async Task ParseAsync()
        {
            for (int line = 1; line < Document.Lines; line++)
            {
                string lineText = Document.CreateString(Document.GetLineStartIndex(line), Document.GetLineLength(line));
                if (lineText.StartsWith("# ")
                 || lineText.StartsWith("## ")
                 || lineText.StartsWith("### "))
                {
                    colorLine(Style.Color.Header, line);
                }
                else if (lineText.StartsWith("```"))
                {
                    colorLine(Style.Color.Identifier, line);
                }
            }
            await Task.CompletedTask;
        }

        private void colorLine(Style.Color color, int line)
        {
            int start = Document.GetLineStartIndex(line);
            int length = Document.GetLineLength(line);
            Document.TextColors.SetColorAt(start, (byte)color, length);
        }
    }
}
