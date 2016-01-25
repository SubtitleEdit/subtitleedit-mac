using System;
using System.Collections.Generic;
using AppKit;
using Nikse.SubtitleEdit.Core;
using System.Linq;
using System.Text;
using Foundation;

namespace SpellCheck
{

    public class SpellChecker : IDisposable
    {
        NSSpellChecker _nativeSpellChecker;

        public  SpellChecker(string language)
        {
            _nativeSpellChecker = new NSSpellChecker();
        }

        public bool Spell(string word)
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
            if (word.Length > 1 && word.StartsWith('I') && !suggestions.Contains("l" + word.Substring(1)) && Spell("l" + word.Substring(1)))
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


    }

}

