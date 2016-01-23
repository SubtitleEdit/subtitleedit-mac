using System;
using AppKit;
using System.Collections.Generic;
using System.Globalization;
using Nikse.SubtitleEdit.Windows;
using Nikse.SubtitleEdit.Core;
using Foundation;
using System.Text;
using UILogic;
using Nikse.SubtitleEdit.UILogic;
using System.Linq;

namespace Nikse.SubtitleEdit.UILogic
{
    public class SubtitleTableDelegate : NSTableViewDelegate
    {
        private IChangeSubtitleTableSelection _controller;

        private const string CellIdentifier = "SubCell";

        public const string CellIdentifierNumber = "Number";
        public const string CellIdentifierStartTime = "StartTime";
        public const string CellIdentifierEndTime = "EndTime";
        public const string CellIdentifierDuration = "Duration";
        public const string CellIdentifierText = "Text";
        public const string CellIdentifierExtra = "Extra";

        public static readonly List<string> CellIdentifiers = new List<string>()
        { 
            CellIdentifierNumber, 
            CellIdentifierStartTime, 
            CellIdentifierEndTime, 
            CellIdentifierDuration, 
            CellIdentifierText,
            CellIdentifierExtra
        };

        private SubtitleTableDataSource DataSource;

        public override void SelectionDidChange(Foundation.NSNotification notification)
        {
            _controller.ChangeSubtitleTableSelection(); 
        }

        public SubtitleTableDelegate(SubtitleTableDataSource datasource, IChangeSubtitleTableSelection controller)
        {
            this.DataSource = datasource;
            _controller = controller;
        }

        private static void ColorView(NSTextField view)
        {
            view.BackgroundColor = Configuration.Settings.Tools.ListViewSyntaxErrorColor.ToNSColor();
        }

        public override NSView GetViewForItem(NSTableView tableView, NSTableColumn tableColumn, nint row)
        {
            int r = (int)row;

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
            var p = DataSource.Subtitle.Paragraphs[r];
            view.BackgroundColor = NSColor.Clear;
            switch (tableColumn.Identifier)
            {
                case CellIdentifierNumber:
                    view.StringValue = p.Number.ToString(CultureInfo.InvariantCulture);
                    break;
                case CellIdentifierStartTime:
                    view.StringValue = p.StartTime.ToString();
                    if (Configuration.Settings.Tools.ListViewSyntaxColorOverlap && r > 0 && r < DataSource.Subtitle.Paragraphs.Count)
                    {
                        Paragraph prev = DataSource.Subtitle.Paragraphs[r - 1];
                        if (p.StartTime.TotalMilliseconds < prev.EndTime.TotalMilliseconds)
                        {
                            ColorView(view);
                            return view;
                        }
                    }
                    break;
                case CellIdentifierEndTime:
                    view.StringValue = p.EndTime.ToString();
                    if (Configuration.Settings.Tools.ListViewSyntaxColorOverlap && r >= 0 && r < DataSource.Subtitle.Paragraphs.Count - 1)
                    {
                        Paragraph next = DataSource.Subtitle.Paragraphs[r + 1];
                        if (p.EndTime.TotalMilliseconds > next.StartTime.TotalMilliseconds)
                        {
                            ColorView(view);
                            return view;
                        }
                    }
                    break;
                case CellIdentifierDuration:
                    view.StringValue = p.Duration.ToShortString();
                    if (Configuration.Settings.Tools.ListViewSyntaxColorDurationBig)
                    {                        
                        if (p.Duration.TotalMilliseconds > Configuration.Settings.General.SubtitleMaximumDisplayMilliseconds)
                        {
                            ColorView(view);
                            return view;
                        }
                    }
                    if (Configuration.Settings.Tools.ListViewSyntaxColorDurationBig)
                    {
                        if (p.Duration.TotalMilliseconds < Configuration.Settings.General.SubtitleMinimumDisplayMilliseconds)
                        {
                            ColorView(view);
                            return view;
                        }
                        double charactersPerSecond = Utilities.GetCharactersPerSecond(p);
                        if (charactersPerSecond > Configuration.Settings.General.SubtitleMaximumCharactersPerSeconds)
                        {
                            ColorView(view);
                            return view;
                        }
                    }
                    break;
                case CellIdentifierText:
                    view.StringValue = p.Text.ToListViewString();
                    if (Configuration.Settings.Tools.ListViewSyntaxColorLongLines)
                    {
                        string s = HtmlUtil.RemoveHtmlTags(p.Text, true);
                        var lines = s.SplitToLines();

                        // number of lines
                        int noOfLines = lines.Length;
                        if (noOfLines > Configuration.Settings.Tools.ListViewSyntaxMoreThanXLinesX)
                        {
                            ColorView(view);
                            return view;
                        }

                        // single line max length
                        foreach (string line in s.SplitToLines())
                        {
                            if (line.Length > Configuration.Settings.General.SubtitleLineMaximumLength)
                            {
                                ColorView(view);
                                return view;
                            }
                        }

                        // total length
                        s = s.Replace(Environment.NewLine, string.Empty); // we don't count new line in total length
                        if (s.Length > Configuration.Settings.General.SubtitleLineMaximumLength * noOfLines)
                        {
                            ColorView(view);
                            return view;
                        }
                    }
                    break;
            }

            return view;
        }
   
    }
}

