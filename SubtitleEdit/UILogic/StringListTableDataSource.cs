using System;
using System.Collections.Generic;
using AppKit;
using System.Linq;

namespace Nikse.SubtitleEdit.UILogic
{
    public class StringListTableDataSource: NSTableViewDataSource
    {

        public List<string> Items { get; }

        public StringListTableDataSource(IEnumerable<string> dataSource)
        {
            Items = dataSource.ToList();
        }

        public override nint GetRowCount (NSTableView tableView)
        {
            return Items.Count;
        }
    }
}

