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
	[Register ("EditInterjections")]
	partial class EditInterjections
	{
		[Outlet]
		AppKit.NSTextField _addText { get; set; }

		[Outlet]
		AppKit.NSButton _buttonAdd { get; set; }

		[Outlet]
		AppKit.NSButton _buttonCancel { get; set; }

		[Outlet]
		AppKit.NSButton _buttonOk { get; set; }

		[Outlet]
		AppKit.NSButton _buttonRemove { get; set; }

		[Outlet]
		AppKit.NSTableView _interjectionsTable { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (_buttonAdd != null) {
				_buttonAdd.Dispose ();
				_buttonAdd = null;
			}

			if (_buttonCancel != null) {
				_buttonCancel.Dispose ();
				_buttonCancel = null;
			}

			if (_buttonOk != null) {
				_buttonOk.Dispose ();
				_buttonOk = null;
			}

			if (_buttonRemove != null) {
				_buttonRemove.Dispose ();
				_buttonRemove = null;
			}

			if (_interjectionsTable != null) {
				_interjectionsTable.Dispose ();
				_interjectionsTable = null;
			}

			if (_addText != null) {
				_addText.Dispose ();
				_addText = null;
			}
		}
	}
}
