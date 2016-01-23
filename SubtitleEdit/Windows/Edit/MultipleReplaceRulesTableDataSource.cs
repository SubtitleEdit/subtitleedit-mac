
using System;
using AppKit;
using System.Collections.Generic;

namespace Edit
{
    public class MultipleReplaceRulesTableDataSource : NSTableViewDataSource
    {
        public List<MultipleReplace.ReplaceExpression> Items { get; }

        public MultipleReplaceRulesTableDataSource(List<MultipleReplace.ReplaceExpression> dataSource)
        {
            Items = dataSource;
        }

        public override nint GetRowCount (NSTableView tableView)
        {
            return Items.Count;
        }
    }
}

