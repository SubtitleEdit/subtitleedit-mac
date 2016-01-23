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
	[Register ("Find")]
	partial class Find
	{
		[Outlet]
		AppKit.NSButton _buttonCancel { get; set; }

		[Outlet]
		AppKit.NSButton _buttonFind { get; set; }

		[Outlet]
		AppKit.NSButton _checkWholeWord { get; set; }

		[Outlet]
		AppKit.NSButton _radioCaseSensitive { get; set; }

		[Outlet]
		AppKit.NSButton _radioNormal { get; set; }

		[Outlet]
		AppKit.NSButton _radioRegEx { get; set; }

		[Outlet]
		AppKit.NSTextField _textFind { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (_textFind != null) {
				_textFind.Dispose ();
				_textFind = null;
			}

			if (_buttonFind != null) {
				_buttonFind.Dispose ();
				_buttonFind = null;
			}

			if (_buttonCancel != null) {
				_buttonCancel.Dispose ();
				_buttonCancel = null;
			}

			if (_checkWholeWord != null) {
				_checkWholeWord.Dispose ();
				_checkWholeWord = null;
			}

			if (_radioNormal != null) {
				_radioNormal.Dispose ();
				_radioNormal = null;
			}

			if (_radioCaseSensitive != null) {
				_radioCaseSensitive.Dispose ();
				_radioCaseSensitive = null;
			}

			if (_radioRegEx != null) {
				_radioRegEx.Dispose ();
				_radioRegEx = null;
			}
		}
	}
}
