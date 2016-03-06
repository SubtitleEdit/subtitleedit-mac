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
		AppKit.NSButton _buttonGoogleIt { get; set; }

		[Outlet]
		AppKit.NSButton _buttonSkipAll { get; set; }

		[Outlet]
		AppKit.NSButton _buttonSkipOne { get; set; }

		[Outlet]
		AppKit.NSButton _buttonUndo { get; set; }

		[Outlet]
		AppKit.NSButton _buttonUseSuggestion { get; set; }

		[Outlet]
		AppKit.NSButton _buttonUseSuggestionAlways { get; set; }

		[Outlet]
		AppKit.NSButton _checkAutoFixNames { get; set; }

		[Outlet]
		AppKit.NSPopUpButton _popUpLanguages { get; set; }

		[Outlet]
		AppKit.NSProgressIndicator _progressBar { get; set; }

		[Outlet]
		AppKit.NSTableView _tableSuggestions { get; set; }

		[Outlet]
		AppKit.NSTextView _textViewFullText { get; set; }

		[Outlet]
		AppKit.NSTextField _textWordNotFound { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (_buttonAbort != null) {
				_buttonAbort.Dispose ();
				_buttonAbort = null;
			}

			if (_buttonAddToNames != null) {
				_buttonAddToNames.Dispose ();
				_buttonAddToNames = null;
			}

			if (_buttonAddToUserDictionary != null) {
				_buttonAddToUserDictionary.Dispose ();
				_buttonAddToUserDictionary = null;
			}

			if (_buttonChange != null) {
				_buttonChange.Dispose ();
				_buttonChange = null;
			}

			if (_buttonChangeAll != null) {
				_buttonChangeAll.Dispose ();
				_buttonChangeAll = null;
			}

			if (_buttonGoogleIt != null) {
				_buttonGoogleIt.Dispose ();
				_buttonGoogleIt = null;
			}

			if (_buttonSkipAll != null) {
				_buttonSkipAll.Dispose ();
				_buttonSkipAll = null;
			}

			if (_buttonSkipOne != null) {
				_buttonSkipOne.Dispose ();
				_buttonSkipOne = null;
			}

			if (_buttonUseSuggestion != null) {
				_buttonUseSuggestion.Dispose ();
				_buttonUseSuggestion = null;
			}

			if (_buttonUseSuggestionAlways != null) {
				_buttonUseSuggestionAlways.Dispose ();
				_buttonUseSuggestionAlways = null;
			}

			if (_checkAutoFixNames != null) {
				_checkAutoFixNames.Dispose ();
				_checkAutoFixNames = null;
			}

			if (_popUpLanguages != null) {
				_popUpLanguages.Dispose ();
				_popUpLanguages = null;
			}

			if (_progressBar != null) {
				_progressBar.Dispose ();
				_progressBar = null;
			}

			if (_tableSuggestions != null) {
				_tableSuggestions.Dispose ();
				_tableSuggestions = null;
			}

			if (_textViewFullText != null) {
				_textViewFullText.Dispose ();
				_textViewFullText = null;
			}

			if (_textWordNotFound != null) {
				_textWordNotFound.Dispose ();
				_textWordNotFound = null;
			}

			if (_buttonUndo != null) {
				_buttonUndo.Dispose ();
				_buttonUndo = null;
			}
		}
	}
}
