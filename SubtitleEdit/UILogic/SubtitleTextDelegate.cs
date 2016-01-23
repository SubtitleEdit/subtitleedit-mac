using System;
using Nikse.SubtitleEdit.Windows;
using Foundation;
using AppKit;

namespace Nikse.SubtitleEdit.UILogic
{
    public class SubtitleTextDelegate : NSTextFieldDelegate
    {
        private ISubtitleTextChanged _ctrl;

        public SubtitleTextDelegate(ISubtitleTextChanged ctrl)
        {
            _ctrl = ctrl;
        }

        public override void DidChange(NSKeyValueChange changeKind, NSIndexSet indexes, NSString forKey)
        {
            if (_ctrl != null)
            {
                _ctrl.SubtitleTextChanged();
            }
            base.DidChange(changeKind, indexes, forKey);
        }

        public override void Changed(NSNotification notification)
        {
            _ctrl.SubtitleTextChanged();
        }

        public void TextDidChange(Foundation.NSNotification notification)
        {
            _ctrl.SubtitleTextChanged();
        }

    }
}

