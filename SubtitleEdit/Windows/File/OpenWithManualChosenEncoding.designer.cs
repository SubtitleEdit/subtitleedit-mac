// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace File
{
	[Register ("OpenWithManualChosenEncoding")]
	partial class OpenWithManualChosenEncoding
	{
		[Outlet]
		AppKit.NSButton _buttonCancel { get; set; }

		[Outlet]
		AppKit.NSButton _buttonOk { get; set; }

		[Outlet]
		AppKit.NSTableView _encodingTable { get; set; }

		[Outlet]
		AppKit.NSTextField _previewLabe { get; set; }

		[Outlet]
		AppKit.NSTextView _previewText { get; set; }

		[Outlet]
		AppKit.NSTextField _searchText { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (_buttonCancel != null) {
				_buttonCancel.Dispose ();
				_buttonCancel = null;
			}

			if (_buttonOk != null) {
				_buttonOk.Dispose ();
				_buttonOk = null;
			}

			if (_encodingTable != null) {
				_encodingTable.Dispose ();
				_encodingTable = null;
			}

			if (_searchText != null) {
				_searchText.Dispose ();
				_searchText = null;
			}

			if (_previewLabe != null) {
				_previewLabe.Dispose ();
				_previewLabe = null;
			}

			if (_previewText != null) {
				_previewText.Dispose ();
				_previewText = null;
			}
		}
	}
}
