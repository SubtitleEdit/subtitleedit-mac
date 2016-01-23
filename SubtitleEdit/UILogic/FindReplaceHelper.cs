using System;

namespace Nikse.SubtitleEdit.UILogic
{
    public class FindReplaceHelper
    {
        public FindReplaceHelper()
        {
        }

        public string FindText { get; set;}
        public string ReplaceText { get; set;}
        public string FindReplaceType { get; set;}
        public int ParagraphIndex { get; set;}
        public int TextIndex { get; set;}
    }
}

