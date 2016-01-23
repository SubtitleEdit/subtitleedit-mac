using System;
using System.Collections.Generic;
using AppKit;
using Nikse.SubtitleEdit.Core;
using System.Linq;

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
   //         var x = new Foundation.NSOrthography(new Foundation.NSCoder());
     //       nint wordCount;
            var result = _nativeSpellChecker.CheckSpelling(word, 0);
 //           var results = _nativeSpellChecker.CheckString(word, new Foundation.NSRange(0, word.Length), new Foundation.NSTextCheckingTypes(), new Foundation.NSDictionary(), 0, out x, out wordCount);
            return result.Length == 0;
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

    }

}

