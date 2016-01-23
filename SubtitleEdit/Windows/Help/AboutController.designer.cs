// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace Nikse.SubtitleEdit.Windows.Help
{
	[Register ("AboutController")]
	partial class AboutController
	{
		[Outlet]
		AppKit.NSButton buttonDonate { get; set; }

		[Outlet]
		AppKit.NSButton buttonOk { get; set; }

		[Outlet]
		AppKit.NSTextField labelDescription { get; set; }

		[Outlet]
		AppKit.NSTextField labelTitle { get; set; }

		[Action ("buttonDonateClick:")]
		partial void buttonDonateClick (Foundation.NSObject sender);

		[Action ("buttonOkClick:")]
		partial void buttonOkClick (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (buttonDonate != null) {
				buttonDonate.Dispose ();
				buttonDonate = null;
			}

			if (buttonOk != null) {
				buttonOk.Dispose ();
				buttonOk = null;
			}

			if (labelDescription != null) {
				labelDescription.Dispose ();
				labelDescription = null;
			}

			if (labelTitle != null) {
				labelTitle.Dispose ();
				labelTitle = null;
			}
		}
	}
}
