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
	[Register ("SpellCheck")]
	partial class SpellCheck
	{
		[Outlet]
		AppKit.NSButton _buttonAbort { get; set; }

		[Outlet]
		AppKit.NSButton _buttonAddToNames { get; set; }

		[Outlet]
		AppKit.NSButton _buttonAddToUserDictionary { get; set; }

		[Outlet]
		AppKit.NSButton _buttonChange { get; set; }

		[Outlet]
		AppKit.NSButton _buttonChangeAll { get; set; }

		[Outlet]
		AppKit.NSButton _buttonSkipAll { get; set; }

		[Outlet]
		AppKit.NSButton _buttonSkipOne { get; set; }

		[Outlet]
		AppKit.NSPopUpButton _popUpLanguages { get; set; }

		[Outlet]
		AppKit.NSProgressIndicator _progressBar { get; set; }

		[Outlet]
		AppKit.NSTextField _textWordNotFound { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (_textWordNotFound != null) {
				_textWordNotFound.Dispose ();
				_textWordNotFound = null;
			}

			if (_buttonChange != null) {
				_buttonChange.Dispose ();
				_buttonChange = null;
			}

			if (_buttonChangeAll != null) {
				_buttonChangeAll.Dispose ();
				_buttonChangeAll = null;
			}

			if (_buttonSkipOne != null) {
				_buttonSkipOne.Dispose ();
				_buttonSkipOne = null;
			}

			if (_buttonSkipAll != null) {
				_buttonSkipAll.Dispose ();
				_buttonSkipAll = null;
			}

			if (_buttonAddToNames != null) {
				_buttonAddToNames.Dispose ();
				_buttonAddToNames = null;
			}

			if (_buttonAddToUserDictionary != null) {
				_buttonAddToUserDictionary.Dispose ();
				_buttonAddToUserDictionary = null;
			}

			if (_popUpLanguages != null) {
				_popUpLanguages.Dispose ();
				_popUpLanguages = null;
			}

			if (_progressBar != null) {
				_progressBar.Dispose ();
				_progressBar = null;
			}

			if (_buttonAbort != null) {
				_buttonAbort.Dispose ();
				_buttonAbort = null;
			}
		}
	}
}
