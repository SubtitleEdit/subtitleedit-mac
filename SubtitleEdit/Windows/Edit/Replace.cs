using System;

using Foundation;
using AppKit;
using UILogic;
using Nikse.SubtitleEdit.Core;
using Nikse.SubtitleEdit.UILogic;

namespace Edit
{
    public partial class Replace : NSWindow
    {
        public Replace(IntPtr handle)
            : base(handle)
        {
        }

        [Export("initWithCoder:")]
        public Replace(NSCoder coder)
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

            var l = Configuration.Settings.Language.ReplaceDialog;
            Title = l.Title;

            _buttonFind.Title = l.Find.RemoveWindowsShortCut();
            _buttonFind.Activated += (object sender, EventArgs e) =>
            {
                (WindowController as ReplaceController).FindPressed();
            };

            _buttonReplace.Title = l.Replace.RemoveWindowsShortCut();
            _buttonReplace.Activated += (object sender, EventArgs e) =>
            {
                    (WindowController as ReplaceController).ReplacePressed();
            };

            _buttonReplaceAll.Title = l.ReplaceAll.RemoveWindowsShortCut();
            _buttonReplaceAll.Activated += (object sender, EventArgs e) =>
            {
                    (WindowController as ReplaceController).ReplaceAllPressed();
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
                (WindowController as ReplaceController).FindPressed();
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
                ReplaceText = _textReplace.StringValue
            };
        }

    }
}
