using System;
using AppKit;

namespace Nikse.SubtitleEdit.UILogic
{
	public static class MessageBox
	{
		public static void Show (string messageText)
		{
			var alert = new NSAlert () {
				AlertStyle = NSAlertStyle.Informational,
				InformativeText = string.Empty,
				MessageText = messageText,
			};
			alert.RunModal ();
		}

	}
}

