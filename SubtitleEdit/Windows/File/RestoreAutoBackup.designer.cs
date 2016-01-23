// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace File
{
	[Register ("RestoreAutoBackup")]
	partial class RestoreAutoBackup
	{
		[Outlet]
		AppKit.NSTableView _autoBackupTable { get; set; }

		[Outlet]
		AppKit.NSButton buttonCancel { get; set; }

		[Outlet]
		AppKit.NSButton buttonOk { get; set; }

		[Outlet]
		AppKit.NSButton buttonOpenContainingFolder { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (_autoBackupTable != null) {
				_autoBackupTable.Dispose ();
				_autoBackupTable = null;
			}

			if (buttonCancel != null) {
				buttonCancel.Dispose ();
				buttonCancel = null;
			}

			if (buttonOk != null) {
				buttonOk.Dispose ();
				buttonOk = null;
			}

			if (buttonOpenContainingFolder != null) {
				buttonOpenContainingFolder.Dispose ();
				buttonOpenContainingFolder = null;
			}
		}
	}
}
