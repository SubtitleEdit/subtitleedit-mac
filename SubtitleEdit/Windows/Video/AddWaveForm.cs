using System;

using Foundation;
using AppKit;
using Nikse.SubtitleEdit.Core;

namespace Video
{
    public partial class AddWaveForm : NSWindow
    {
        public AddWaveForm(IntPtr handle)
            : base(handle)
        {
        }

        [Export("initWithCoder:")]
        public AddWaveForm(NSCoder coder)
            : base(coder)
        {
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();


            WillClose += (object sender, EventArgs e) =>
            { 
                NSApplication.SharedApplication.StopModal(); 
            };

            Title = Configuration.Settings.Language.AddWaveform.Title;
            _labelPleaseWait.StringValue = Configuration.Settings.Language.AddWaveform.PleaseWait;

            _buttonCancel.Activated += (object sender, EventArgs e) =>
            {
                (WindowController as AddWaveFormController).DoCancel();
                _buttonCancel.Enabled = false;
            };  

            _progressBar.StartAnimation(this);
        }

        public void SetSourceFile(string text)
        {
            _labelSourceFile.StringValue = text;
        }

        public void SetProgressText(string text)
        {
            _labelCurrentTask.StringValue = text;
        }

        public void StopProgressBar()
        {
            _progressBar.StopAnimation(this);
        }

    }
}
