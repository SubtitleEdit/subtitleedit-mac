using System;
using AppKit;

namespace Nikse.SubtitleEdit.UILogic
{
    public class MainWindowSplitViewVDelegate : NSSplitViewDelegate
    {
        public override nfloat SetMaxCoordinateOfSubview(NSSplitView splitView, nfloat proposedMaximumPosition, nint subviewDividerIndex)
        {
            if (subviewDividerIndex == 0)
            {
                return proposedMaximumPosition - 50;
            }
            return proposedMaximumPosition - 250;
        }

        public override nfloat SetMinCoordinateOfSubview(NSSplitView splitView, nfloat proposedMinimumPosition, nint subviewDividerIndex)
        {
            if (subviewDividerIndex == 0)
            {
                return proposedMinimumPosition + 250;
            }
            return proposedMinimumPosition + 50;
        }
    }
}