using System;
using AppKit;
using System.Collections.Generic;

namespace Nikse.SubtitleEdit.UILogic
{
    public class StringListTableDelegate : NSTableViewDelegate
    {

        private const string CellIdentifier = "SubCell"; 

        public const string CellIdentifierFirst = "First";

        private IStringListTableDelegate _callback;

        public static readonly List<string> CellIdentifiers = new List<string>() 
            { 
                CellIdentifierFirst
            };

        private StringListTableDataSource DataSource;

        public override void SelectionDidChange (Foundation.NSNotification notification)
        {
            if (_callback != null)
            {
                _callback.SelectionDidChange();
            }
        }

        public StringListTableDelegate (StringListTableDataSource datasource, IStringListTableDelegate callback)
        {
            this.DataSource = datasource;
            _callback = callback;
        }

        public override NSView GetViewForItem (NSTableView tableView, NSTableColumn tableColumn, nint row)
        {
            int r = (int)row;

            // This pattern allows you reuse existing views when they are no-longer in use.
            // If the returned view is null, you instance up a new view
            // If a non-null view is returned, you modify it enough to reflect the new data
            NSTextField view = (NSTextField)tableView.MakeView (CellIdentifier, this);
            if (view == null) {
                view = new NSTextField ();
                view.Identifier = CellIdentifier;
                view.BackgroundColor = NSColor.Clear;
                view.Bordered = false;
                view.Selectable = false;
                view.Editable = false;
            }
                
            // Setup view based on the column selected
            switch (tableColumn.Identifier) {
                case CellIdentifierFirst:
                    view.StringValue = DataSource.Items[r];
                    break;
            }

            return view;
        }  

    }
}

