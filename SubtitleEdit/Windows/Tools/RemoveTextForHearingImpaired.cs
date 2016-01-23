using System;

using Foundation;
using AppKit;
using Nikse.SubtitleEdit.Core;
using Nikse.SubtitleEdit.Core.Forms;
using System.Collections.Generic;
using System.Linq;
using Nikse.SubtitleEdit.UILogic;

namespace Tools
{
    public partial class RemoveTextForHearingImpaired : NSWindow
    {
        public RemoveTextForHearingImpaired(IntPtr handle)
            : base(handle)
        {
        }

        [Export("initWithCoder:")]
        public RemoveTextForHearingImpaired(NSCoder coder)
            : base(coder)
        {
        }

        private void InitializeTable(NSTableView table)
        {
            var columns = table.TableColumns();
            columns[0].SetIdentifier(PreviewTableDelegate.CellIdentifiers[0]);
            columns[0].MinWidth = 50;
            columns[0].MaxWidth = 200;
            columns[0].Width = 60;
            var bc = new NSButtonCell();
            bc.SetButtonType(NSButtonType.OnOff);
         //   columns[0].DataCell = bc;
            columns[0].Title = Configuration.Settings.Language.General.Apply;
            columns[1].SetIdentifier(PreviewTableDelegate.CellIdentifiers[1]);
            columns[1].MinWidth = 50;
            columns[1].MaxWidth = 200;
            columns[1].Width = 60;
            columns[1].Title = Configuration.Settings.Language.General.LineNumber;
            table.AddColumn(new NSTableColumn(PreviewTableDelegate.CellIdentifiers[2]) 
                {
                    MinWidth = 100,
                    MaxWidth = 2000,
                    Width = 250,
                    Title = Configuration.Settings.Language.General.Before,
                });
            table.AddColumn(new NSTableColumn(PreviewTableDelegate.CellIdentifiers[3]) 
                {
                    MinWidth = 100,
                    MaxWidth = 2000,
                    Width = 250,
                    Title = Configuration.Settings.Language.General.After,
                });                             
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();

            InitializeTable(_previewTable);

            this.WillClose += (object sender, EventArgs e) => 
            { 
                SaveSettings();
                NSApplication.SharedApplication.StopModal(); 
            };

            _buttonOK.Activated += (object sender, EventArgs e) => 
            {
                (WindowController as RemoveTextForHearingImpairedController).OkPressed(new Subtitle());
                Close();
            };  

            _buttonCancel.Activated += (object sender, EventArgs e) => 
            {
                Close();
            };  
            
            SetSettings();


            var l = Configuration.Settings.Language.RemoveTextFromHearImpaired;
            Title = l.Title;

            _removeBetweenSquare.Title = l.SquareBrackets;
            _removeBetweenSquare.Activated += GeneratePreview;

            _removeBetweenPara.Title = l.Parentheses;
            _removeBetweenPara.Activated += GeneratePreview;

            _removeBetweenCurly.Title = l.Brackets;
            _removeBetweenCurly.Activated += GeneratePreview;

            _removeBetweenQuest.Title = l.QuestionMarks;
            _removeBetweenQuest.Activated += GeneratePreview;

            _removeBetweenCust.Title = string.Empty;
            _removeBetweenCust.Activated += GeneratePreview;

            _removeBetweenOnlyIfOnSeperateLine.Title = l.OnlyIfInSeparateLine;
            _removeBetweenOnlyIfOnSeperateLine.Activated += GeneratePreview;

            _removeTextBeforeColon.Title = l.RemoveTextBeforeColon;
            _removeTextBeforeColon.Activated += GeneratePreview;

            _removeTextBeforeColonOnlyUppercase.Title = l.OnlyIfTextIsUppercase;
            _removeTextBeforeColonOnlyUppercase.Activated += GeneratePreview;

            _removeTextBeforeColonOnlySeperateLine.Title = l.OnlyIfInSeparateLine;
            _removeTextBeforeColonOnlySeperateLine.Activated += GeneratePreview;

            _removeLineIfUppercase.Title = l.RemoveTextIfAllUppercase;
            _removeLineIfUppercase.Activated += GeneratePreview;

            _removeLineIfContains.Title = l.RemoveTextIfContains;
            _removeLineIfContains.Activated += GeneratePreview;

            _removeInterjections.Title = l.RemoveInterjections;
            _removeInterjections.Activated += GeneratePreview;

            var list = new List<NSString>()
            {
                new NSString("¶"),
                new NSString("♪"),
                new NSString("♫")
            };
            _removeBetweenCustLeft.RemoveAll();
            _removeBetweenCustRight.RemoveAll();
            foreach (var ns in list)
            {
                _removeBetweenCustLeft.Add(ns);
                _removeBetweenCustRight.Add(ns);
            }
            _removeBetweenCustLeft.Select(list[0]);
            _removeBetweenCustRight.Select(list[0]);

            list = new List<NSString>()
            {
                new NSString("¶"),
                new NSString("♪"),
                new NSString("♫"),
                new NSString("♪, ♫")
            };
            _removeLineIfContainsText.RemoveAll();
            foreach (var ns in list)
            {
                _removeLineIfContainsText.Add(ns);
            }
            _removeLineIfContainsText.Select(list[0]);


            _editInterjections.Title = l.EditInterjections;
            _editInterjections.Activated += (object sender, EventArgs e) => 
            {
                using(var controller = new EditInterjectionsController())
                {
                    controller.Window.ReleasedWhenClosed = true;
                    NSApplication.SharedApplication.RunModalForWindow(controller.Window); // window's WillClose event stops modal
                    if (controller.WasOkPressed)
                    {
                            GeneratePreview(null, null);
                    }
                }       
            };

            _andForCustom.StringValue = l.And;


            GeneratePreview(null, null);
        }

