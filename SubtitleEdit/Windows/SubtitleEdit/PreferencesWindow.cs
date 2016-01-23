using System;

using Foundation;
using AppKit;
using CoreGraphics;
using Nikse.SubtitleEdit.Core;

namespace SubtitleEdit
{
    public partial class PreferencesWindow : NSWindow
    {

        private NSViewController _subviewController = null;
        private NSView _subview = null;

        public PreferencesWindow(IntPtr handle)
            : base(handle)
        {
        }

        [Export("initWithCoder:")]
        public PreferencesWindow(NSCoder coder)
            : base(coder)
        {
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();

            Title = Configuration.Settings.Language.Settings.Title;

            _toolbarShortcuts.Activated += (object sender, EventArgs e) => 
            {
                _preferencesToolbar.SelectedItemIdentifier = "shortcuts";
                ShowPanel(new PreferencesShortcutsController()); 
            };

            _toolbarGeneral.Activated += (object sender, EventArgs e) => 
            {
                ShowGeneralPreferences();
            };

            _toolbarVideo.Activated += (object sender, EventArgs e) => 
                {
                    _preferencesToolbar.SelectedItemIdentifier = "video";
                    ShowPanel(new PreferencesVideoController());
                };

            ShowGeneralPreferences();

            this.WillClose += (object sender, EventArgs e) => 
            { 
                NSApplication.SharedApplication.StopModal(); 
            };
        }

        void ShowGeneralPreferences()
        {
            _preferencesToolbar.SelectedItemIdentifier = "general";
            ShowPanel(new PreferencesGeneralController());
        }

//        partial void OkClicked (NSObject sender)
//        {                       
//            (WindowController as PreferencesWindowController).OkPressed();
//            Close();
//        }

        private void ShowPanel(NSViewController controller) {

            // Is there a view already being displayed?
            if (_subview != null) {
                // Yes, remove it from the view
                _subview.RemoveFromSuperview ();

                // Release memory
                _subview = null;
                _subviewController = null;
            }

            // Save values
            _subviewController = controller;
            _subview = controller.View;

            // Define frame and display
            _subview.Frame = new CGRect (0, 0, _preferencesView.Frame.Width, _preferencesView.Frame.Height);
            _preferencesView.AddSubview (_subview);
        }

    }
}
