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
using Nikse.SubtitleEdit.Core.SpellCheck;
using UILogic;

namespace Edit
{
    public partial class SpellCheckController : NSWindowController
    {

        public bool WasChanged { get; set; }

        private Subtitle _subtitle;
        ISubtitleParagraphShow _subtitleParagraphShow;
        private SpellChecker _spellChecker;
        private Paragraph _currentParagraph;
        private int _currentParagraphIndex;
        private string _currentWord;
        private SpellCheckWord _currentSpellCheckWord;
        private int _wordsIndex;
        private List<SpellCheckWord> _words;
        private List<string> _skipAllList = new List<string>();
        private bool _abort = false;
        private int _noOfAddedWords = 0;
        private int _noOfChangedWords = 0;
        private bool _autoFixNames;
        private List<UndoObject> _undoList = new List<UndoObject>();
        private SpellCheckWordLists _spellCheckWordLists;
        private Dictionary<string, string> _changeAllDictionary;

        public SpellCheckController(IntPtr handle)
            : base(handle)
        {
        }

        [Export("initWithCoder:")]
        public SpellCheckController(NSCoder coder)
            : base(coder)
        {
        }

        public SpellCheckController(Subtitle subtitle, ISubtitleParagraphShow subtitleParagraphShow)
            : base("SpellCheck")
        {
            _subtitle = subtitle;
            _subtitleParagraphShow = subtitleParagraphShow;
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
        }

        internal void LoadDictionaries(string language)
        {
            _skipAllList = new List<string>();
            _spellChecker = new SpellChecker(language);
            _spellCheckWordLists = new SpellCheckWordLists(Configuration.DictionariesFolder, "en", _spellChecker);
            _changeAllDictionary = new Dictionary<string, string>();
        }

        public void InitializeSpellCheck()
        {
            _skipAllList = new List<string>();
            _currentParagraphIndex = 0;
            _words = SpellChecker.Split(_subtitle.Paragraphs[0].Text);
            _wordsIndex = -1;
            LoadDictionaries("en");
            (Window as SpellCheck).InitializeLanguages(_spellChecker.GetLanguages());              
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

        public void ChangeWord(string newWord)
        {
            if (_currentSpellCheckWord.Text == newWord)
                return;

            _spellChecker.CorrectWord(newWord, _currentParagraph, _currentSpellCheckWord.Text, _currentSpellCheckWord.Index);
            _noOfChangedWords++;
            PrepareNextWord();
        }

        public void ChangeWordAll(string newWord)
        {
            if (_currentSpellCheckWord.Text == newWord)
                return;

            if (!_changeAllDictionary.ContainsKey(_currentWord))
                _changeAllDictionary.Add(_currentWord, newWord);
            _spellChecker.CorrectWord(newWord, _currentParagraph, _currentSpellCheckWord.Text, _currentSpellCheckWord.Index);
            _noOfChangedWords++;
            PrepareNextWord();
        }

        public void AddToNames(string newWord)
        {
            _spellCheckWordLists.AddName(newWord);
            if (string.Compare(newWord, _currentWord, StringComparison.OrdinalIgnoreCase) != 0)
                return; // don't prepare next word if change was more than just casing
            if (newWord != _currentWord)
            {
                _changeAllDictionary.Add(_currentWord, newWord);
                _spellChecker.CorrectWord(newWord, _currentParagraph, _currentSpellCheckWord.Text, _currentSpellCheckWord.Index);
            }
            PrepareNextWord();
        }

        public void AddToUserDictionary(string newWord)
        {
            if (_spellCheckWordLists.AddUserWord(newWord))
                _noOfAddedWords++;
            PrepareNextWord();            
        }

        public void SetAutoFixState(bool autoFixNames)
        {
            _autoFixNames = autoFixNames;
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
                                    _currentParagraph = _subtitle.Paragraphs[_currentParagraphIndex];
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
                                        MessageBox.Show("Spell check completed" + Environment.NewLine +
                                            Environment.NewLine +
                                            "Changed words: " + _noOfChangedWords);
                                        Close();
                                        return;
                                    }
                                }

                                var spelledOk = string.IsNullOrWhiteSpace(_currentWord);

                                if (!spelledOk && _skipAllList.Contains(_currentWord))
                                {
                                    spelledOk = true;
                                }

                                if (!spelledOk && _spellCheckWordLists.HasName(_currentWord))
                                {
                                    spelledOk = true;
                                }

                                if (!spelledOk && _spellCheckWordLists.HasUserWord(_currentWord))
                                {
                                    spelledOk = true;
                                }

                                if (!spelledOk && _spellCheckWordLists.HasNameExtended(_currentWord, _currentParagraph.Text))
                                {
                                    spelledOk = true;
                                }

