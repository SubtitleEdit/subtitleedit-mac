using System;

using Foundation;
using AppKit;
using Nikse.SubtitleEdit.Core;
using System.Collections.Generic;
using Nikse.SubtitleEdit.Core.SpellCheck;
using System.Text;
using Nikse.SubtitleEdit.UILogic;
using System.Linq;

namespace Edit
{
    public partial class SpellCheck : NSWindow
    {

        private List<string> _suggestions = new List<string>();

        public SpellCheck(IntPtr handle)
            : base(handle)
        {
        }

        [Export("initWithCoder:")]
        public SpellCheck(NSCoder coder)
            : base(coder)
        {
        }

        private void InitializeTable(NSTableView table)
        {
            var columns = table.TableColumns();
            columns[0].SetIdentifier(StringListTableDelegate.CellIdentifiers[0]);
            columns[0].MinWidth = 50;
            columns[0].MaxWidth = 20000;
            columns[0].Width = 2060;
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
            };

            _buttonChange.Activated += (object sender, EventArgs e) =>
            {
                (WindowController as SpellCheckController).ChangeWord(_textWordNotFound.StringValue);
            };

            _buttonChangeAll.Activated += (object sender, EventArgs e) =>
            {
                (WindowController as SpellCheckController).ChangeWordAll(_textWordNotFound.StringValue);
            };

            _buttonAddToNames.Activated += (object sender, EventArgs e) =>
            {
                (WindowController as SpellCheckController).AddToNames(_textWordNotFound.StringValue);
            };

            _buttonAddToUserDictionary.Activated += (object sender, EventArgs e) =>
            {
                (WindowController as SpellCheckController).AddToUserDictionary(_textWordNotFound.StringValue);
            };
                    
            _buttonGoogleIt.Activated += (object sender, EventArgs e) =>
            {
                System.Diagnostics.Process.Start("https://www.google.com/search?q=" + Utilities.UrlEncode(_textWordNotFound.StringValue));
            };

            _buttonUndo.Hidden = true;
            _buttonUndo.Activated += (object sender, EventArgs e) => 
                {
                    (WindowController as SpellCheckController).UndoLastAction();
                };

            _buttonUseSuggestion.Activated += (object sender, EventArgs e) =>
            {
                int idx = (int)_tableSuggestions.SelectedRow;
                if (_suggestions.Count > 0 && idx >= 0)
                {
                    (WindowController as SpellCheckController).ChangeWord(_suggestions[idx]);
                }
            };

            _buttonUseSuggestionAlways.Activated += (object sender, EventArgs e) =>
            {
                int idx = (int)_tableSuggestions.SelectedRow;
                if (_suggestions.Count > 0 && idx >= 0)
                {
                    (WindowController as SpellCheckController).ChangeWordAll(_suggestions[idx]);
                }
            };

            _popUpLanguages.Activated += (object sender, EventArgs e) =>
            {
                (WindowController as SpellCheckController).LoadDictionaries(_popUpLanguages.SelectedItem.Title);
            };      

            _checkAutoFixNames.Activated += (object sender, EventArgs e) => 
            {
                    (WindowController as SpellCheckController).SetAutoFixState(_checkAutoFixNames.State == NSCellStateValue.On);
            };

            InitializeTable(_tableSuggestions);
        }

        public void InitializeLanguages(List<string> list)
        {
            _popUpLanguages.RemoveAllItems();
            foreach (var language in list)
            {
                _popUpLanguages.AddItem(language);
            }
        }

        public void SetUndoButton(bool hidden, string text)
        {
            _buttonUndo.Hidden = hidden;
            _buttonUndo.Title = text;
        }

        public void ShowSuggestions(List<string> suggestions)
        {
            _suggestions = suggestions;
            var ds = new StringListTableDataSource(suggestions);
            _tableSuggestions.DataSource = ds;
            _tableSuggestions.Delegate = new StringListTableDelegate(ds, null);
            if (suggestions.Count > 0)
            {
                _tableSuggestions.SelectRow(0, false);
            }
        }

        public void ShowUnknownWord(SpellCheckWord currentSpellCheckWord, Paragraph currentParagraph)
        {
            string text = HtmlUtil.RemoveHtmlTags(currentParagraph.Text, true);
            _textViewFullText.Editable = true;
            _textViewFullText.TextStorage.SetString(new NSAttributedString(""));
            _textViewFullText.InsertText(new NSString(text));
            int idx = text.IndexOf(currentSpellCheckWord.Text);
            while (idx >= 0)
            {
                bool startOk = idx == 0 || text.Substring(idx - 1, 1).ToLower() == text.Substring(idx - 1, 1).ToUpper();
                if (startOk)
                {
                    int endIdx = idx + currentSpellCheckWord.Text.Length;
                    bool endOk = endIdx >= text.Length || text.Substring(endIdx, 1).ToLower() == text.Substring(endIdx, 1).ToUpper();
                    if (endOk)
                    {
                        _textViewFullText.SetTextColor(NSColor.Red, new NSRange(idx, currentSpellCheckWord.Text.Length));
                    }
                }
                if (idx < text.Length - 1)
                {
                    idx = text.IndexOf(currentSpellCheckWord.Text, idx + 1);
                }
            }
            _textViewFullText.Editable = false;

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
