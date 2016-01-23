using System;
using AppKit;
using System.Collections.Generic;
using Nikse.SubtitleEdit.Core;

namespace Nikse.SubtitleEdit.UILogic
{
    public class PreviewTableDelegate : NSTableViewDelegate
    {

        private const string CellIdentifier = "SubCell"; 

        public const string CellIdentifierApply = "Apply";
        public const string CellIdentifierLineNumber = "LineNumber";
        public const string CellIdentifierBefore = "Before";
        public const string CellIdentifierAfter = "After";

        public static readonly List<string> CellIdentifiers = new List<string>() 
            { 
                CellIdentifierApply, 
                CellIdentifierLineNumber, 
                CellIdentifierBefore, 
                CellIdentifierAfter, 
            };
        
        private PreviewTableDataSource _dataSource;

        public PreviewTableDelegate()
        {
        }

        public PreviewTableDelegate (PreviewTableDataSource datasource)
        {
            _dataSource = datasource;
        }

        public override NSView GetViewForItem (NSTableView tableView, NSTableColumn tableColumn, nint row)
        {
            int r = (int)row;
            if (tableColumn.Identifier == CellIdentifierApply)
            {
                //var v = (NSButton)tableView.MakeView (CellIdentifier, this);
                NSButton v = null;
                if (v == null)
                {
                    v = new NSButton();
                    v.Title = string.Empty;
                    v.SetButtonType(NSButtonType.Switch);
                    if (_dataSource.Items[r].Apply)
                    {
                        v.State = NSCellStateValue.On;
                    }
                    else
                    {
                        v.State = NSCellStateValue.Off;
                    }
                    v.Activated += (object sender, EventArgs e) =>
                    {
                        _dataSource.Items[r].Apply = v.State == NSCellStateValue.On;
                    };
                }
                return v;
            }


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
                case CellIdentifierApply:
                    view.StringValue = _dataSource.Items[r].Apply.ToString();
                    break;
                case CellIdentifierLineNumber:
                    view.StringValue = _dataSource.Items[r].LineNumber;
                    break;
                case CellIdentifierBefore:
                    view.StringValue = _dataSource.Items[r].Before.ToListViewString();
                    break;
                case CellIdentifierAfter:
                    view.StringValue = _dataSource.Items[r].After.ToListViewString();
                    break;
            }

            return view;
        }  

    }
}

