using System;
using System.Text.RegularExpressions;
using System.IO;

namespace Nikse.SubtitleEdit.UILogic
{
    public class FixCommonErrorsPreviewItem
    {
        public string Id { get; set; }
        public bool Apply { get; set; }
        public string LineNumber { get; set; }
        public string Action { get; set; }
        public string Before { get; set; }
        public string After { get; set; }
        public object Tag { get; set; }

        public FixCommonErrorsPreviewItem(string id, bool apply, string lineNumber, string action, string before, string after)
        {
            Id = id;   
            Apply = apply;
            LineNumber = lineNumber;
            Action = action;
            Before = before;
            After = after;
        }

    }
}

