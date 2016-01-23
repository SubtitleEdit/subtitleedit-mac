using System;
using System.Text;
using System.Collections.Generic;
using AppKit;

namespace File
{
    public class EncodingTableDataSource: NSTableViewDataSource
    {
        public List<Encoding> Source = new List<Encoding>();

        public EncodingTableDataSource (List<Encoding> source)
        {
            Source = source;
        }

        public override nint GetRowCount (NSTableView tableView)
        {
            return Source.Count;
        }

    }
}

