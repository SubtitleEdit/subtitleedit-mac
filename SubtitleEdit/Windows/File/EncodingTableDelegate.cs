using System;
using AppKit;
using Foundation;
using System.Collections.Generic;

namespace File
{
    public class EncodingTableDelegate: NSTableViewDelegate
    {
        private OpenWithManualChosenEncodingController _controller;

        private const string CellIdentifier = "SubCell"; 

        public const string CellIdentifierCodePage = "CodePage";
        public const string CellIdentifierWebName= "WebName";
        public const string CellIdentifierName = "Name";

        public static readonly List<string> CellIdentifiers = new List<string>() 
            { 
                CellIdentifierCodePage, 
                CellIdentifierWebName, 
                CellIdentifierName, 
            };

        private EncodingTableDataSource DataSource;

        public override void SelectionDidChange (Foundation.NSNotification notification)
        {
            _controller.ChangeEncodingSelection (); 
        }

        public EncodingTableDelegate (EncodingTableDataSource datasource, OpenWithManualChosenEncodingController controller)
        {
            this.DataSource = datasource;
            _controller = controller;
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
                case CellIdentifierCodePage:
                    view.StringValue = DataSource.Source[r].CodePage.ToString();
                    break;
                case CellIdentifierWebName:
                    view.StringValue = DataSource.Source[r].WebName.ToString();
                    break;
                case CellIdentifierName:
                    view.StringValue = DataSource.Source[r].EncodingName.ToString();
                    break;
            }

            return view;
        }  
    }
}

