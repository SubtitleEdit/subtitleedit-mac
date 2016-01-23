using System;

using Foundation;
using AppKit;
using Nikse.SubtitleEdit.Core;
using Nikse.SubtitleEdit.UILogic;
using UILogic;

namespace Edit
{
    public partial class Find : NSWindow
    {
        public Find(IntPtr handle)
            : base(handle)
        {
        }

        [Export("initWithCoder:")]
        public Find(NSCoder coder)
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

            var l = Configuration.Settings.Language.FindDialog;
            Title = l.Title;

            _buttonFind.Title = l.Find.RemoveWindowsShortCut();
            _buttonFind.Activated += (object sender, EventArgs e) =>
            {
                (WindowController as FindController).FindPressed();
                Close();        
            };

            _buttonCancel.Title = Configuration.Settings.Language.General.Cancel.RemoveWindowsShortCut();
            _buttonCancel.Activated += (object sender, EventArgs e) =>
            {
                Close();
            };

            _checkWholeWord.Title = l.WholeWord.RemoveWindowsShortCut();
            _checkWholeWord.Activated += (object sender, EventArgs e) =>
            {
                
            }; 

            _radioNormal.Title = l.Normal.RemoveWindowsShortCut();
            _radioNormal.Activated += (object sender, EventArgs e) =>
            {
                
            };  

            _radioCaseSensitive.Title = l.CaseSensitive.RemoveWindowsShortCut();
            _radioCaseSensitive.Activated += (object sender, EventArgs e) =>
            {

            };  
            _radioRegEx.Title = l.RegularExpression.RemoveWindowsShortCut();
            _radioRegEx.Activated += (object sender, EventArgs e) =>
            {

            };  

            _textFind.Activated += (object sender, EventArgs e) =>
            {
                (WindowController as FindController).FindPressed();
                Close(); 
            };  

        }

        private Nikse.SubtitleEdit.Core.Enums.FindType GetFindType()
        {
            if (_radioRegEx.State == NSCellStateValue.On)
            {
                return Nikse.SubtitleEdit.Core.Enums.FindType.RegEx;
            }
            if (_radioCaseSensitive.State == NSCellStateValue.On)
            {
                return Nikse.SubtitleEdit.Core.Enums.FindType.CaseSensitive;
            }
            return Nikse.SubtitleEdit.Core.Enums.FindType.Normal;
        }

        public FindReplaceInfo GetFindReplaceInfo()
        {
            return new FindReplaceInfo
            {
                FindText = _textFind.StringValue,
                FindType = GetFindType(),
            };
        }

    }
}
