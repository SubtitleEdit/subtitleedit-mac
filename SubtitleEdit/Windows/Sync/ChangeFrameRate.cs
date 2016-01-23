using System;

using Foundation;
using AppKit;
using Nikse.SubtitleEdit.Core;
using System.Globalization;

namespace Sync
{
    public partial class ChangeFrameRate : NSWindow
    {
        public ChangeFrameRate(IntPtr handle)
            : base(handle)
        {
        }

        [Export("initWithCoder:")]
        public ChangeFrameRate(NSCoder coder)
            : base(coder)
        {
        }

        void FillFrameRates(NSComboBox combo)
        {
            combo.RemoveAll();
            combo.Add(new NSString((23.976).ToString(CultureInfo.CurrentUICulture)));
            combo.Add(new NSString((24).ToString(CultureInfo.CurrentUICulture)));
            combo.Add(new NSString((25).ToString(CultureInfo.CurrentUICulture)));
            combo.Add(new NSString((29.97).ToString(CultureInfo.CurrentUICulture)));
            combo.Add(new NSString((30).ToString(CultureInfo.CurrentUICulture)));
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();

            WillClose += (object sender, EventArgs e) => 
            { 
                NSApplication.SharedApplication.StopModal(); 
            };

            Title = Configuration.Settings.Language.ChangeFrameRate.Title;
            _labelFromFrameRate.StringValue = Configuration.Settings.Language.ChangeFrameRate.FromFrameRate;
            _labelToFrameRate.StringValue = Configuration.Settings.Language.ChangeFrameRate.ToFrameRate;

            FillFrameRates(_comboFromFrameRate);
            _comboFromFrameRate.StringValue = Configuration.Settings.General.CurrentFrameRate.ToString(CultureInfo.CurrentUICulture);

            FillFrameRates(_comboToFrameRate);
            _comboToFrameRate.StringValue = Configuration.Settings.General.DefaultFrameRate.ToString(CultureInfo.CurrentUICulture);

            _buttonOk.Activated += (object sender, EventArgs e) => 
            {
                (WindowController as ChangeFrameRateController).OkPressed(_comboFromFrameRate.DoubleValue, _comboToFrameRate.DoubleValue);
                Close();
            };  
            
            _buttonCancel.Activated += (object sender, EventArgs e) => 
            {
                Close();
            };  
        }

    }
}
