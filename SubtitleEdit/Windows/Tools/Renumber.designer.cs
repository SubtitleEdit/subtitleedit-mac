// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace Tools
{
	[Register ("Renumber")]
	partial class Renumber
	{
		[Outlet]
		AppKit.NSButton _buttonCancel { get; set; }

		[Outlet]
		AppKit.NSButton _buttonOK { get; set; }

		[Outlet]
		AppKit.NSTextField _textFieldStartNumber { get; set; }

		[Action ("CancelClicked:")]
		partial void CancelClicked (Foundation.NSObject sender);

		[Action ("OkClicked:")]
		partial void OkClicked (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (_textFieldStartNumber != null) {
				_textFieldStartNumber.Dispose ();
				_textFieldStartNumber = null;
			}

			if (_buttonCancel != null) {
				_buttonCancel.Dispose ();
				_buttonCancel = null;
			}

			if (_buttonOK != null) {
				_buttonOK.Dispose ();
				_buttonOK = null;
			}
		}
	}
}
