using System;
using AppKit;
using System.Collections.Generic;
using Nikse.SubtitleEdit.Core;

namespace Edit
{
    public class MultipleRulesTableDelegate : NSTableViewDelegate
    {

        private MultipleReplaceRulesTableDataSource _dataSource;
        private MultipleReplaceController _controller;

        private const string CellIdentifier = "SubCell";

        public const string CellIdentifierEnabled = "Enabled";
        public const string CellIdentifierFindWhat = "FindWhat";
        public const string CellIdentifierReplaceWith = "ReplaceWith";
        public const string CellIdentifierSearchType = "SearchType";

        public static readonly List<string> CellIdentifiers = new List<string>()
        { 
            CellIdentifierEnabled, 
            CellIdentifierFindWhat, 
            CellIdentifierReplaceWith, 
            CellIdentifierSearchType, 
        };

        public MultipleRulesTableDelegate()
        {
        }

        public MultipleRulesTableDelegate(MultipleReplaceRulesTableDataSource datasource, MultipleReplaceController controller)
        {
            _dataSource = datasource;
            _controller = controller;
        }

        public override void SelectionDidChange (Foundation.NSNotification notification)
        {
            _controller.RuleTableSelectionChanged (); 
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
                        _dataSource.Items[r].Checked = v.State == NSCellStateValue.On;
                        _controller.GeneratePreview();
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
                case CellIdentifierFindWhat:
                    view.StringValue = _dataSource.Items[r].FindWhat;
                    break;
                case CellIdentifierReplaceWith:
                    view.StringValue = _dataSource.Items[r].ReplaceWith;
                    break;
                case CellIdentifierSearchType:
                    switch (_dataSource.Items[r].SearchType)
                    {
                        case 0: 
                            view.StringValue = Configuration.Settings.Language.MultipleReplace.Normal;
                            break;
                        case 1:
                            view.StringValue = Configuration.Settings.Language.MultipleReplace.CaseSensitive;
                            break;
                        default:
                            view.StringValue = Configuration.Settings.Language.MultipleReplace.RegularExpression;
                            break;
                    }
                    break;
            }

            return view;
        }

    }
}

