using System;
using AppKit;
using Nikse.SubtitleEdit.Windows;
using Nikse.SubtitleEdit.Core;

namespace Nikse.SubtitleEdit.UILogic
{
    public class MainWindowDelegate : NSWindowDelegate
    {
        MainWindowController _controller;

        public MainWindowDelegate(MainWindowController controller)
        {
            _controller = controller;
        }            
            
        public override bool WindowShouldClose(Foundation.NSObject sender)
        {
            return _controller.ContinueIfChanged();
        }

        public override void WillClose(Foundation.NSNotification notification)
        {
            _controller.DoClose();
        }
       
    }
}

