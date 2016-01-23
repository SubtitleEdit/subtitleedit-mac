using System;

using Foundation;
using AppKit;
using Nikse.SubtitleEdit.Core;
using Nikse.SubtitleEdit.Core.Forms;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Nikse.SubtitleEdit.UILogic;

namespace Tools
{
    public partial class RemoveTextForHearingImpairedController : NSWindowController
    {

        public bool WasOkPressed { get; set;}
        public Subtitle FixedSubtitle { get; set;}
        private Subtitle _subtitle;
        private RemoveTextForHI _removeTextForHiLib;
        private Dictionary<Paragraph, string> _fixes;

        public RemoveTextForHearingImpairedController(IntPtr handle)
            : base(handle)
        {
        }

        [Export("initWithCoder:")]
        public RemoveTextForHearingImpairedController(NSCoder coder)
            : base(coder)
        {
        }

        public RemoveTextForHearingImpairedController(Subtitle subtitle)
            : base("RemoveTextForHearingImpaired")
        {
            _subtitle = subtitle;
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
        }

        public new RemoveTextForHearingImpaired Window
        {
            get { return (RemoveTextForHearingImpaired)base.Window; }
        }

        public void OkPressed(Subtitle subtitle)
        {
            WasOkPressed = true;

            FixedSubtitle = new Subtitle(_subtitle, false);
            foreach (var fix in Window.GetFixes())
            {
                var p = FixedSubtitle.GetParagraphOrDefaultById(fix.Id);
                if (p != null)
                {
                    p.Text = fix.After;
                }
            }
            FixedSubtitle.RemoveEmptyLines();
        }

        public void GeneratePreview(RemoveTextForHISettings settings)
        {
            if (_subtitle == null)
                return;

            _removeTextForHiLib = new RemoveTextForHI(settings);
            _removeTextForHiLib.Warnings = new List<int>();
            int count = 0;
            _fixes = new Dictionary<Paragraph, string>();
            var previewItems = new List<PreviewItem>();
            for (int index = 0; index < _subtitle.Paragraphs.Count; index++)
            {
                Paragraph p = _subtitle.Paragraphs[index];
                _removeTextForHiLib.WarningIndex = index - 1;
                string newText = _removeTextForHiLib.RemoveTextFromHearImpaired(p.Text);
                if (p.Text.Replace(" ", string.Empty) != newText.Replace(" ", string.Empty))
                {
                    count++;
                    previewItems.Add(new PreviewItem(p.ID, true, p.Number.ToString(CultureInfo.InvariantCulture), p.Text, newText));
                    _fixes.Add(p, newText);
                }
            }
            Window.ShowFixes(previewItems);
            //groupBoxLinesFound.Text = string.Format(_language.LinesFoundX, count);
        }

    }
}
