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
	[Register ("PreferencesVideoController")]
	partial class PreferencesVideoController
	{
		[Outlet]
		AppKit.NSButton _radioAVFPlayer { get; set; }

		[Outlet]
		AppKit.NSButton _radioVlcPlayer { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (_radioAVFPlayer != null) {
				_radioAVFPlayer.Dispose ();
				_radioAVFPlayer = null;
			}

			if (_radioVlcPlayer != null) {
				_radioVlcPlayer.Dispose ();
				_radioVlcPlayer = null;
			}
		}
	}
}