        public IEnumerable<PreviewItem> GetFixes()
        {
            return (_previewTable.DataSource as PreviewTableDataSource).Items.Where(p => p.Apply);
        }

        public void ShowFixes(List<PreviewItem> previewItems)
        {
            var ds = new PreviewTableDataSource(previewItems);
            _previewTable.DataSource = ds;
            _previewTable.Delegate = new PreviewTableDelegate(ds);
        }

        private void SetCheckBox(NSButton checkBox, bool isChecked)
        {
            if (isChecked)
            {
                checkBox.State = NSCellStateValue.On;
            }
            else
            {
                checkBox.State = NSCellStateValue.Off;
            }
        }

        private void SetSettings()
        {
            SetCheckBox(_removeBetweenCurly, Configuration.Settings.RemoveTextForHearingImpaired.RemoveTextBetweenBrackets);
            SetCheckBox(_removeBetweenPara, Configuration.Settings.RemoveTextForHearingImpaired.RemoveTextBetweenParentheses);    
            SetCheckBox(_removeBetweenCurly, Configuration.Settings.RemoveTextForHearingImpaired.RemoveTextBetweenCurlyBrackets);    
            SetCheckBox(_removeBetweenQuest, Configuration.Settings.RemoveTextForHearingImpaired.RemoveTextBetweenQuestionMarks);    
            SetCheckBox(_removeBetweenOnlyIfOnSeperateLine, Configuration.Settings.RemoveTextForHearingImpaired.RemoveTextBetweenOnlySeperateLines);    
            SetCheckBox(_removeTextBeforeColon, Configuration.Settings.RemoveTextForHearingImpaired.RemoveTextBeforeColon);    
            SetCheckBox(_removeTextBeforeColonOnlyUppercase, Configuration.Settings.RemoveTextForHearingImpaired.RemoveTextBeforeColonOnlyIfUppercase);    
            SetCheckBox(_removeTextBeforeColonOnlySeperateLine, Configuration.Settings.RemoveTextForHearingImpaired.RemoveTextBeforeColonOnlyOnSeparateLine);    
            SetCheckBox(_removeInterjections, Configuration.Settings.RemoveTextForHearingImpaired.RemoveInterjections);    
            SetCheckBox(_removeLineIfContains, Configuration.Settings.RemoveTextForHearingImpaired.RemoveIfContains);    
            SetCheckBox(_removeLineIfUppercase, Configuration.Settings.RemoveTextForHearingImpaired.RemoveIfAllUppercase);    
        }

