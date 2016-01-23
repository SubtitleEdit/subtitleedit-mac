using System;
using Nikse.SubtitleEdit.Core;
using System.Text;

namespace Nikse.SubtitleEdit.UILogic
{
    public static class UIStringExtensions
    {
       
        public static string ToListViewString(this String str)
        {
            var sb = new StringBuilder();
            int count = 0;
            foreach (var s in str.SplitToLines())
            {
                if (count > 0)
                {
                    sb.Append(Configuration.Settings.General.ListViewLineSeparatorString);
                }
                sb.Append(s);
                count++;
            }
            return sb.ToString();
        }

        public static string RemoveWindowsShortCut(this String str)
        {
            return str.Replace("&&", "@@@@@@").Replace("&", string.Empty).Replace("@@@@@@", "&");
//
//            var sb = new StringBuilder(str.Length);
//            for (int i=0; i<str.Length; i++)
//            {
//                string ch = str.Substring(i, 1);
//                if (ch == "&" && !str.Substring(i).StartsWith("& ", StringComparison.InvariantCulture))
//                {
//                }
//                else
//                {
//                    sb.Append(ch);
//                }
//            }
//            return sb.ToString();
        }

    }
}

