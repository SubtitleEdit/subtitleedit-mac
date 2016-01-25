using System;

using Foundation;
using AppKit;
using Nikse.SubtitleEdit.Core;
using System.Collections.Generic;

namespace Edit
{
    public partial class SpellCheck : NSWindow
    {
        public SpellCheck(IntPtr handle)
            : base(handle)
        {
        }

        [Export("initWithCoder:")]
        public SpellCheck(NSCoder coder)
            : base(coder)
        {
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();

            this.WillClose += (object sender, EventArgs e) =>
            { 
                NSApplication.SharedApplication.StopModal(); 
            };

            var l = Configuration.Settings.Language.SpellCheck;
            Title = l.Title;

            (WindowController as SpellCheckController).InitializeSpellCheck();

            _buttonAbort.Activated += (object sender, EventArgs e) => 
                {
                    (WindowController as SpellCheckController).Abort();
                    Close();
                };

            _buttonSkipAll.Activated += (object sender, EventArgs e) => 
                {
                    (WindowController as SpellCheckController).SkipAll();
                };

            _buttonSkipOne.Activated += (object sender, EventArgs e) => 
                {
                    (WindowController as SpellCheckController).SkipOne();
                    _textWordNotFound.StringValue = string.Empty;                
                };


        }

        public void InitializeLanguages(List<string> list)
        {
            
        }

        public void ShowUnknownWord(SpellCheckWord currentSpellCheckWord, Paragraph currentParagraph)
        {
            _textWordNotFound.StringValue = currentSpellCheckWord.Text;
        }

        public void ShowProgress(int currentParagraphIndex, Subtitle _subtitle)
        {
            _progressBar.MaxValue = _subtitle.Paragraphs.Count;
            _progressBar.DoubleValue = currentParagraphIndex;
            Title = Configuration.Settings.Language.SpellCheck.Title + " - " + (currentParagraphIndex + 1) + " / " + _subtitle.Paragraphs.Count;
        }
    }
}
