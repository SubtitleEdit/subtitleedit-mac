using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;
using Nikse.SubtitleEdit.Core;
using System.Text;
using System.Globalization;

namespace SubtitleEdit
{
    public partial class PreferencesGeneral : AppKit.NSView
    {
        #region Constructors

        // Called when created from unmanaged code
        public PreferencesGeneral(IntPtr handle)
            : base(handle)
        {
            Initialize();
        }

        // Called when created directly from a XIB file
        [Export("initWithCoder:")]
        public PreferencesGeneral(NSCoder coder)
            : base(coder)
        {
            Initialize();
        }

        // Shared initialization code
        void Initialize()
        {
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();

            _comboDefaultFrameRate.RemoveAll();
            _comboDefaultFrameRate.Add(new NSString((23.976).ToString()));
            _comboDefaultFrameRate.Add(new NSString((24).ToString()));
            _comboDefaultFrameRate.Add(new NSString((25).ToString()));
            _comboDefaultFrameRate.Add(new NSString((29.97).ToString()));
            _comboDefaultFrameRate.Add(new NSString((30).ToString()));
            _comboDefaultFrameRate.StringValue = Configuration.Settings.General.DefaultFrameRate.ToString();
            _comboDefaultFrameRate.Activated += (object sender, EventArgs e) =>
            {
                Configuration.Settings.General.DefaultFrameRate = _comboDefaultFrameRate.DoubleValue;
            };

            _popUpDefaultFileEncoding.RemoveAllItems();
            _popUpDefaultFileEncoding.AddItem(Encoding.UTF8.WebName);

            _popUpSingleLineMaxLength.RemoveAllItems();
            var stringList = new List<string>();
            int selectIndex = 0;
            for (int i = 10; i <= 500; i++)
            {
                stringList.Add(i.ToString());
                if (i == Configuration.Settings.General.SubtitleLineMaximumLength)
                {
                    selectIndex = stringList.Count - 1;
                }
            }
            _popUpSingleLineMaxLength.AddItems(stringList.ToArray());
            _popUpSingleLineMaxLength.SelectItem(selectIndex);
            _popUpSingleLineMaxLength.Activated += (object sender, EventArgs e) =>
            {
                Configuration.Settings.General.SubtitleLineMaximumLength = int.Parse(_popUpSingleLineMaxLength.TitleOfSelectedItem);                  
            };

            _popUpMaxCharsSec.RemoveAllItems();
            stringList = new List<string>();
            for (int i = 50; i <= 1000; i++)
            {
                stringList.Add((i / 10.0).ToString());
                if (i == Configuration.Settings.General.SubtitleMaximumCharactersPerSeconds)
                {
                    selectIndex = stringList.Count - 1;
                }
            }
            _popUpMaxCharsSec.AddItems(stringList.ToArray());
            _popUpMaxCharsSec.SelectItem(selectIndex);
            _popUpMaxCharsSec.Activated += (object sender, EventArgs e) =>
            {
                Configuration.Settings.General.SubtitleMaximumCharactersPerSeconds = double.Parse(_popUpMaxCharsSec.TitleOfSelectedItem);                  
            };

            _textMinDuration.IntValue = Configuration.Settings.General.SubtitleMinimumDisplayMilliseconds;
            _stepperMinDuration.MinValue = 100;
            _stepperMinDuration.MaxValue = 2000;
            _stepperMinDuration.Increment = 100;
            _stepperMinDuration.IntValue = Configuration.Settings.General.SubtitleMinimumDisplayMilliseconds;
            _stepperMinDuration.Activated += (object sender, EventArgs e) =>
            {
                int v = _stepperMinDuration.IntValue;
                Configuration.Settings.General.SubtitleMinimumDisplayMilliseconds = v;
                _textMinDuration.IntValue = v;
            };

            _textMaxDuration.IntValue = Configuration.Settings.General.SubtitleMaximumDisplayMilliseconds;
            _stepperMaxDuration.MinValue = 3000;
            _stepperMaxDuration.MaxValue = 50000;
            _stepperMaxDuration.Increment = 1000;
            _stepperMaxDuration.IntValue = Configuration.Settings.General.SubtitleMaximumDisplayMilliseconds;
            _stepperMaxDuration.Activated += (object sender, EventArgs e) =>
            {
                int v = _stepperMaxDuration.IntValue;
                Configuration.Settings.General.SubtitleMaximumDisplayMilliseconds = v;
                _textMaxDuration.IntValue = v;
            };

            _popUpMinGap.RemoveAllItems();
            stringList = new List<string>();
            selectIndex = 0;
            for (int i = 0; i <= 1000; i++)
            {
                stringList.Add(i.ToString());
                if (i == Configuration.Settings.General.MinimumMillisecondsBetweenLines)
                {
                    selectIndex = stringList.Count - 1;
                }
            }
            _popUpMinGap.AddItems(stringList.ToArray());
            _popUpMinGap.SelectItem(selectIndex);
            _popUpMinGap.Activated += (object sender, EventArgs e) =>
            {
                Configuration.Settings.General.MinimumMillisecondsBetweenLines = int.Parse(_popUpMinGap.TitleOfSelectedItem);
            };

            _popUpUnbreakLinesShorterThan.RemoveAllItems();
            stringList = new List<string>();
            selectIndex = 0;
            for (int i = 10; i <= 100; i++)
            {
                stringList.Add(i.ToString());
                if (i == Configuration.Settings.Tools.MergeLinesShorterThan)
                {
                    selectIndex = stringList.Count - 1;
                }
            }
            _popUpUnbreakLinesShorterThan.AddItems(stringList.ToArray());
            _popUpUnbreakLinesShorterThan.SelectItem(selectIndex);
            _popUpUnbreakLinesShorterThan.Activated += (object sender, EventArgs e) =>
            {
                Configuration.Settings.Tools.MergeLinesShorterThan = int.Parse(_popUpUnbreakLinesShorterThan.TitleOfSelectedItem);
            };

            _checkPromptDeleteLines.Title = Configuration.Settings.Language.Settings.PromptDeleteLines;
            if (Configuration.Settings.General.PromptDeleteLines)
            {
                _checkPromptDeleteLines.State = NSCellStateValue.On;
            }
            else
            {
                _checkPromptDeleteLines.State = NSCellStateValue.Off;
            }
            _checkPromptDeleteLines.Activated += (object sender, EventArgs e) =>
            {
                Configuration.Settings.General.PromptDeleteLines = _checkPromptDeleteLines.State == NSCellStateValue.On;
            };
        }

        #endregion
    }
}
