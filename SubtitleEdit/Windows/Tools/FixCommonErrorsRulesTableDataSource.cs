using System;
using AppKit;
using System.Collections.Generic;
using Nikse.SubtitleEdit.Tools;

namespace Tools
{
    public class FixCommonErrorsRulesTableDataSource: NSTableViewDataSource
    {
        public List<FixItem> Items { get; }

        public FixCommonErrorsRulesTableDataSource(List<FixItem> dataSource)
        {
            Items = dataSource;
        }

        public override nint GetRowCount (NSTableView tableView)
        {
            return Items.Count;
        }
    }
}

