using System;
using System.Collections.Generic;
using AppKit;
using Nikse.SubtitleEdit.Core;
using Foundation;

namespace Nikse.SubtitleEdit.UILogic
{
	public class SubtitleTableDataSource : NSTableViewDataSource
	{
		public Subtitle Subtitle = new Subtitle();
        private IOpenSubtitle _openSubtitleAction;

		public SubtitleTableDataSource (Subtitle subtitle, IOpenSubtitle openSubtitleAction)
		{
			Subtitle = subtitle;
            _openSubtitleAction = openSubtitleAction;
		}

		public override nint GetRowCount (NSTableView tableView)
		{
			return Subtitle.Paragraphs.Count;
		}
       
        public override bool AcceptDrop(NSTableView tableView, NSDraggingInfo info, nint row, NSTableViewDropOperation dropOperation)
        {
            NSPasteboard pboard = info.DraggingPasteboard;
            NSArray files = (NSArray)pboard.GetPropertyListForType(NSPasteboard.NSFilenamesType);
            if (files.Count == 1)
            {
                return true;
            }
            return false;
        }

        public override NSDragOperation ValidateDrop(NSTableView tableView, NSDraggingInfo info, nint row, NSTableViewDropOperation dropOperation)
        {
            if (_openSubtitleAction != null)
            {
                return NSDragOperation.Copy;
            }
            return NSDragOperation.None;
        }

        public override void UpdateDraggingItems(NSTableView tableView, NSDraggingInfo draggingInfo)
        {
            if (_openSubtitleAction == null)
            {
                return;
            }
                
            NSPasteboard pboard = draggingInfo.DraggingPasteboard;
            NSArray files = (NSArray)pboard.GetPropertyListForType(NSPasteboard.NSFilenamesType);
            if (files.Count == 1)
            {
                _openSubtitleAction.OpenSubtitlePromptForChanges((string)files.GetItem<NSString>(0), false);
            }
        }       

    }
}

