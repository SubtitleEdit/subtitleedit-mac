using System;
using System.Collections.Generic;
using AppKit;

namespace Nikse.SubtitleEdit.UILogic
{
    public class PreviewTableDataSource : NSTableViewDataSource
    {

        public List<PreviewItem> Items { get; }

        public PreviewTableDataSource(List<PreviewItem> dataSource)
        {
            Items = dataSource;
        }

        public override nint GetRowCount (NSTableView tableView)
        {
            return Items.Count;
        }
    }
}

