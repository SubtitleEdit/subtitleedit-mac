using System;
using AppKit;

namespace Nikse.SubtitleEdit.UILogic
{
    public class MainWindowSplitViewHDelegate : NSSplitViewDelegate
    {
        public override nfloat SetMaxCoordinateOfSubview(NSSplitView splitView, nfloat proposedMaximumPosition, nint subviewDividerIndex)
        {
            return proposedMaximumPosition - 300;
        }

        public override nfloat SetMinCoordinateOfSubview(NSSplitView splitView, nfloat proposedMinimumPosition, nint subviewDividerIndex)
        {
            return proposedMinimumPosition + 300;
        }
    }
}