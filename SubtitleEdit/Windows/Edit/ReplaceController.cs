using System;

using Foundation;
using AppKit;
using Nikse.SubtitleEdit.Core;
using UILogic;
using Nikse.SubtitleEdit.Windows;

namespace Edit
{
    public partial class ReplaceController : NSWindowController
    {
        public bool WasFindPressed { get; set;}
        public bool WasReplacePressed { get; set;}
        public bool WasReplaceAllPressed { get; set;}
        private Subtitle _subtitle;
        private MainWindowController _main;

        public FindReplaceInfo FindReplaceInfo { get; set;}

        public ReplaceController(IntPtr handle)
            : base(handle)
        {
        }

        [Export("initWithCoder:")]
        public ReplaceController(NSCoder coder)
            : base(coder)
        {
        }

        public ReplaceController(Subtitle subtitle, MainWindowController main)
            : base("Replace")
        {
            _subtitle = subtitle;
            _main = main;
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
        }

        public new Replace Window
        {
            get { return (Replace)base.Window; }
        }

        public void FindPressed()
        {
            WasFindPressed = true;
            FindReplaceInfo = Window.GetFindReplaceInfo();
            FindReplaceInfo.PerformFind(_subtitle);      
            if (FindReplaceInfo.Success)
            {
                _main.ShowFindResult(FindReplaceInfo);
            }
        }

        public void ReplacePressed()
        {
            WasFindPressed = true;
            FindReplaceInfo = Window.GetFindReplaceInfo();
            FindReplaceInfo.PerformReplace(_subtitle);      
            if (FindReplaceInfo.Success)
            {
                _main.ShowReplaceResult(FindReplaceInfo);
            }
        }

        public void ReplaceAllPressed()
        {
            WasReplaceAllPressed = true;
            FindReplaceInfo = Window.GetFindReplaceInfo();
            FindReplaceInfo.PerformReplaceAll(_subtitle);      
            _main.ShowReplaceResult(FindReplaceInfo);
            Window.Close();
        }

    }
}
