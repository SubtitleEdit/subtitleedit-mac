using System;

namespace Nikse.SubtitleEdit.Tools
{
    public class FixItem
    {
        public string Name { get; private set; }
        public string Example { get; private set; }
        public Action Action { get; private set; }
        public bool DefaultChecked { get; private set; }
        public bool Checked { get; set; }

        public FixItem(string name, string example, Action action, bool selected)
        {
            Name = name;
            Example = example;
            Action = action;
            DefaultChecked = selected;
            Checked = selected;
        }
    }
}

