// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace Sync
{
	[Register ("AdjustAllTimes")]
	partial class AdjustAllTimes
	{
		[Outlet]
		AppKit.NSTextField _adjustAmount { get; set; }

		[Outlet]
		AppKit.NSStepper _adjustStepper { get; set; }

		[Outlet]
		AppKit.NSButton _buttonShowEarlier { get; set; }

		[Outlet]
		AppKit.NSButton _buttonShowLater { get; set; }

		[Outlet]
		AppKit.NSTextField _labelTotalAdjustment { get; set; }

		[Outlet]
		AppKit.NSButtonCell _radioAllLines { get; set; }

		[Outlet]
		AppKit.NSMatrix _radioMatrix { get; set; }

		[Outlet]
		AppKit.NSButtonCell _radioSelectedLines { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (_adjustAmount != null) {
				_adjustAmount.Dispose ();
				_adjustAmount = null;
			}

			if (_adjustStepper != null) {
				_adjustStepper.Dispose ();
				_adjustStepper = null;
			}

			if (_buttonShowEarlier != null) {
				_buttonShowEarlier.Dispose ();
				_buttonShowEarlier = null;
			}

			if (_buttonShowLater != null) {
				_buttonShowLater.Dispose ();
				_buttonShowLater = null;
			}

			if (_labelTotalAdjustment != null) {
				_labelTotalAdjustment.Dispose ();
				_labelTotalAdjustment = null;
			}

			if (_radioMatrix != null) {
				_radioMatrix.Dispose ();
				_radioMatrix = null;
			}

			if (_radioAllLines != null) {
				_radioAllLines.Dispose ();
				_radioAllLines = null;
			}

			if (_radioSelectedLines != null) {
				_radioSelectedLines.Dispose ();
				_radioSelectedLines = null;
			}
		}
	}
}
