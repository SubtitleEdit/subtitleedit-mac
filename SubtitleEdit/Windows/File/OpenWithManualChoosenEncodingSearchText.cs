using System;
using AppKit;
using Foundation;

namespace File
{
    public class OpenWithManualChoosenEncodingSearchText : NSTextFieldDelegate
    {
            private OpenWithManualChosenEncodingController _ctrl;

        public OpenWithManualChoosenEncodingSearchText(OpenWithManualChosenEncodingController ctrl)
            {
                _ctrl = ctrl;
            }

            public override void DidChange(NSKeyValueChange changeKind, NSIndexSet indexes, NSString forKey)
            {
                _ctrl.SearchTextChanged();
                base.DidChange(changeKind, indexes, forKey);
            }

            public override void Changed(NSNotification notification)
            {
                _ctrl.SearchTextChanged();
            }

            public void TextDidChange(Foundation.NSNotification notification)
            {
                _ctrl.SearchTextChanged();
            }
    }
}

