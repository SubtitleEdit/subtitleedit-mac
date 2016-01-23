using System;

using Foundation;
using AppKit;

namespace Nikse.SubtitleEdit.Windows.Help
{
	public partial class About : NSWindow
	{
		public About (IntPtr handle) : base (handle)
		{
		}

		[Export ("initWithCoder:")]
		public About (NSCoder coder) : base (coder)
		{
		}

		public override void AwakeFromNib ()
		{
			base.AwakeFromNib ();
            this.WillClose += (object sender, EventArgs e) => 
                { 
                    NSApplication.SharedApplication.StopModal(); 
                };
		}

      

	}
}
