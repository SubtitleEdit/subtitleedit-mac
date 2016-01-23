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
	[Register ("PreferencesVideo")]
	partial class PreferencesVideo
	{
		[Outlet]
		AppKit.NSButtonCell _radioAVF { get; set; }

		[Outlet]
		AppKit.NSMatrix _radioVideoPlayer { get; set; }

		[Outlet]
		AppKit.NSButtonCell _radioVlc { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (_radioVlc != null) {
				_radioVlc.Dispose ();
				_radioVlc = null;
			}

			if (_radioAVF != null) {
				_radioAVF.Dispose ();
				_radioAVF = null;
			}

			if (_radioVideoPlayer != null) {
				_radioVideoPlayer.Dispose ();
				_radioVideoPlayer = null;
			}
		}
	}
}
