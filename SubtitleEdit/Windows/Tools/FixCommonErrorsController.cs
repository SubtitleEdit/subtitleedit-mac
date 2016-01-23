using System;

using Foundation;
using AppKit;
using Nikse.SubtitleEdit.Core;

namespace Tools
{
    public partial class FixCommonErrorsController : NSWindowController
    {

        public bool WasOkPressed { get; set;}
        public Subtitle Subtitle;

        public FixCommonErrorsController(IntPtr handle)
            : base(handle)
        {
        }

        [Export("initWithCoder:")]
        public FixCommonErrorsController(NSCoder coder)
            : base(coder)
        {
        }

        public FixCommonErrorsController(Subtitle subtitle)
            : base("FixCommonErrors")
        {
            Subtitle = new Subtitle(subtitle, false);
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
        }

        public new FixCommonErrors Window
        {
            get { return (FixCommonErrors)base.Window; }
        }

        public void OkPressed()
        {
            WasOkPressed = true;
            Subtitle = Window.FixedSubtitle;
        }

        public void SaveRuleState(int index, bool isChecked)
        {
            Window.SaveRuleState(index, isChecked);
        }

        public void PreviewTableSelectionChanged()
        {
            Window.PreviewTableSelectionChanged();
        }
    }
}
