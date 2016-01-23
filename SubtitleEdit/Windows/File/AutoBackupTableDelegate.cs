using System;
using AppKit;
using System.Collections.Generic;

namespace File
{
    public class AutoBackupTableDelegate : NSTableViewDelegate
    {

        private const string CellIdentifier = "SubCell"; 

        public const string CellIdentifierDateAndTime = "DateAndTime";
        public const string CellIdentifierFileName = "FileName";
        public const string CellIdentifierExtension = "Extension";
        public const string CellIdentifierSize = "Size";

        public static readonly List<string> CellIdentifiers = new List<string>() 
            { 
                CellIdentifierDateAndTime, 
                CellIdentifierFileName, 
                CellIdentifierExtension, 
                CellIdentifierSize, 
            };
        
        private AutoBackupTableDataSource _dataSource;

        public AutoBackupTableDelegate()
        {
        }

        public AutoBackupTableDelegate (AutoBackupTableDataSource datasource)
        {
            _dataSource = datasource;
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
                case CellIdentifierDateAndTime:
                    view.StringValue = _dataSource.DataSource[r].DisplayDate;
                    break;
                case CellIdentifierFileName:
                    view.StringValue = _dataSource.DataSource[r].FileName;
                    break;
                case CellIdentifierExtension:
                    view.StringValue = _dataSource.DataSource[r].Extension;
                    break;
                case CellIdentifierSize:
                    view.StringValue = _dataSource.DataSource[r].DisplaySize;
                    break;
            }

            return view;
        }  
    }
}

