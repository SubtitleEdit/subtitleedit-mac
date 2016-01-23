using System;
using Nikse.SubtitleEdit.Core.Enums;
using Nikse.SubtitleEdit.Core;
using System.Text.RegularExpressions;
using Nikse.SubtitleEdit.UILogic;

namespace UILogic
{
    public class FindReplaceInfo
    {
        public string FindText { get; set; }
        public int FindTextLength { get; set; }
        public string ReplaceText { get; set; }
        public int CurrentLineIndex { get; set; }
        public int CurrentStringIndex { get; set; }
        public FindType FindType { get; set; }
        public bool Success { get; set; }

        public FindReplaceInfo()
        {
        }

        private static bool NotLoopDone(bool increment, int lineNumber, Subtitle subtitle)
        {
            if (increment)
            {
                return lineNumber < subtitle.Paragraphs.Count;
            }
            return lineNumber >= 0;
        }

        public void PerformFind(Subtitle subtitle, bool increment = true)
        {
            int incrementValue = 1;
            if (!increment)
            {
                incrementValue = -1;
            }
            Success = false;
            if (FindType == Nikse.SubtitleEdit.Core.Enums.FindType.Normal)
            {
                for (int i = CurrentLineIndex; NotLoopDone(increment, i, subtitle); i += incrementValue)
                {       
                    var text = subtitle.Paragraphs[i].Text;
                    if (CurrentStringIndex >= text.Length)
                        CurrentStringIndex = 0;
                    int index = text.IndexOf(FindText, CurrentStringIndex, StringComparison.InvariantCultureIgnoreCase);
                    CurrentStringIndex = 0;
                    if (index >= 0)
                    {
                        CurrentLineIndex = i;
                        CurrentStringIndex = index;
                        FindTextLength = FindText.Length;
                        Success = true;
                        return;
                    }
                }
            }
            else if (FindType == Nikse.SubtitleEdit.Core.Enums.FindType.CaseSensitive)
            {
                for (int i = CurrentLineIndex; NotLoopDone(increment, i, subtitle); i += incrementValue)
                {       
                    var text = subtitle.Paragraphs[i].Text;
                    if (CurrentStringIndex >= text.Length)
                        CurrentStringIndex = 0;
                    int index = subtitle.Paragraphs[i].Text.IndexOf(FindText, StringComparison.InvariantCulture);
                    CurrentStringIndex = 0;

                    if (index >= 0)
                    {
                        CurrentLineIndex = i;
                        CurrentStringIndex = index;
                        FindTextLength = FindText.Length;
                        Success = true;
                        return;
                    }
                }
            }
            else
            {
                try
                {
                    var regEx = new Regex(FindText, RegexOptions.Compiled);
                    for (int i = CurrentLineIndex; NotLoopDone(increment, i, subtitle); i += incrementValue)
                    {       
                        var text = subtitle.Paragraphs[i].Text;
                        if (CurrentStringIndex >= text.Length)
                            CurrentStringIndex = 0;                       
                        Match match = regEx.Match(subtitle.Paragraphs[i].Text, CurrentStringIndex);
                        CurrentStringIndex = 0;                       
                        if (match.Success)
                        {
                            string groupName = Utilities.GetRegExGroup(FindText);
                            if (groupName != null && match.Groups[groupName] != null && match.Groups[groupName].Success)
                            {
                                FindTextLength = match.Groups[groupName].Length;
                                CurrentStringIndex = match.Groups[groupName].Index;
                            }
                            else
                            {
                                FindTextLength = match.Length;
                                CurrentStringIndex = match.Index;
                            }
                            Success = true;
                            return;
                        }
                    }
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.Message);
                }
            }
            FindTextLength = 0;
        }

        public void PerformReplace(Subtitle subtitle)
        {
            PerformFind(subtitle);
            if (Success)
            {
                subtitle.Paragraphs[CurrentLineIndex].Text = subtitle.Paragraphs[CurrentLineIndex].Text.Remove(CurrentStringIndex, FindTextLength).Insert(CurrentStringIndex, ReplaceText);
            }
        }

        public void PerformReplaceAll(Subtitle subtitle)
        {
            do
            {
                PerformReplace(subtitle);
            }
            while (Success);
        }

    }
}

