using System;
using System.Collections.Generic;
using AppKit;

namespace File
{
    public class AutoBackupTableDataSource : NSTableViewDataSource
    {

        public List<AutoBackupItem> DataSource { get; }

        public AutoBackupTableDataSource(List<AutoBackupItem> dataSource)
        {
            DataSource = dataSource;
        }

        public override nint GetRowCount (NSTableView tableView)
        {
            return DataSource.Count;
        }
    }
}

