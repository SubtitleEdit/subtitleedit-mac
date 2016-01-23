using System;

using Foundation;
using AppKit;
using Nikse.SubtitleEdit.Core;

namespace Edit
{
    public partial class MultipleReplaceController : NSWindowController
    {

        public bool WasOkPressed { get; set;}
        private Subtitle _subtitle;
        public Subtitle FixedSubtitle;

        public MultipleReplaceController(IntPtr handle)
            : base(handle)
        {
        }

        [Export("initWithCoder:")]
        public MultipleReplaceController(NSCoder coder)
            : base(coder)
        {
        }

        public MultipleReplaceController(Subtitle subtitle)
            : base("MultipleReplace")
        {
            _subtitle = subtitle;
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
        }

        public new MultipleReplace Window
        {
            get { return (MultipleReplace)base.Window; }
        }

        public void OkPressed()
        {
            WasOkPressed = true;
            FixedSubtitle = Window.GetFixedSubtitle(_subtitle);
        }

        public void GeneratePreview()
        {
            Window.GeneratePreview(_subtitle, Window.MultipleSearchAndReplaceList);
        }

        public void RuleTableSelectionChanged()
        {
            Window.ShowEditRule();
        }

    }
}
