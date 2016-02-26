using System;

using Foundation;
using AppKit;
using System.Diagnostics;
using Nikse.SubtitleEdit.Core;
using CoreGraphics;

namespace Nikse.SubtitleEdit.Windows.Help
{
	public partial class AboutController : NSWindowController
	{
		private readonly LanguageStructure.About _language = Configuration.Settings.Language.About;
		private readonly LanguageStructure.General _languageGeneral = Configuration.Settings.Language.General;

		public AboutController (IntPtr handle) : base (handle)
		{
		}

		[Export ("initWithCoder:")]
		public AboutController (NSCoder coder) : base (coder)
		{
		}

		public AboutController () : base ("About")
		{
		}

		public override void AwakeFromNib ()
		{
			base.AwakeFromNib ();

			this.Window.Title = _language.Title + " - " + (IntPtr.Size * 8) + "-bit";
			buttonOk.StringValue = _languageGeneral.Ok;
			buttonDonate.StringValue = "Donate";

			string[] versionInfo = Utilities.AssemblyVersion.Split('.');
			string revisionNumber = "0";
			if (versionInfo.Length >= 4)
				revisionNumber = versionInfo[3];
			if (revisionNumber == "0")
			{
				var description = Utilities.AssemblyDescription;
				if (description != null && description.Length > 7)
					revisionNumber = Utilities.AssemblyDescription.Substring(0, 7);
				labelTitle.StringValue = String.Format("{0} {1}.{2}.{3}, {4}", _languageGeneral.Title, versionInfo[0], versionInfo[1], versionInfo[2], revisionNumber);
			}
			else
			{
				labelTitle.StringValue = String.Format("{0} {1}.{2}.{3}, build {4}", _languageGeneral.Title, versionInfo[0], versionInfo[1], versionInfo[2], revisionNumber);
			}

            labelTitle.StringValue = "Subtitle Edit Mac alpha 1";

			string aboutText = _language.AboutText1.TrimEnd() + Environment.NewLine +
				Environment.NewLine +
				_languageGeneral.TranslatedBy.Trim();
			while (aboutText.Contains("\n ") || aboutText.Contains("\n\t"))
			{
				aboutText = aboutText.Replace("\n ", "\n");
				aboutText = aboutText.Replace("\n\t", "\n");
			}
			labelDescription.StringValue = aboutText;           
		}

		public new About Window 
        {
			get { return (About)base.Window; }
		}

		partial void buttonOkClick (NSObject sender)
		{            
            Window.Close();
		}

		partial void buttonDonateClick (NSObject sender)
		{
			labelTitle.SizeToFit();
			Process.Start("http://www.nikse.dk/Donate");
		}                      

	}
}
