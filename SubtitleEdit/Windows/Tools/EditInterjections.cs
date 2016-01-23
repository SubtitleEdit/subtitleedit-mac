using System;

using Foundation;
using AppKit;
using Nikse.SubtitleEdit.Core;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nikse.SubtitleEdit.UILogic;

namespace Tools
{
    public partial class EditInterjections : NSWindow
    {

        private List<string> _interjections;

        public EditInterjections(IntPtr handle)
            : base(handle)
        {
        }

        [Export("initWithCoder:")]
        public EditInterjections(NSCoder coder)
            : base(coder)
        {
        }

        private void InitializeTable(NSTableView table)
        {
            var columns = table.TableColumns();
            columns[0].SetIdentifier(StringListTableDelegate.CellIdentifiers[0]);
            columns[0].MinWidth = 50;
            columns[0].MaxWidth = 20000;
            columns[0].Width = 2060;
            columns[0].Title = Configuration.Settings.Language.Interjections.Title;

            _interjections = GetInterjections();
            ShowInterjections(_interjections);
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();

            this.WillClose += (object sender, EventArgs e) => 
            { 
                NSApplication.SharedApplication.StopModal(); 
            };

            _buttonOk.Activated += (object sender, EventArgs e) => 
            {
                Configuration.Settings.Tools.Interjections = BuildInterjectionsString();
                (WindowController as EditInterjectionsController).OkPressed();
                Close();
            };  

            _buttonCancel.Activated += (object sender, EventArgs e) => 
            {
                Close();
            };  

            _buttonAdd.Activated += (object sender, EventArgs e) => 
                {
                    string s = _addText.StringValue.Trim();
                    if (s.Length == 0)
                    {
                        return;
                    }
                    if (_interjections.Contains(s))
                    {
                        return;
                    }
                    _interjections.Add(s);
                    _interjections = _interjections.OrderBy(p=>p).ToList();
                    _addText.StringValue = string.Empty;
                    ShowInterjections(_interjections);
                };  

            _buttonRemove.Activated += (object sender, EventArgs e) => 
                {
                    var index = (int)_interjectionsTable.SelectedRow;
                    if (index < 0 || index >= _interjectionsTable.RowCount)
                    {
                        return;
                    }
                    _interjections.RemoveAt(index);
                    ShowInterjections(_interjections);
                    if (index >= _interjections.Count)
                    {
                        index--;
                    }
                    if (index >= 0)
                    {
                        _interjectionsTable.SelectRow((nint)index, false);
                    }

                };  

            var l = Configuration.Settings.Language.Interjections;
            Title = l.Title;
            _buttonOk.Title = Configuration.Settings.Language.General.Ok;
            _buttonCancel.Title = Configuration.Settings.Language.General.Cancel;


            InitializeTable(_interjectionsTable);
        }

        private string BuildInterjectionsString()
        {
            var sb = new StringBuilder();
            int count = 0;
            foreach (string s in _interjections)
            {
                if (count > 0)
                {
                    sb.Append(";");
                }
                sb.Append(s);
                count++;
            }
            return sb.ToString();
        }

        private List<string> GetInterjections()
        {
            var interjections = new List<string>();
            string[] arr = Configuration.Settings.Tools.Interjections.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string s in arr)
            {
                interjections.Add(s.Trim());
            }
            return interjections;
        }

        public void ShowInterjections(List<string> interjections)
        {
            var ds = new StringListTableDataSource(interjections.OrderBy(p=>p));
            _interjectionsTable.DataSource = ds;
            _interjectionsTable.Delegate = new StringListTableDelegate(ds, null);
        }

    }
}
