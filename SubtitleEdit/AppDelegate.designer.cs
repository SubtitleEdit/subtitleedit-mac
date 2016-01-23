// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace Nikse.SubtitleEdit
{
	[Register ("AppDelegate")]
	partial class AppDelegate
	{
		[Outlet]
		AppKit.NSMenuItem _clearRecent { get; set; }

		[Outlet]
		AppKit.NSMenuItem _menuFixCommonErrors { get; set; }

		[Outlet]
		AppKit.NSMenuItem _menuItemFind { get; set; }

		[Outlet]
		AppKit.NSMenuItem _menuItemFindNext { get; set; }

		[Outlet]
		AppKit.NSMenuItem _menuItemFindPrevious { get; set; }

		[Outlet]
		AppKit.NSMenuItem _menuItemMultipleReplace { get; set; }

		[Outlet]
		AppKit.NSMenuItem _menuItemReplace { get; set; }

		[Outlet]
		AppKit.NSMenuItem _menuItemSpellCheck { get; set; }

		[Outlet]
		AppKit.NSMenuItem _openRecent { get; set; }

		[Outlet]
		AppKit.NSMenuItem _videoOpen { get; set; }

		[Action ("AboutClicked:")]
		partial void AboutClicked (Foundation.NSObject sender);

		[Action ("AdjustAllTimesClicked:")]
		partial void AdjustAllTimesClicked (Foundation.NSObject sender);

		[Action ("ChangeFrameRateClicked:")]
		partial void ChangeFrameRateClicked (Foundation.NSObject sender);

		[Action ("HelpClicked:")]
		partial void HelpClicked (Foundation.NSObject sender);

		[Action ("NewClicked:")]
		partial void NewClicked (Foundation.NSObject sender);

		[Action ("OpenClicked:")]
		partial void OpenClicked (Foundation.NSObject sender);

		[Action ("OpenWithManualChosenEncodingClicked:")]
		partial void OpenWithManualChosenEncodingClicked (Foundation.NSObject sender);

		[Action ("PreferencesClicked:")]
		partial void PreferencesClicked (Foundation.NSObject sender);

		[Action ("RemoveTextForHearingImpairedClicked:")]
		partial void RemoveTextForHearingImpairedClicked (Foundation.NSObject sender);

		[Action ("RenumberClicked:")]
		partial void RenumberClicked (Foundation.NSObject sender);

		[Action ("RestoreAutoBackupClicked:")]
		partial void RestoreAutoBackupClicked (Foundation.NSObject sender);

		[Action ("SaveAsClicked:")]
		partial void SaveAsClicked (Foundation.NSObject sender);

		[Action ("SaveClicked:")]
		partial void SaveClicked (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (_clearRecent != null) {
				_clearRecent.Dispose ();
				_clearRecent = null;
			}

			if (_menuFixCommonErrors != null) {
				_menuFixCommonErrors.Dispose ();
				_menuFixCommonErrors = null;
			}

			if (_menuItemFind != null) {
				_menuItemFind.Dispose ();
				_menuItemFind = null;
			}

			if (_menuItemFindNext != null) {
				_menuItemFindNext.Dispose ();
				_menuItemFindNext = null;
			}

			if (_menuItemFindPrevious != null) {
				_menuItemFindPrevious.Dispose ();
				_menuItemFindPrevious = null;
			}

			if (_menuItemMultipleReplace != null) {
				_menuItemMultipleReplace.Dispose ();
				_menuItemMultipleReplace = null;
			}

			if (_menuItemReplace != null) {
				_menuItemReplace.Dispose ();
				_menuItemReplace = null;
			}

			if (_openRecent != null) {
				_openRecent.Dispose ();
				_openRecent = null;
			}

			if (_videoOpen != null) {
				_videoOpen.Dispose ();
				_videoOpen = null;
			}

			if (_menuItemSpellCheck != null) {
				_menuItemSpellCheck.Dispose ();
				_menuItemSpellCheck = null;
			}
		}
	}
}
