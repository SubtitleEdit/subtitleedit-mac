using System;

using Foundation;
using AppKit;
using Nikse.SubtitleEdit.Core;

namespace Sync
{
    public partial class AdjustAllTimes : NSWindow
    {

        public AdjustAllTimes(IntPtr handle)
            : base(handle)
        {
        }

        [Export("initWithCoder:")]
        public AdjustAllTimes(NSCoder coder)
            : base(coder)
        {
        }

        public void SetTotalAdjustment(double milliseconds)
        {
            _labelTotalAdjustment.StringValue = string.Format(Configuration.Settings.Language.ShowEarlierLater.TotalAdjustmentX, new TimeCode(milliseconds).ToShortString());
        }

        public int AdjustmentMilliseconds
        {
            get
            { 
                return _adjustStepper.IntValue;
            }
        }

        public AdjustmentSelection AdjustmentSelection
        {
            get
            { 
                if (_radioAllLines.State == NSCellStateValue.On)
                    return AdjustmentSelection.AllLines;
                return AdjustmentSelection.SelectedLines;
            }
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();

            this.WillClose += (object sender, EventArgs e) => 
            { 
                NSApplication.SharedApplication.StopModal(); 
            };

            _adjustStepper.IntValue = Configuration.Settings.General.DefaultAdjustMilliseconds;
            _adjustAmount.StringValue = new TimeCode(_adjustStepper.IntValue).ToString();

            _radioAllLines.Title = Configuration.Settings.Language.ShowEarlierLater.AllLines;
            _radioSelectedLines.Title = Configuration.Settings.Language.ShowEarlierLater.SelectedLinesOnly;

            _buttonShowEarlier.Activated += (object sender, EventArgs e) => 
            {
                (WindowController as AdjustAllTimesController).ShowEarlierOrLater(- AdjustmentMilliseconds, AdjustmentSelection);
            };

            _buttonShowLater.Activated += (object sender, EventArgs e) => 
            {
                (WindowController as AdjustAllTimesController).ShowEarlierOrLater(AdjustmentMilliseconds, AdjustmentSelection);
            };

            _adjustStepper.MinValue = -100000;
            _adjustStepper.MaxValue = 100000;
            _adjustStepper.Increment = 100;
            _adjustStepper.Activated += (object sender, EventArgs e) => 
            {
                _adjustAmount.StringValue = new TimeCode(AdjustmentMilliseconds).ToString();
                Configuration.Settings.General.DefaultAdjustMilliseconds = AdjustmentMilliseconds;
            };
            
        }

    }
}
