// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace Edit
{
	[Register ("MultipleReplace")]
	partial class MultipleReplace
	{
		[Outlet]
		AppKit.NSButton _addButton { get; set; }

		[Outlet]
		AppKit.NSButton _buttonCancel { get; set; }

		[Outlet]
		AppKit.NSButton _buttonOk { get; set; }

		[Outlet]
		AppKit.NSTextField _findLabel { get; set; }

		[Outlet]
		AppKit.NSTextField _findText { get; set; }

		[Outlet]
		AppKit.NSPopUpButton _findType { get; set; }

		[Outlet]
		AppKit.NSBox _previewBox { get; set; }

		[Outlet]
		AppKit.NSTableView _previewTable { get; set; }

		[Outlet]
		AppKit.NSTextField _replaceLabel { get; set; }

		[Outlet]
		AppKit.NSTextField _replaceText { get; set; }

		[Outlet]
		AppKit.NSBox _rulesBox { get; set; }

		[Outlet]
		AppKit.NSMenu _rulesContextMenu { get; set; }

		[Outlet]
		AppKit.NSMenuItem _rulesContextMenuDelete { get; set; }

		[Outlet]
		AppKit.NSMenuItem _rulesContextMenuMoveDown { get; set; }

		[Outlet]
		AppKit.NSMenuItem _rulesContextMenuMoveUp { get; set; }

		[Outlet]
		AppKit.NSTableView _rulesTable { get; set; }

		[Outlet]
		AppKit.NSButton _updateButton { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (_addButton != null) {
				_addButton.Dispose ();
				_addButton = null;
			}

			if (_buttonCancel != null) {
				_buttonCancel.Dispose ();
				_buttonCancel = null;
			}

			if (_buttonOk != null) {
				_buttonOk.Dispose ();
				_buttonOk = null;
			}

			if (_findLabel != null) {
				_findLabel.Dispose ();
				_findLabel = null;
			}

			if (_findText != null) {
				_findText.Dispose ();
				_findText = null;
			}

			if (_findType != null) {
				_findType.Dispose ();
				_findType = null;
			}

			if (_previewBox != null) {
				_previewBox.Dispose ();
				_previewBox = null;
			}

			if (_previewTable != null) {
				_previewTable.Dispose ();
				_previewTable = null;
			}

			if (_replaceLabel != null) {
				_replaceLabel.Dispose ();
				_replaceLabel = null;
			}

			if (_replaceText != null) {
				_replaceText.Dispose ();
				_replaceText = null;
			}

			if (_rulesBox != null) {
				_rulesBox.Dispose ();
				_rulesBox = null;
			}

			if (_rulesTable != null) {
				_rulesTable.Dispose ();
				_rulesTable = null;
			}

			if (_updateButton != null) {
				_updateButton.Dispose ();
				_updateButton = null;
			}

			if (_rulesContextMenu != null) {
				_rulesContextMenu.Dispose ();
				_rulesContextMenu = null;
			}

			if (_rulesContextMenuDelete != null) {
				_rulesContextMenuDelete.Dispose ();
				_rulesContextMenuDelete = null;
			}

			if (_rulesContextMenuMoveUp != null) {
				_rulesContextMenuMoveUp.Dispose ();
				_rulesContextMenuMoveUp = null;
			}

			if (_rulesContextMenuMoveDown != null) {
				_rulesContextMenuMoveDown.Dispose ();
				_rulesContextMenuMoveDown = null;
			}
		}
	}
}
