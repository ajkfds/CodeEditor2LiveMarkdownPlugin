using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pluginLiveMarkdown
{
    public class Plugin : CodeEditor2Plugin.IPlugin
    {
        public bool Register()
        {
            // Supply this plugin's navigate-node type (was a CreateNode()
            // override on the data class).
            CodeEditor2.NavigatePanel.NavigateNodeFactory.Register<Data.LiveMarkdownFile>(
                file => new NavigatePanel.LiveMarkdownFileNode(file));

            // register filetype (live markdown file: .md)
            FileTypes.LiveMarkdownFile fileType = new FileTypes.LiveMarkdownFile();
            CodeEditor2.Global.FileTypes.Add(fileType.ID, fileType);
            return true;
        }

        public bool Initialize()
        {
            return true;
        }

        public string Id { get { return "LiveMarkdown"; } }
    }
}
