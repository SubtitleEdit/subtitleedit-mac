using System;
using AppKit;
using Nikse.SubtitleEdit.Windows;

namespace Nikse.SubtitleEdit.UILogic
{
    public class SubtitleTableContextMenuDelegate : NSMenuDelegate
    {
        private MainWindow _mainWindow;

        public SubtitleTableContextMenuDelegate(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
        }

        public override void MenuWillOpen(NSMenu menu)
        {
            _mainWindow.ContextMenuWillOpen();
            menu.Title = "Will open";
        }

        public override void MenuWillHighlightItem(NSMenu menu, NSMenuItem item)
        {
            
        }

    }
}

