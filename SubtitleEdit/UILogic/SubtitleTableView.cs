using System;
using AppKit;
using Foundation;

namespace Nikse.SubtitleEdit.UILogic
{    
    public class SubtitleTableView : NSTableView
    {
        public SubtitleTableView()
        {
        }

        public override NSDragOperation DraggingEntered(NSDraggingInfo sender)
        {
            return NSDragOperation.Copy;  
        }

        public override void DraggingEnded (NSDraggingInfo sender)
        {
            // Update the dropped item counter and display
           
        }

    }
}

