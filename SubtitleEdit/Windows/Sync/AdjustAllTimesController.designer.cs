// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace Sync
{
	[Register ("AdjustAllTimesController")]
	partial class AdjustAllTimesController
	{
		[Outlet]
		AppKit.NSMatrix _test { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (_test != null) {
				_test.Dispose ();
				_test = null;
			}
		}
	}
}
