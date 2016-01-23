using System;
using System.Text.RegularExpressions;
using System.IO;

namespace File
{
    public class AutoBackupItem
    {
        public string FullPath { get; set; }
        public string DisplayDate { get; set; }
        public string FileName { get; set; }
        public string Extension { get; set; }
        public string DisplaySize { get; set; }

        public AutoBackupItem(string fileName)
        {
            FullPath = fileName;
            DisplayDate = Path.GetFileName(fileName).Substring(0, 19).Replace('_', ' ');
            DisplayDate = DisplayDate.Remove(13, 1).Insert(13, ":");
            DisplayDate = DisplayDate.Remove(16, 1).Insert(16, ":");

            FileName = Path.GetFileName(fileName).Remove(0, 20);
            if (FileName == "srt")
                FileName = "Untitled.srt";

            Extension = Path.GetExtension(fileName);

            try
            {
                DisplaySize = new FileInfo(fileName).Length + " bytes";
            }
            catch
            {
                DisplaySize = "?";
            }
        }

    }
}

