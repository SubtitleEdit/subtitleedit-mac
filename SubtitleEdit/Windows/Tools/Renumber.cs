using System;

using Foundation;
using AppKit;
using Nikse.SubtitleEdit.Core;

namespace Tools
{
    public partial class Renumber : NSWindow
    {

        public Renumber(IntPtr handle)
            : base(handle)
        {
        }

        [Export("initWithCoder:")]
        public Renumber(NSCoder coder)
            : base(coder)
        {
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();

            _textFieldStartNumber.IntValue = 1;
            this.WillClose += (object sender, EventArgs e) => 
            { 
                NSApplication.SharedApplication.StopModal(); 
            };
            Title = Configuration.Settings.Language.StartNumberingFrom.Title;

            _buttonOK.Activated += (object sender, EventArgs e) => 
            {
                    (WindowController as RenumberController).OkPressed(_textFieldStartNumber.IntValue);
                    Close();    
            };

            _buttonCancel.Activated += (object sender, EventArgs e) =>  
            {
                Close();    
            };
        }

       


    }
}
