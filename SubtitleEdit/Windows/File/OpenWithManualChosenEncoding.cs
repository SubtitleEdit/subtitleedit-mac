using System;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using AppKit;
using Foundation;
using Nikse.SubtitleEdit.Core;
using MacLibSe;

namespace File
{
    public partial class OpenWithManualChosenEncoding : NSWindow
    {
        List<Encoding> _allEncodings = new List<Encoding>();

        public OpenWithManualChosenEncoding(IntPtr handle)
            : base(handle)
        {
        }

        [Export("initWithCoder:")]
        public OpenWithManualChosenEncoding(NSCoder coder)
            : base(coder)
        {
        }

        private Encoding CurrentEncoding
        {
            get
            { 
                try
                {
                    var index = (int)_encodingTable.SelectedRow;
                    var encoding = (_encodingTable.DataSource as EncodingTableDataSource).Source[index];
                    return encoding;
                }
                catch
                {
                    return Encoding.UTF8;
                }
            }
        }

        public void ShowPreview(byte[] buffer)
        {
            var encoding = CurrentEncoding;
            _previewText.Value = CurrentEncoding.GetString(buffer);
            _previewLabe.StringValue = Configuration.Settings.Language.General.Preview + " - " + encoding.EncodingName;
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();

            WillClose += (object sender, EventArgs e) => 
            { 
                NSApplication.SharedApplication.StopModal(); 
            };

            Title = Configuration.Settings.Language.ChooseEncoding.Title;

            _previewText.Editable = false;
            _previewText.RichText = false;

            _buttonOk.Activated += (object sender, EventArgs e) => 
            {
                (WindowController as OpenWithManualChosenEncodingController).OkPressed(CurrentEncoding);
                Close();
            };  

            _buttonCancel.Activated += (object sender, EventArgs e) => 
            {
                Close();
            };  
                    
            var columns = _encodingTable.TableColumns();
            columns[0].SetIdentifier(EncodingTableDelegate.CellIdentifiers[0]);
            columns[0].MinWidth = 20;
            columns[0].MaxWidth = 100;
            columns[0].Width = 40;
            columns[0].Title = "Code page";
            columns[1].SetIdentifier(EncodingTableDelegate.CellIdentifiers[1]);
            columns[1].MinWidth = 50;
            columns[1].MaxWidth = 200;
            columns[1].Width = 90;
            columns[1].Title = "Web name";
            _encodingTable.AddColumn(new NSTableColumn(EncodingTableDelegate.CellIdentifiers[2]) 
                {
                    MinWidth = 50,
                    MaxWidth = 99200,
                    Width = 9000,
                    Title = "Name",
                });

            _allEncodings = new List<Encoding>();
            _allEncodings.Add(Encoding.UTF8);
            foreach (var ei in EncodingHelper.GetEncodings())
            {
                try
                {
                    var encoding = ei.GetEncoding();
                    if (ei.Name != Encoding.UTF8.BodyName && ei.CodePage >= 949 && !encoding.EncodingName.Contains("EBCDIC") && ei.CodePage != 1047) 
                    {
                        _allEncodings.Add(encoding);
                    }
                }
                catch
                {
                }
            }
            ApplySearchFilter();
            SelectEncoding((WindowController as OpenWithManualChosenEncodingController).ChosenEcoding);

            _searchText.Delegate = new OpenWithManualChoosenEncodingSearchText(WindowController as OpenWithManualChosenEncodingController);
        }

        private void SelectEncoding(Encoding encoding)
        {
            var encodings = (_encodingTable.DataSource as EncodingTableDataSource).Source;
            for (int i = 0; i < encodings.Count; i++)
            {
                var item = encodings[i];
                if (item.CodePage == encoding.CodePage)
                {
                    _encodingTable.SelectRow((nint)i, true);
                    break;
                }
            }
        }

        public void ApplySearchFilter()
        {
            var searchText = _searchText.StringValue.Trim();
            List<Encoding> encodings;
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                encodings = _allEncodings.Where(p=>p.CodePage.ToString().Contains(searchText) ||
                                                   p.EncodingName.ToLowerInvariant().Contains(searchText.ToLower()) ||
                                                   p.WebName.ToLowerInvariant().Contains(searchText.ToLower())).ToList();
            }
            else
            {
                encodings = _allEncodings;
            }
            var ds = new EncodingTableDataSource (encodings);
            _encodingTable.DataSource = ds;
            _encodingTable.Delegate = new EncodingTableDelegate (ds, WindowController as OpenWithManualChosenEncodingController); 
            _encodingTable.SelectRow((nint)0, true);
        }
            
    }
}
