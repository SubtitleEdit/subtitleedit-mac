// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace Nikse.SubtitleEdit.Windows
{
	[Register ("MainWindow")]
	partial class MainWindow
	{
		[Outlet]
		AppKit.NSImageView _audioViz { get; set; }

		[Outlet]
		AppKit.NSBox _audioVizBox { get; set; }

		[Outlet]
		AppKit.NSButton _buttonAddWaveform { get; set; }

		[Outlet]
		AppKit.NSTextField _duration { get; set; }

		[Outlet]
		AppKit.NSStepper _durationStepper { get; set; }

		[Outlet]
		AppKit.NSSplitView _splitViewHor { get; set; }

		[Outlet]
		AppKit.NSSplitView _splitViewVert { get; set; }

		[Outlet]
		AppKit.NSTextField _startTime { get; set; }

		[Outlet]
		AppKit.NSStepper _startTimeStepper { get; set; }

		[Outlet]
		AppKit.NSTableView _subtitleTable { get; set; }

		[Outlet]
		AppKit.NSMenuItem _subtitleTableContextMenuDeleteLines { get; set; }

		[Outlet]
		AppKit.NSMenuItem _subtitletableContextMenuInsertAfter { get; set; }

		[Outlet]
		AppKit.NSMenuItem _subtitleTableContextMenuInsertBefore { get; set; }

		[Outlet]
		AppKit.NSMenuItem _subtitleTableContextMenuInsertText { get; set; }

		[Outlet]
		AppKit.NSMenuItem _subtitleTableContextMenuItalic { get; set; }

		[Outlet]
		AppKit.NSMenuItem _subtitleTableContextMenuMerge { get; set; }

		[Outlet]
		AppKit.NSMenuItem _subtitleTableContextMenuNormal { get; set; }

		[Outlet]
		AppKit.NSMenuItem _subtitletableContextMenuSplit { get; set; }

		[Outlet]
		AppKit.NSToolbarItem _toolbarShowAudioViz { get; set; }

		[Outlet]
		AppKit.NSToolbarItem _toolbarShowVideo { get; set; }

		[Outlet]
		AppKit.NSBox _videoBox { get; set; }

		[Outlet]
		AppKit.NSView _videoPlayerView { get; set; }

		[Outlet]
		AppKit.NSTextField subtitleText { get; set; }

		[Outlet]
		AppKit.NSComboBox toolbarEncodingComboBox { get; set; }

		[Outlet]
		AppKit.NSToolbarItem toolbarFind { get; set; }

		[Outlet]
		AppKit.NSToolbarItem toolbarHelp { get; set; }

		[Outlet]
		AppKit.NSToolbarItem toolbarNew { get; set; }

		[Outlet]
		AppKit.NSToolbarItem toolbarOpen { get; set; }

		[Outlet]
		AppKit.NSToolbarItem toolbarReplace { get; set; }

		[Outlet]
		AppKit.NSToolbarItem toolbarSave { get; set; }

		[Outlet]
		AppKit.NSToolbarItem toolbarSaveAs { get; set; }

		[Outlet]
		AppKit.NSToolbarItem toolbarSubtitleFormat { get; set; }

		[Outlet]
		AppKit.NSComboBox toolbarSubtitleFormatComboBox { get; set; }

		[Action ("toolbarNewClick:")]
		partial void toolbarNewClick (Foundation.NSObject sender);

		[Action ("toolbarOpenClick:")]
		partial void toolbarOpenClick (Foundation.NSObject sender);

		[Action ("toolbarSaveAsClick:")]
		partial void toolbarSaveAsClick (Foundation.NSObject sender);

		[Action ("toolbarSaveClick:")]
		partial void toolbarSaveClick (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (_videoBox != null) {
				_videoBox.Dispose ();
				_videoBox = null;
			}

			if (_audioVizBox != null) {
				_audioVizBox.Dispose ();
				_audioVizBox = null;
			}

			if (_toolbarShowAudioViz != null) {
				_toolbarShowAudioViz.Dispose ();
				_toolbarShowAudioViz = null;
			}

			if (_toolbarShowVideo != null) {
				_toolbarShowVideo.Dispose ();
				_toolbarShowVideo = null;
			}

			if (_audioViz != null) {
				_audioViz.Dispose ();
				_audioViz = null;
			}

			if (_buttonAddWaveform != null) {
				_buttonAddWaveform.Dispose ();
				_buttonAddWaveform = null;
			}

			if (_duration != null) {
				_duration.Dispose ();
				_duration = null;
			}

			if (_durationStepper != null) {
				_durationStepper.Dispose ();
				_durationStepper = null;
			}

			if (_splitViewHor != null) {
				_splitViewHor.Dispose ();
				_splitViewHor = null;
			}

			if (_splitViewVert != null) {
				_splitViewVert.Dispose ();
				_splitViewVert = null;
			}

			if (_startTime != null) {
				_startTime.Dispose ();
				_startTime = null;
			}

			if (_startTimeStepper != null) {
				_startTimeStepper.Dispose ();
				_startTimeStepper = null;
			}

			if (_subtitleTable != null) {
				_subtitleTable.Dispose ();
				_subtitleTable = null;
			}

			if (_subtitleTableContextMenuDeleteLines != null) {
				_subtitleTableContextMenuDeleteLines.Dispose ();
				_subtitleTableContextMenuDeleteLines = null;
			}

			if (_subtitletableContextMenuInsertAfter != null) {
				_subtitletableContextMenuInsertAfter.Dispose ();
				_subtitletableContextMenuInsertAfter = null;
			}

			if (_subtitleTableContextMenuInsertBefore != null) {
				_subtitleTableContextMenuInsertBefore.Dispose ();
				_subtitleTableContextMenuInsertBefore = null;
			}

			if (_subtitleTableContextMenuInsertText != null) {
				_subtitleTableContextMenuInsertText.Dispose ();
				_subtitleTableContextMenuInsertText = null;
			}

			if (_subtitleTableContextMenuItalic != null) {
				_subtitleTableContextMenuItalic.Dispose ();
				_subtitleTableContextMenuItalic = null;
			}

			if (_subtitleTableContextMenuMerge != null) {
				_subtitleTableContextMenuMerge.Dispose ();
				_subtitleTableContextMenuMerge = null;
			}

			if (_subtitleTableContextMenuNormal != null) {
				_subtitleTableContextMenuNormal.Dispose ();
				_subtitleTableContextMenuNormal = null;
			}

			if (_subtitletableContextMenuSplit != null) {
				_subtitletableContextMenuSplit.Dispose ();
				_subtitletableContextMenuSplit = null;
			}

			if (_videoPlayerView != null) {
				_videoPlayerView.Dispose ();
				_videoPlayerView = null;
			}

			if (subtitleText != null) {
				subtitleText.Dispose ();
				subtitleText = null;
			}

			if (toolbarEncodingComboBox != null) {
				toolbarEncodingComboBox.Dispose ();
				toolbarEncodingComboBox = null;
			}

			if (toolbarFind != null) {
				toolbarFind.Dispose ();
				toolbarFind = null;
			}

			if (toolbarHelp != null) {
				toolbarHelp.Dispose ();
				toolbarHelp = null;
			}

			if (toolbarNew != null) {
				toolbarNew.Dispose ();
				toolbarNew = null;
			}

			if (toolbarOpen != null) {
				toolbarOpen.Dispose ();
				toolbarOpen = null;
			}

			if (toolbarReplace != null) {
				toolbarReplace.Dispose ();
				toolbarReplace = null;
			}

			if (toolbarSave != null) {
				toolbarSave.Dispose ();
				toolbarSave = null;
			}

			if (toolbarSaveAs != null) {
				toolbarSaveAs.Dispose ();
				toolbarSaveAs = null;
			}

			if (toolbarSubtitleFormat != null) {
				toolbarSubtitleFormat.Dispose ();
				toolbarSubtitleFormat = null;
			}

			if (toolbarSubtitleFormatComboBox != null) {
				toolbarSubtitleFormatComboBox.Dispose ();
				toolbarSubtitleFormatComboBox = null;
			}
		}
	}
}