                                if (!spelledOk && _changeAllDictionary.ContainsKey(_currentWord))
                                {
                                    _noOfChangedWords++;
                                    _spellChecker.CorrectWord(_changeAllDictionary[_currentWord], _currentParagraph, _currentWord, 0);
                                    spelledOk = true;
                                }

                                if (!spelledOk && _changeAllDictionary.ContainsKey(_currentWord.Trim('\'')))
                                {
                                    _noOfChangedWords++;
                                    _spellChecker.CorrectWord(_changeAllDictionary[_currentWord], _currentParagraph, _currentWord.Trim('\''), 0);
                                    spelledOk = true;
                                }

                                if (!spelledOk && _wordsIndex >= 00 && _wordsIndex < _words.Count && _spellCheckWordLists.IsWordInUserPhrases(_wordsIndex, _words))
                                {
                                    spelledOk = true;
                                }
                                    
                                if (!spelledOk)
                                {
                                    spelledOk = _spellChecker.DoSpell(_currentWord);

                                    if (!spelledOk)
                                    {
                                        string removeUnicode = _currentWord.Replace(Char.ConvertFromUtf32(0x200b), string.Empty); // zero width space
                                        removeUnicode = removeUnicode.Replace(Char.ConvertFromUtf32(0x2060), string.Empty); // word joiner
                                        removeUnicode = removeUnicode.Replace(Char.ConvertFromUtf32(0xfeff), string.Empty); // zero width no-break space
                                        spelledOk = _spellChecker.DoSpell(removeUnicode);
                                    }


                                    // auto fix name
                                    if (_autoFixNames && !spelledOk)
                                    {
                                        var suggestions = _spellChecker.Suggest(_currentWord);
                                        if (!spelledOk && _autoFixNames && _currentWord.Length > 1 && suggestions.Contains(char.ToUpper(_currentWord[0]) + _currentWord.Substring(1)))
                                        {
                                            var newWord = char.ToUpper(_currentWord[0]) + _currentWord.Substring(1);
                                            _noOfChangedWords++;
                                            _spellChecker.CorrectWord(newWord, _currentParagraph, _currentWord, 0);
                                            spelledOk = true;
                                        }
                                        if (!spelledOk && _autoFixNames && _currentWord.Length > 1)
                                        {
                                            if (_currentWord.Length > 3 && suggestions.Contains(_currentWord.ToUpper()))
                                            { // does not work well with two letter words like "da" and "de" which get auto-corrected to "DA" and "DE"
                                                var newWord = _currentWord.ToUpper();
                                                _noOfChangedWords++;
                                                _spellChecker.CorrectWord(newWord, _currentParagraph, _currentWord, 0);
                                                spelledOk = true;
                                            }
                                            if (!spelledOk && _spellCheckWordLists.HasName(char.ToUpper(_currentWord[0]) + _currentWord.Substring(1)))
                                            {
                                                var newWord = char.ToUpper(_currentWord[0]) + _currentWord.Substring(1);
                                                _noOfChangedWords++;
                                                _spellChecker.CorrectWord(newWord, _currentParagraph, _currentWord, 0);
                                                spelledOk = true;
                                            }
                                            if (!spelledOk && _currentWord.Length > 3 && _currentWord.StartsWith("mc", StringComparison.InvariantCultureIgnoreCase) && _spellCheckWordLists.HasName(char.ToUpper(_currentWord[0]) + _currentWord.Substring(1, 1) + char.ToUpper(_currentWord[2]) + _currentWord.Remove(0, 3)))
                                            {
                                                var newWord = char.ToUpper(_currentWord[0]) + _currentWord.Substring(1, 1) + char.ToUpper(_currentWord[2]) + _currentWord.Remove(0, 3);
                                                _noOfChangedWords++;
                                                _spellChecker.CorrectWord(newWord, _currentParagraph, _currentWord, 0);
                                                spelledOk = true;
                                            }
                                            if (!spelledOk && _spellCheckWordLists.HasName(_currentWord.ToUpperInvariant()))
                                            {
                                                var newWord = _currentWord.ToUpperInvariant();
                                                _noOfChangedWords++;
                                                _spellChecker.CorrectWord(newWord, _currentParagraph, _currentWord, 0);
                                                spelledOk = true;
                                            }
                                        }
                                    }


                                    if (!spelledOk)
                                    {
                                        if (!_abort)
                                        {
                                            _abort = true;
                                            Window.ShowProgress(_currentParagraphIndex, _subtitle);
                                            if (_subtitleParagraphShow != null)
                                            {
                                                _subtitleParagraphShow.SubtitleParagraphShow(_currentParagraphIndex); // show current line in main window
                                            }
                                            Window.ShowSuggestions(_spellChecker.Suggest(_currentWord));
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
