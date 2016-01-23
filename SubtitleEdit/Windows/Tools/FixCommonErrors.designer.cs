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
	[Register ("FixCommonErrors")]
	partial class FixCommonErrors
	{
		[Outlet]
		AppKit.NSBox _boxStep1 { get; set; }

		[Outlet]
		AppKit.NSBox _boxStep2 { get; set; }

		[Outlet]
		AppKit.NSButton _buttonApplySelectedFixes { get; set; }

		[Outlet]
		AppKit.NSButton _buttonBack { get; set; }

		[Outlet]
		AppKit.NSButton _buttonCancel { get; set; }

		[Outlet]
		AppKit.NSButton _buttonInvertFixes { get; set; }

		[Outlet]
		AppKit.NSButton _buttonInvertSelection { get; set; }

		[Outlet]
		AppKit.NSButton _buttonNext { get; set; }

		[Outlet]
		AppKit.NSButton _buttonSelectAll { get; set; }

		[Outlet]
		AppKit.NSButton _buttonSelectAllFixes { get; set; }

		[Outlet]
		AppKit.NSPopUpButton _popUpLanguage { get; set; }

		[Outlet]
		AppKit.NSTextField _subtitleText { get; set; }

		[Outlet]
		AppKit.NSTableView _tablePreview { get; set; }

		[Outlet]
		AppKit.NSTableView _tableRules { get; set; }

		[Outlet]
		AppKit.NSTableView _tableSubtitle { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (_boxStep1 != null) {
				_boxStep1.Dispose ();
				_boxStep1 = null;
			}

			if (_boxStep2 != null) {
				_boxStep2.Dispose ();
				_boxStep2 = null;
			}

			if (_buttonApplySelectedFixes != null) {
				_buttonApplySelectedFixes.Dispose ();
				_buttonApplySelectedFixes = null;
			}

			if (_buttonBack != null) {
				_buttonBack.Dispose ();
				_buttonBack = null;
			}

			if (_buttonCancel != null) {
				_buttonCancel.Dispose ();
				_buttonCancel = null;
			}

			if (_buttonInvertFixes != null) {
				_buttonInvertFixes.Dispose ();
				_buttonInvertFixes = null;
			}

			if (_buttonInvertSelection != null) {
				_buttonInvertSelection.Dispose ();
				_buttonInvertSelection = null;
			}

			if (_buttonNext != null) {
				_buttonNext.Dispose ();
				_buttonNext = null;
			}

			if (_buttonSelectAll != null) {
				_buttonSelectAll.Dispose ();
				_buttonSelectAll = null;
			}

			if (_buttonSelectAllFixes != null) {
				_buttonSelectAllFixes.Dispose ();
				_buttonSelectAllFixes = null;
			}

			if (_popUpLanguage != null) {
				_popUpLanguage.Dispose ();
				_popUpLanguage = null;
			}

			if (_tablePreview != null) {
				_tablePreview.Dispose ();
				_tablePreview = null;
			}

			if (_tableRules != null) {
				_tableRules.Dispose ();
				_tableRules = null;
			}

			if (_tableSubtitle != null) {
				_tableSubtitle.Dispose ();
				_tableSubtitle = null;
			}

			if (_subtitleText != null) {
				_subtitleText.Dispose ();
				_subtitleText = null;
			}
		}
	}
}
