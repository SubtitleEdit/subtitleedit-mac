using System;

using Foundation;
using AppKit;

namespace Sync
{
    public partial class AdjustAllTimesController : NSWindowController
    {
        IAdjustAction _applyAdjustmentAction;
        double _totalAdjustmentMilliseconds;

        public AdjustAllTimesController(IntPtr handle)
            : base(handle)
        {
        }

        [Export("initWithCoder:")]
        public AdjustAllTimesController(NSCoder coder)
            : base(coder)
        {
        }

        public AdjustAllTimesController(IAdjustAction adjustAction)
            : base("AdjustAllTimes")
        {
            _applyAdjustmentAction = adjustAction;
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
        }

        public new AdjustAllTimes Window
        {
            get { return (AdjustAllTimes)base.Window; }
        }

        public void ShowEarlierOrLater(double adjustMilliseconds, AdjustmentSelection selection)
        {            
            _applyAdjustmentAction.DoAdjustment(adjustMilliseconds, selection);
            _totalAdjustmentMilliseconds += adjustMilliseconds;
            Window.SetTotalAdjustment(_totalAdjustmentMilliseconds);
        }
            
    }
}
