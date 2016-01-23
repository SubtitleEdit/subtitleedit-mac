// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace Video
{
	[Register ("AddWaveForm")]
	partial class AddWaveForm
	{
		[Outlet]
		AppKit.NSButton _buttonCancel { get; set; }

		[Outlet]
		AppKit.NSTextField _labelCurrentTask { get; set; }

		[Outlet]
		AppKit.NSTextField _labelPleaseWait { get; set; }

		[Outlet]
		AppKit.NSTextField _labelSourceFile { get; set; }

		[Outlet]
		AppKit.NSProgressIndicator _progressBar { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (_buttonCancel != null) {
				_buttonCancel.Dispose ();
				_buttonCancel = null;
			}

			if (_labelCurrentTask != null) {
				_labelCurrentTask.Dispose ();
				_labelCurrentTask = null;
			}

			if (_labelPleaseWait != null) {
				_labelPleaseWait.Dispose ();
				_labelPleaseWait = null;
			}

			if (_labelSourceFile != null) {
				_labelSourceFile.Dispose ();
				_labelSourceFile = null;
			}

			if (_progressBar != null) {
				_progressBar.Dispose ();
				_progressBar = null;
			}
		}
	}
}
