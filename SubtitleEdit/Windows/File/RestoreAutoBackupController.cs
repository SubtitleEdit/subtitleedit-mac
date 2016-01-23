using System;

using Foundation;
using AppKit;

namespace File
{
    public partial class RestoreAutoBackupController : NSWindowController
    {

        public string FileName { get; set;}
        public bool WasOkPressed { get; set;}

        public RestoreAutoBackupController(IntPtr handle)
            : base(handle)
        {
        }

        [Export("initWithCoder:")]
        public RestoreAutoBackupController(NSCoder coder)
            : base(coder)
        {
        }

        public RestoreAutoBackupController()
            : base("RestoreAutoBackup")
        {
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
            WasOkPressed = false;
        }

        public new RestoreAutoBackup Window
        {
            get { return (RestoreAutoBackup)base.Window; }
        }

        public void OkPressed(string fileName)
        {
            if (fileName != null)
            {
                WasOkPressed = true;
                FileName = fileName;
            }
        }

    }
}
