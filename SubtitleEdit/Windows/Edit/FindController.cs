using System;

using Foundation;
using AppKit;
using Nikse.SubtitleEdit.Core;
using UILogic;
using Nikse.SubtitleEdit.UILogic;
using System.Text.RegularExpressions;

namespace Edit
{
    public partial class FindController : NSWindowController
    {

        public bool WasFindPressed { get; set;}
        private Subtitle _subtitle;
        public FindReplaceInfo FindReplaceInfo { get; set;}

        public FindController(IntPtr handle)
            : base(handle)
        {
        }

        [Export("initWithCoder:")]
        public FindController(NSCoder coder)
            : base(coder)
        {
        }

        public FindController(Subtitle subtitle)
            : base("Find")
        {
            _subtitle = subtitle;
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
        }

        public new Find Window
        {
            get { return (Find)base.Window; }
        }

        public void FindPressed()
        {
            WasFindPressed = true;
            FindReplaceInfo = Window.GetFindReplaceInfo();
            FindReplaceInfo.PerformFind(_subtitle);           
        }

    }
}
