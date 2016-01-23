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
	[Register ("ChangeFrameRate")]
	partial class ChangeFrameRate
	{
		[Outlet]
		AppKit.NSButton _buttonCancel { get; set; }

		[Outlet]
		AppKit.NSButton _buttonOk { get; set; }

		[Outlet]
		AppKit.NSComboBox _comboFromFrameRate { get; set; }

		[Outlet]
		AppKit.NSComboBox _comboToFrameRate { get; set; }

		[Outlet]
		AppKit.NSTextField _labelFromFrameRate { get; set; }

		[Outlet]
		AppKit.NSTextField _labelToFrameRate { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (_labelFromFrameRate != null) {
				_labelFromFrameRate.Dispose ();
				_labelFromFrameRate = null;
			}

			if (_labelToFrameRate != null) {
				_labelToFrameRate.Dispose ();
				_labelToFrameRate = null;
			}

			if (_comboFromFrameRate != null) {
				_comboFromFrameRate.Dispose ();
				_comboFromFrameRate = null;
			}

			if (_comboToFrameRate != null) {
				_comboToFrameRate.Dispose ();
				_comboToFrameRate = null;
			}

			if (_buttonOk != null) {
				_buttonOk.Dispose ();
				_buttonOk = null;
			}

			if (_buttonCancel != null) {
				_buttonCancel.Dispose ();
				_buttonCancel = null;
			}
		}
	}
}
