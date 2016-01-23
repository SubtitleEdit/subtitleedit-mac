using System;

using Foundation;
using AppKit;
using Nikse.SubtitleEdit.Windows;
using Nikse.SubtitleEdit.Core;
using Nikse.SubtitleEdit.Windows.Help;
using SubtitleEdit;
using Tools;

namespace Nikse.SubtitleEdit
{
    public partial class AppDelegate : NSApplicationDelegate
    {
        MainWindowController _mainWindowController;

        public AppDelegate()
        {
        }

        public override void DidFinishLaunching(NSNotification notification)
        {
            _mainWindowController = new MainWindowController();
            _mainWindowController.Window.MakeKeyAndOrderFront(this);  

            _menuItemMultipleReplace.Activated += (object sender, EventArgs e) =>
            {
                _mainWindowController.ShowMultipleReplace();
            };

            _menuFixCommonErrors.Activated += (object sender, EventArgs e) =>
            {
                _mainWindowController.FixCommonErrors();
            };

            _menuItemFind.Activated += (object sender, EventArgs e) =>
            {
                _mainWindowController.Find();
            };
            _menuItemReplace.Activated += (object sender, EventArgs e) =>
            {
                _mainWindowController.Replace();
            };
            _menuItemFindNext.Activated += (object sender, EventArgs e) =>
            {
                _mainWindowController.FindNext();
            };
            _menuItemFindPrevious.Activated += (object sender, EventArgs e) =>
            {
                _mainWindowController.FindPrevious();
            };

            _videoOpen.Activated += (object sender, EventArgs e) => 
            {
                _mainWindowController.OpenVideo();
            };

            _menuItemSpellCheck.Activated += (object sender, EventArgs e) => 
                {
                    _mainWindowController.SpellCheckAndGrammer();
                };
        }

        public NSMenuItem OpenRecent { get { return _openRecent; } }

        public override void WillTerminate(NSNotification notification)
        {
        }

        public override NSApplicationTerminateReply ApplicationShouldTerminate(NSApplication sender)
        {
            if (_mainWindowController.ContinueIfChanged())
            {
                return NSApplicationTerminateReply.Now;
            }
            else
            {
                return NSApplicationTerminateReply.Cancel;
            }
        }

        public override bool ApplicationShouldTerminateAfterLastWindowClosed(NSApplication sender)
        {
            return true;
        }

        #region top menu items

        // subtitle edit
        partial void PreferencesClicked(NSObject sender)
        {
            _mainWindowController.ShowPreferences();   
        }

        partial void RestoreAutoBackupClicked(NSObject sender)
        {
            using (var controller = new File.RestoreAutoBackupController())
            {
                controller.Window.ReleasedWhenClosed = true;
                NSApplication.SharedApplication.RunModalForWindow(controller.Window); // window's WillClose event stops modal
                if (controller.WasOkPressed)
                {
                    _mainWindowController.OpenSubtitlePromptForChanges(controller.FileName, true);
                }
            }   
        }


        // File
        partial void OpenClicked(NSObject sender)
        {
            _mainWindowController.OpenSubtitlePrompt();
        }

        partial void SaveClicked(NSObject sender)
        {
            _mainWindowController.SaveSubtitle();
        }

        partial void SaveAsClicked(NSObject sender)
        {
            _mainWindowController.SaveAsSubtitlePrompt();
        }

        partial void AboutClicked(NSObject sender)
        {
            using (var controller = new AboutController())
            {
                controller.Window.ReleasedWhenClosed = true;
                NSApplication.SharedApplication.RunModalForWindow(controller.Window); // window's WillClose event stops modal
            }               
        }

        partial void NewClicked(NSObject sender)
        {
            _mainWindowController.NewSubtitle();
        }

        partial void OpenWithManualChosenEncodingClicked(NSObject sender)
        {
            _mainWindowController.OpenWithManualChosenEncoding();
        }


        // Edit


        // Tools

        partial void RenumberClicked(NSObject sender)
        {
            _mainWindowController.Renumber();
        }

        partial void RemoveTextForHearingImpairedClicked(NSObject sender)
        {
            _mainWindowController.RemoveTextForHi();
        }


        // Sync
        partial void AdjustAllTimesClicked(NSObject sender)
        {
            _mainWindowController.AdjustAllTimes();
        }

        partial void ChangeFrameRateClicked(NSObject sender)
        {
            _mainWindowController.ChangeFrameRate();
        }


        // Help
        partial void HelpClicked(NSObject sender)
        {
            Utilities.ShowHelp(null);
        }

        #endregion
    }
}

