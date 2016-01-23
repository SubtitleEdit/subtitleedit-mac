using System;
using Nikse.SubtitleEdit.UILogic;
using System.Collections.Generic;
using AppKit;

namespace Tools
{
    public class FixCommonErrorsPreviewTableDataSource: NSTableViewDataSource
    {
        public List<FixCommonErrorsPreviewItem> Items { get; }

        public FixCommonErrorsPreviewTableDataSource(List<FixCommonErrorsPreviewItem> dataSource)
        {
            Items = dataSource;
        }

        public override nint GetRowCount (NSTableView tableView)
        {
            return Items.Count;
        }

    }
}

