using System;

using Foundation;
using AppKit;
using System.Text;
using System.IO;
using Nikse.SubtitleEdit.Core;

namespace File
{
    public partial class OpenWithManualChosenEncodingController : NSWindowController
    {

        public Encoding ChosenEcoding { get; set;}
        public bool WasOkPressed { get; set;}
        private byte[] _fileBuffer;

        public OpenWithManualChosenEncodingController(IntPtr handle)
            : base(handle)
        {
        }

        [Export("initWithCoder:")]
        public OpenWithManualChosenEncodingController(NSCoder coder)
            : base(coder)
        {
        }

        public OpenWithManualChosenEncodingController(string fileName)
            : base("OpenWithManualChosenEncoding")
        {
            try
            {
                using (var file = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    int length = (int)file.Length;
                    if (length > 100000)
                        length = 100000;

                    file.Position = 0;
                    _fileBuffer = new byte[length];
                    file.Read(_fileBuffer, 0, length);

                    for (int i = 0; i < length; i++)
                    {
                        if (_fileBuffer[i] < 10)
                            _fileBuffer[i] = 32;
                    }
                }
                ChosenEcoding = LanguageAutoDetect.GetEncodingFromFile(fileName);
            }
            catch
            {
                ChosenEcoding = Encoding.UTF8;
                _fileBuffer = new byte[0];
            }
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
        }

        public new OpenWithManualChosenEncoding Window
        {
            get { return (OpenWithManualChosenEncoding)base.Window; }
        }

        public void OkPressed(Encoding chosenEncoding)
        {
            WasOkPressed = true;
            ChosenEcoding = chosenEncoding;
        }

        public void ChangeEncodingSelection()
        {
            Window.ShowPreview(_fileBuffer);
        }

        public void SearchTextChanged()
        {
            (Window as OpenWithManualChosenEncoding).ApplySearchFilter();
        }

    }
}
