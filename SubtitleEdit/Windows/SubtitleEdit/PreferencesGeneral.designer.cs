// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace SubtitleEdit
{
	[Register ("PreferencesGeneral")]
	partial class PreferencesGeneral
	{
		[Outlet]
		AppKit.NSButton _checkPromptDeleteLines { get; set; }

		[Outlet]
		AppKit.NSComboBox _comboDefaultFrameRate { get; set; }

		[Outlet]
		AppKit.NSPopUpButton _popUpDefaultFileEncoding { get; set; }

		[Outlet]
		AppKit.NSPopUpButton _popUpMaxCharsSec { get; set; }

		[Outlet]
		AppKit.NSPopUpButton _popUpMinGap { get; set; }

		[Outlet]
		AppKit.NSPopUpButton _popUpSingleLineMaxLength { get; set; }

		[Outlet]
		AppKit.NSPopUpButton _popUpUnbreakLinesShorterThan { get; set; }

		[Outlet]
		AppKit.NSStepper _stepperMaxDuration { get; set; }

		[Outlet]
		AppKit.NSStepper _stepperMinDuration { get; set; }

		[Outlet]
		AppKit.NSTextField _textMaxDuration { get; set; }

		[Outlet]
		AppKit.NSTextField _textMinDuration { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (_comboDefaultFrameRate != null) {
				_comboDefaultFrameRate.Dispose ();
				_comboDefaultFrameRate = null;
			}

			if (_popUpDefaultFileEncoding != null) {
				_popUpDefaultFileEncoding.Dispose ();
				_popUpDefaultFileEncoding = null;
			}

			if (_popUpSingleLineMaxLength != null) {
				_popUpSingleLineMaxLength.Dispose ();
				_popUpSingleLineMaxLength = null;
			}

			if (_popUpMaxCharsSec != null) {
				_popUpMaxCharsSec.Dispose ();
				_popUpMaxCharsSec = null;
			}

			if (_popUpMinGap != null) {
				_popUpMinGap.Dispose ();
				_popUpMinGap = null;
			}

			if (_popUpUnbreakLinesShorterThan != null) {
				_popUpUnbreakLinesShorterThan.Dispose ();
				_popUpUnbreakLinesShorterThan = null;
			}

			if (_textMaxDuration != null) {
				_textMaxDuration.Dispose ();
				_textMaxDuration = null;
			}

			if (_stepperMaxDuration != null) {
				_stepperMaxDuration.Dispose ();
				_stepperMaxDuration = null;
			}

			if (_textMinDuration != null) {
				_textMinDuration.Dispose ();
				_textMinDuration = null;
			}

			if (_stepperMinDuration != null) {
				_stepperMinDuration.Dispose ();
				_stepperMinDuration = null;
			}

			if (_checkPromptDeleteLines != null) {
				_checkPromptDeleteLines.Dispose ();
				_checkPromptDeleteLines = null;
			}
		}
	}
}
