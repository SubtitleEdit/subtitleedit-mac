using System;
using AppKit;
using System.Collections.Generic;
using Nikse.SubtitleEdit.Core;

namespace Tools
{
    public class FixCommonErrorsPreviewTableDelegate: NSTableViewDelegate
    {

        private FixCommonErrorsPreviewTableDataSource _dataSource;
        private FixCommonErrorsController _controller;

        private const string CellIdentifier = "SubCell";

        public const string CellIdentifierEnabled = "Enabled";
        public const string CellIdentifierAction = "Action";
        public const string CellIdentifierBefore = "Before";
        public const string CellIdentifierAfter = "After";

        public static readonly List<string> CellIdentifiers = new List<string>()
            { 
                CellIdentifierEnabled, 
                CellIdentifierAction, 
                CellIdentifierBefore, 
                CellIdentifierAfter, 
            };

        public FixCommonErrorsPreviewTableDelegate()
        {
        }

        public FixCommonErrorsPreviewTableDelegate(FixCommonErrorsPreviewTableDataSource datasource, FixCommonErrorsController controller)
        {
            _dataSource = datasource;
            _controller = controller;
        }

        public override void SelectionDidChange (Foundation.NSNotification notification)
        {
            _controller.PreviewTableSelectionChanged (); 
        }


        public override NSView GetViewForItem(NSTableView tableView, NSTableColumn tableColumn, nint row)
        {
            int r = (int)row;
            if (tableColumn.Identifier == CellIdentifierEnabled)
            {
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
                        var b = v.State == NSCellStateValue.On;
                            _dataSource.Items[r].Apply = b;
                      //  _controller.SaveRuleState(r, b);
                    };
                }
                return v;
            }


            // This pattern allows you reuse existing views when they are no-longer in use.
            // If the returned view is null, you instance up a new view
            // If a non-null view is returned, you modify it enough to reflect the new data
            NSTextField view = (NSTextField)tableView.MakeView(CellIdentifier, this);
            if (view == null)
            {
                view = new NSTextField();
                view.Identifier = CellIdentifier;
                view.BackgroundColor = NSColor.Clear;
                view.Bordered = false;
                view.Selectable = false;
                view.Editable = false;
            }

            // Setup view based on the column selected
            switch (tableColumn.Identifier)
            {
                case CellIdentifierEnabled:
                    view.StringValue = _dataSource.Items[r].Apply.ToString();
                    break;
                case CellIdentifierAction:
                    view.StringValue = _dataSource.Items[r].Action;
                    break;
                case CellIdentifierBefore:
                    view.StringValue = _dataSource.Items[r].Before;
                    break;
                case CellIdentifierAfter:
                    view.StringValue = _dataSource.Items[r].After;
                    break;
            }

            return view;
        }

    }
}