        private void SaveSettings()
        {
            Configuration.Settings.RemoveTextForHearingImpaired.RemoveTextBetweenBrackets = _removeBetweenCurly.State == NSCellStateValue.On;
            Configuration.Settings.RemoveTextForHearingImpaired.RemoveTextBetweenParentheses = _removeBetweenPara.State == NSCellStateValue.On;
            Configuration.Settings.RemoveTextForHearingImpaired.RemoveTextBetweenCurlyBrackets = _removeBetweenCurly.State == NSCellStateValue.On;
            Configuration.Settings.RemoveTextForHearingImpaired.RemoveTextBetweenQuestionMarks = _removeBetweenQuest.State == NSCellStateValue.On;
            Configuration.Settings.RemoveTextForHearingImpaired.RemoveTextBetweenCustom = _removeBetweenCust.State == NSCellStateValue.On;
            Configuration.Settings.RemoveTextForHearingImpaired.RemoveTextBetweenOnlySeperateLines = _removeBetweenOnlyIfOnSeperateLine.State == NSCellStateValue.On;    
            Configuration.Settings.RemoveTextForHearingImpaired.RemoveTextBeforeColon = _removeTextBeforeColon.State == NSCellStateValue.On;    
            Configuration.Settings.RemoveTextForHearingImpaired.RemoveTextBeforeColonOnlyIfUppercase = _removeTextBeforeColonOnlyUppercase.State == NSCellStateValue.On; 
            Configuration.Settings.RemoveTextForHearingImpaired.RemoveTextBeforeColonOnlyOnSeparateLine = _removeTextBeforeColonOnlySeperateLine.State == NSCellStateValue.On; 
            Configuration.Settings.RemoveTextForHearingImpaired.RemoveInterjections = _removeInterjections.State == NSCellStateValue.On; 
            Configuration.Settings.RemoveTextForHearingImpaired.RemoveIfContains = _removeLineIfContains.State == NSCellStateValue.On; 
            Configuration.Settings.RemoveTextForHearingImpaired.RemoveIfAllUppercase = _removeLineIfUppercase.State == NSCellStateValue.On;
        }

        public RemoveTextForHISettings GetSettings()
        {
            var removeWhereContains = _removeLineIfContainsText.StringValue.Replace(" ", string.Empty).Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();

            return new RemoveTextForHISettings
            {
                OnlyIfInSeparateLine = _removeBetweenOnlyIfOnSeperateLine.State == NSCellStateValue.On,
                RemoveIfAllUppercase = _removeLineIfUppercase.State == NSCellStateValue.On,
                RemoveTextBeforeColon = _removeTextBeforeColon.State == NSCellStateValue.On,
                RemoveTextBeforeColonOnlyUppercase = _removeTextBeforeColonOnlyUppercase.State == NSCellStateValue.On,
                ColonSeparateLine = _removeTextBeforeColonOnlySeperateLine.State == NSCellStateValue.On,
                RemoveWhereContains = _removeLineIfContains.State == NSCellStateValue.On,
                RemoveIfTextContains = removeWhereContains,
                RemoveTextBetweenCustomTags = _removeBetweenCust.State == NSCellStateValue.On,
                RemoveInterjections = _removeInterjections.State == NSCellStateValue.On,
                RemoveTextBetweenSquares = _removeBetweenSquare.State == NSCellStateValue.On,
                RemoveTextBetweenBrackets = _removeBetweenCurly.State == NSCellStateValue.On,
                RemoveTextBetweenQuestionMarks = _removeBetweenQuest.State == NSCellStateValue.On,
                RemoveTextBetweenParentheses = _removeBetweenPara.State == NSCellStateValue.On,
                CustomStart = _removeBetweenCustLeft.StringValue,
                CustomEnd = _removeBetweenCustRight.StringValue
            };
        }

        private void GeneratePreview(Object sender, EventArgs e)
        {
            var settings = GetSettings();
            (WindowController as RemoveTextForHearingImpairedController).GeneratePreview(settings);
        }

    }
}
