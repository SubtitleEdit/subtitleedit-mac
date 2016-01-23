// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace Edit
{
	[Register ("Replace")]
	partial class Replace
	{
		[Outlet]
		AppKit.NSButton _buttonFind { get; set; }

		[Outlet]
		AppKit.NSButton _buttonReplace { get; set; }

		[Outlet]
		AppKit.NSButton _buttonReplaceAll { get; set; }

		[Outlet]
		AppKit.NSTextField _labelFindWhat { get; set; }

		[Outlet]
		AppKit.NSTextField _labelReplaceWith { get; set; }

		[Outlet]
		AppKit.NSButton _radioCaseSensitive { get; set; }

		[Outlet]
		AppKit.NSButton _radioNormal { get; set; }

		[Outlet]
		AppKit.NSButton _radioRegEx { get; set; }

		[Outlet]
		AppKit.NSTextField _textFind { get; set; }

		[Outlet]
		AppKit.NSTextField _textReplace { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (_buttonFind != null) {
				_buttonFind.Dispose ();
				_buttonFind = null;
			}

			if (_buttonReplace != null) {
				_buttonReplace.Dispose ();
				_buttonReplace = null;
			}

			if (_buttonReplaceAll != null) {
				_buttonReplaceAll.Dispose ();
				_buttonReplaceAll = null;
			}

			if (_radioCaseSensitive != null) {
				_radioCaseSensitive.Dispose ();
				_radioCaseSensitive = null;
			}

			if (_radioNormal != null) {
				_radioNormal.Dispose ();
				_radioNormal = null;
			}

			if (_radioRegEx != null) {
				_radioRegEx.Dispose ();
				_radioRegEx = null;
			}

			if (_textFind != null) {
				_textFind.Dispose ();
				_textFind = null;
			}

			if (_textReplace != null) {
				_textReplace.Dispose ();
				_textReplace = null;
			}

			if (_labelFindWhat != null) {
				_labelFindWhat.Dispose ();
				_labelFindWhat = null;
			}

			if (_labelReplaceWith != null) {
				_labelReplaceWith.Dispose ();
				_labelReplaceWith = null;
			}
		}
	}
}
