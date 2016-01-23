using System;

namespace Nikse.SubtitleEdit.UILogic
{
    public interface IOpenSubtitle
    {
        bool OpenSubtitlePromptForChanges(string fileName, bool requireSaveAs);
    }
}

