using System;
using System.Collections.Generic;
using AppKit;
using Nikse.SubtitleEdit.Core;
using Nikse.SubtitleEdit.Core.SpellCheck;
using Nikse.SubtitleEdit.Core.Interfaces;
using System.Linq;
using System.Text;
using Foundation;

namespace SpellCheck
{

    public class SpellChecker : IDisposable, IDoSpell
    {
        NSSpellChecker _nativeSpellChecker;

        public  SpellChecker(string language)
        {
            _nativeSpellChecker = new NSSpellChecker();
        }

        public bool DoSpell(string word)
        { 
            nint wordCount = 0;
            var res = _nativeSpellChecker.CheckSpelling(word, 0, "en",true, 0, out wordCount); 
            return res.Location != 0;
        }

        public List<string> Suggest(string word)
        {
            var list = _nativeSpellChecker.GuessesForWordRange(new Foundation.NSRange(0, word.Length), word, CurrentLanguage, 0).ToList();
            AddIShouldBeLowercaseLSuggestion(list, word);
            return list;
        }

        public virtual void Dispose()
        {
            _nativeSpellChecker.Dispose();
        }

        protected void AddIShouldBeLowercaseLSuggestion(List<string> suggestions, string word)
        {
            if (suggestions == null)
            {
                return;
            }

            // "I" can often be an ocr bug - should really be "l"
            if (word.Length > 1 && word.StartsWith('I') && !suggestions.Contains("l" + word.Substring(1)) && DoSpell("l" + word.Substring(1)))
            {
                suggestions.Add("l" + word.Substring(1));
            }
        }

        public List<string> GetLanguages()
        {            
            return _nativeSpellChecker.AvailableLanguages.ToList();
        }

        public string CurrentLanguage
        {
            get
            { 
                return _nativeSpellChecker.Language;
            }
            set
            { 
                _nativeSpellChecker.Language = value;
            }
        }


        public const string WordSplitChars = " -.,?!:;\"“”()[]{}|<>/+\r\n¿¡…—–♪♫„“";



        public static List<SpellCheckWord> Split(string s)
        {
            var list = new List<SpellCheckWord>();
            var sb = new StringBuilder();
            for (int i = 0; i < s.Length; i++)
            {
                if (SpellChecker.WordSplitChars.Contains(s[i]))
                {
                    if (sb.Length > 0)
                        list.Add(new SpellCheckWord { Text = sb.ToString(), Index = i - sb.Length });
                    sb.Clear();
                }
                else
                {
                    sb.Append(s[i]);
                }
            }
            if (sb.Length > 0)
                list.Add(new SpellCheckWord { Text = sb.ToString(), Index = s.Length - 1 - sb.Length });
            return list;
        }


        private int GetPositionFromWordIndex(string text, int wordIndex)
        {
            var sb = new StringBuilder();
            int index = -1;
            for (int i = 0; i < text.Length; i++)
            {
                if (SpellCheckWordLists.SplitChars.Contains(text[i]))
                {
                    if (sb.Length > 0)
                    {
                        index++;
                        if (index == wordIndex)
                        {
                            int pos = i - sb.Length;
                            if (pos > 0)
                                pos--;
                            if (pos >= 0)
                                return pos;
                        }
                    }
                    sb.Clear();
                }
                else
                {
                    sb.Append(text[i]);
                }
            }
            if (sb.Length > 0)
            {
                index++;
                if (index == wordIndex)
                {
                    int pos = text.Length - 1 - sb.Length;
                    if (pos >= 0)
                        return pos;
                }
            }
            return 0;
        }

        public void CorrectWord(string changeWord, Paragraph p, string oldWord, int wordIndex)
        {
            if (oldWord != changeWord)
            {
                int startIndex = p.Text.IndexOf(oldWord, StringComparison.Ordinal);
                if (wordIndex >= 0)
                {
                    startIndex = p.Text.IndexOf(oldWord, GetPositionFromWordIndex(p.Text, wordIndex), StringComparison.Ordinal);
                }
                while (startIndex >= 0 && startIndex < p.Text.Length && p.Text.Substring(startIndex).Contains(oldWord))
                {
                    bool startOk = startIndex == 0 ||
                        p.Text[startIndex - 1] == ' ' ||
                        p.Text[startIndex - 1] == '>' ||
                        p.Text[startIndex - 1] == '"' ||
                        p.Text[startIndex - 1] == '\'' ||
                        startIndex == p.Text.Length - oldWord.Length ||
                        Environment.NewLine.EndsWith(p.Text[startIndex - 1]);
                    if (startOk)
                    {
                        int end = startIndex + oldWord.Length;
                        if (end <= p.Text.Length)
                        {
                            if (end == p.Text.Length || (" ,.!?:;')<\"-]}%&$£" + Environment.NewLine).Contains(p.Text[end]))
                                p.Text = p.Text.Remove(startIndex, oldWord.Length).Insert(startIndex, changeWord);
                        }
                    }
                    if (startIndex + 2 >= p.Text.Length)
                        startIndex = -1;
                    else
                        startIndex = p.Text.IndexOf(oldWord, startIndex + 2, StringComparison.Ordinal);

                    // stop if using index
                    if (wordIndex >= 0)
                        startIndex = -1;
                }
            }
        }


    }

}

