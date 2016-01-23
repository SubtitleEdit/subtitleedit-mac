using System;

using Foundation;
using AppKit;
using Nikse.SubtitleEdit.Core;
using System.Text.RegularExpressions;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace File
{
    public partial class RestoreAutoBackup : NSWindow
    {
        private static Regex fileNamePattern = new Regex(@"^\d\d\d\d-\d\d-\d\d_\d\d-\d\d-\d\d", RegexOptions.Compiled);

        public RestoreAutoBackup(IntPtr handle)
            : base(handle)
        {
        }

        [Export("initWithCoder:")]
        public RestoreAutoBackup(NSCoder coder)
            : base(coder)
        {
        }

        private void InitializeTable(NSTableView table)
        {
            var columns = table.TableColumns();
            columns[0].SetIdentifier(AutoBackupTableDelegate.CellIdentifiers[0]);
            columns[0].MinWidth = 110;
            columns[0].MaxWidth = 200;
            columns[0].Width = 140;
            columns[0].Title = Configuration.Settings.Language.RestoreAutoBackup.DateAndTime;
            columns[1].SetIdentifier(AutoBackupTableDelegate.CellIdentifiers[1]);
            columns[1].MinWidth = 140;
            columns[1].MaxWidth = 10200;
            columns[1].Width = 260;
            columns[1].Title = Configuration.Settings.Language.RestoreAutoBackup.FileName;
            table.AddColumn(new NSTableColumn(AutoBackupTableDelegate.CellIdentifiers[2]) 
            {
                MinWidth = 50,
                MaxWidth = 200,
                Width = 70,
                Title = Configuration.Settings.Language.RestoreAutoBackup.Extension,
            });
            table.AddColumn(new NSTableColumn(AutoBackupTableDelegate.CellIdentifiers[3]) 
            {
                MinWidth = 60,
                MaxWidth = 150,
                Width = 90,
                Title = Configuration.Settings.Language.General.Size,
            });


            //2011-12-13_20-19-18_title
            if (Directory.Exists(Configuration.AutoBackupFolder))
            {
                var autoBackupItems = new List<AutoBackupItem>();
                var files = Directory.GetFiles(Configuration.AutoBackupFolder, "*.*");
                foreach (string fileName in files)
                {
                    if (fileNamePattern.IsMatch(Path.GetFileName(fileName)))
                    {
                        autoBackupItems.Add(new AutoBackupItem(fileName));
                    }
                }
                autoBackupItems = autoBackupItems.OrderBy(p => p.DisplayDate).ToList();
                var ds = new AutoBackupTableDataSource(autoBackupItems);
                table.DataSource = ds;
                table.Delegate = new AutoBackupTableDelegate(ds);
            }
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();

            WillClose += (object sender, EventArgs e) => 
            { 
                NSApplication.SharedApplication.StopModal(); 
            };

            Title = Configuration.Settings.Language.RestoreAutoBackup.Title;
            buttonOk.StringValue = Configuration.Settings.Language.General.Ok;
            buttonCancel.StringValue = Configuration.Settings.Language.General.Cancel;

            InitializeTable(_autoBackupTable);

            if (_autoBackupTable.RowCount > 0)
            {
                //                labelOpenContainingFolder.TextColor = NSColor.Blue; //
//                var attributes = new CoreText.CTStringAttributes {
//                                ForegroundColor = new CoreGraphics.CGColor(0,0, 255),
//                    UnderlineStyle = CoreText.CTUnderlineStyle.Single
//                };
//                buttonOpenContainingFolder.AttributedTitle  = new NSAttributedString(Configuration.Settings.Language.Main.Menu.File.OpenContainingFolder, attributes); 
                buttonOpenContainingFolder.Title = Configuration.Settings.Language.Main.Menu.File.OpenContainingFolder;
                buttonOpenContainingFolder.Activated += (object sender, EventArgs e) => 
                {
                    Process.Start(Configuration.AutoBackupFolder);
                };
            }
            else
            {
                buttonOpenContainingFolder.Hidden = true;
            }

            buttonOk.Activated += (object sender, EventArgs e) => 
            {
                string fileName = null;
                var index = _autoBackupTable.SelectedRow;
                if (index >= 0)
                {
                        fileName = (_autoBackupTable.DataSource as AutoBackupTableDataSource).DataSource[(int)index].FullPath;
                }
                (WindowController as RestoreAutoBackupController).OkPressed(fileName);
                Close();        
            };

            buttonCancel.Activated += (object sender, EventArgs e) => 
            {
                Close();
            };

        }
    }
}
