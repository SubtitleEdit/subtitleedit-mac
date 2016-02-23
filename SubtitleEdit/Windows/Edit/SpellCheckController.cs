using System;

using Foundation;
using AppKit;
using Nikse.SubtitleEdit.Core;
using SpellCheck;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Nikse.SubtitleEdit.UILogic;
using Nikse.SubtitleEdit.Core.Dictionaries;

namespace Edit
{
    public partial class SpellCheckController : NSWindowController
    {

        public bool WasChanged { get; set; }

        private Subtitle _subtitle;
        private SpellChecker _spellChecker;
        private Paragraph _currentParagraph;
        private int _currentParagraphIndex;
        private string _currentWord;
        private SpellCheckWord _currentSpellCheckWord;
        private int _wordsIndex;
        private List<SpellCheckWord> _words;
        private List<string> _skipAllList = new List<string>();
        private bool _abort = false;
        private List<string> _namesEtcList = new List<string>();

        public SpellCheckController(IntPtr handle)
            : base(handle)
        {
        }

        [Export("initWithCoder:")]
        public SpellCheckController(NSCoder coder)
            : base(coder)
        {
        }

        public SpellCheckController(Subtitle subtitle)
            : base("SpellCheck")
        {
            _subtitle = subtitle;
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
        }

        void LoadNames()
        {
            var namesList = new NamesList(Configuration.DictionariesFolder, "en", Configuration.Settings.WordLists.UseOnlineNamesEtc, Configuration.Settings.WordLists.NamesEtcUrl);
            _namesEtcList = namesList.GetAllNames();
        }

        public void InitializeSpellCheck()
        {
            _spellChecker = new SpellChecker("en");
            (Window as SpellCheck).InitializeLanguages(_spellChecker.GetLanguages());              
            _skipAllList = new List<string>();
            _currentParagraphIndex = 0;
            _words = SpellChecker.Split(_subtitle.Paragraphs[0].Text);
            _wordsIndex = -1;
            LoadNames();
            PrepareNextWord();
        }

        public new SpellCheck Window
        {
            get { return (SpellCheck)base.Window; }
        }

        public void SkipAll()
        {
            string wordOriginalCasing = _currentWord.Trim();
            string word = wordOriginalCasing.ToLower();
            _skipAllList.Add(word);
            _skipAllList.Add(word.ToUpper());
            if (word.Length > 0)
            {
                _skipAllList.Add(word.Substring(0, 1).ToUpper() + word.Substring(1));
            }
            if (!_skipAllList.Contains(wordOriginalCasing))
            {
                _skipAllList.Add(wordOriginalCasing);
            }
            PrepareNextWord();
        }

        public void SkipOne()
        {
            PrepareNextWord();
        }

        public void Abort()
        {
            _abort = true;
        }

        private void PrepareNextWord()
        {
            _abort = false;
            Task.Run(() =>
                {                    
                    InvokeOnMainThread(() =>
                        {
                            while (!_abort)
                            {
                                if (_wordsIndex + 1 < _words.Count)
                                {
                                    _wordsIndex++;
                                    _currentWord = _words[_wordsIndex].Text;
                                    _currentSpellCheckWord = _words[_wordsIndex];
                                }
                                else
                                {
                                    if (_currentParagraphIndex + 1 < _subtitle.Paragraphs.Count)
                                    {
                                        _currentParagraphIndex++;
                                        _currentParagraph = _subtitle.Paragraphs[_currentParagraphIndex];
                                        _words = SpellChecker.Split(_currentParagraph.Text);
                                        _wordsIndex = 0;
                                        if (_words.Count == 0)
                                        {
                                            _currentWord = string.Empty;
                                        }
                                        else
                                        {
                                            _currentWord = _words[_wordsIndex].Text;
                                            _currentSpellCheckWord = _words[_wordsIndex];
                                            _wordsIndex = -1;
                                        }
                                    }
                                    else
                                    {
                                        Window.ShowProgress(_currentParagraphIndex, _subtitle);
                                        _abort = true;
                                        MessageBox.Show("Spell check completed");
                                        Close();
                                        return;
                                    }
                                }

                                var spelledOk = string.IsNullOrWhiteSpace(_currentWord);

                                if (!spelledOk && _skipAllList.Contains(_currentWord))
                                {
                                    spelledOk = true;
                                }

                                if (!spelledOk && _namesEtcList.Contains(_currentWord))
                                {
                                    spelledOk = true;
                                }

                                if (!spelledOk)
                                {
                                    spelledOk = _spellChecker.Spell(_currentWord);
                                    if (!spelledOk)
                                    {
                                        if (!_abort)
                                        {
                                            _abort = true;
                                            Window.ShowProgress(_currentParagraphIndex, _subtitle);
                                            Window.ShowUnknownWord(_currentSpellCheckWord, _currentParagraph);
                                            return;
                                        }
                                    }
                                }
                            }
                        });

                });
        }


    }
}
