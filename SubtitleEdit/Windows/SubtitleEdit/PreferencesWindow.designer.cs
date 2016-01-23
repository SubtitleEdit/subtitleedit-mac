// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace SubtitleEdit
{
	[Register ("PreferencesWindow")]
	partial class PreferencesWindow
	{
		[Outlet]
		AppKit.NSToolbar _preferencesToolbar { get; set; }

		[Outlet]
		AppKit.NSView _preferencesView { get; set; }

		[Outlet]
		AppKit.NSToolbarItem _toolbarGeneral { get; set; }

		[Outlet]
		AppKit.NSToolbarItem _toolbarShortcuts { get; set; }

		[Outlet]
		AppKit.NSToolbarItem _toolbarVideo { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (_preferencesToolbar != null) {
				_preferencesToolbar.Dispose ();
				_preferencesToolbar = null;
			}

			if (_preferencesView != null) {
				_preferencesView.Dispose ();
				_preferencesView = null;
			}

			if (_toolbarGeneral != null) {
				_toolbarGeneral.Dispose ();
				_toolbarGeneral = null;
			}

			if (_toolbarShortcuts != null) {
				_toolbarShortcuts.Dispose ();
				_toolbarShortcuts = null;
			}

			if (_toolbarVideo != null) {
				_toolbarVideo.Dispose ();
				_toolbarVideo = null;
			}
		}
	}
}
