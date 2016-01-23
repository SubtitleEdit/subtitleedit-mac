using System;
using AppKit;
using System.Collections.Generic;
using Nikse.SubtitleEdit.Core;

namespace Tools
{
    public class FixCommonErrorsTableDelegate: NSTableViewDelegate
    {

        private FixCommonErrorsRulesTableDataSource _dataSource;
        private FixCommonErrorsController _controller;

        private const string CellIdentifier = "SubCell";

        public const string CellIdentifierEnabled = "Enabled";
        public const string CellIdentifierFixWhat = "FixWhat";
        public const string CellIdentifierExample = "Example";

        public static readonly List<string> CellIdentifiers = new List<string>()
            { 
                CellIdentifierEnabled, 
                CellIdentifierFixWhat, 
                CellIdentifierExample, 
            };

        public FixCommonErrorsTableDelegate()
        {
        }

        public FixCommonErrorsTableDelegate(FixCommonErrorsRulesTableDataSource datasource, FixCommonErrorsController controller)
        {
            _dataSource = datasource;
            _controller = controller;
        }

        public override void SelectionDidChange (Foundation.NSNotification notification)
        {
            //_controller.RuleTableSelectionChanged (); 
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
                    if (_dataSource.Items[r].Checked)
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
                        _dataSource.Items[r].Checked = b;
                        _controller.SaveRuleState(r, b);
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
                    view.StringValue = _dataSource.Items[r].Checked.ToString();
                    break;
                case CellIdentifierFixWhat:
                    view.StringValue = _dataSource.Items[r].Name;
                    break;
                case CellIdentifierExample:
                    view.StringValue = _dataSource.Items[r].Example;
                    break;
            }

            return view;
        }

    }
}

