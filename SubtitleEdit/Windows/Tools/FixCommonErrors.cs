using System;

using Foundation;
using AppKit;
using System.Collections.Generic;
using Nikse.SubtitleEdit.Core;
using Nikse.SubtitleEdit.Core.Forms.FixCommonErrors;
using Nikse.SubtitleEdit.Core.SubtitleFormats;
using Nikse.SubtitleEdit.UILogic;
using System.Linq;
using Nikse.SubtitleEdit.Tools;
using System.Text;
using System.Globalization;
using UILogic;
using Nikse.SubtitleEdit.Core.Dictionaries;

namespace Tools
{
    public partial class FixCommonErrors : NSWindow, IFixCallbacks, IChangeSubtitleTableSelection, ISubtitleTextChanged
    {
        private Subtitle _subtitle;
        private List<FixItem> _fixActions;
        private List<FixCommonErrorsPreviewItem> _previewItems;
        private List<string> _deleteIDs;
        private int _totalErrors = 0;
        private int _totalFixes = 0;
        private string _autoDetectGoogleLanguage = "en";
        private List<CultureInfo> _languages = new List<CultureInfo>();
        private int _step = 0;
        private const int StepRules = 0;
        private const int StepPreview = 1;
        private bool _doFix = false;
        private List<string> _namesEtcList;
        private HashSet<string> _abbreviationList;
        private int _indexAloneLowercaseIToUppercaseIEnglish;
        private int _turkishAnsiIndex;
        private int _danishLetterIIndex;
        private int _spanishInvertedQuestionAndExclamationMarksIndex;


        public FixCommonErrors(IntPtr handle)
            : base(handle)
        {
        }

        [Export("initWithCoder:")]
        public FixCommonErrors(NSCoder coder)
            : base(coder)
        {
        }

        private Subtitle GetSubtitle()
        {
            if (_doFix)
            {
                return _subtitle;
            }
            else
            {
                return new Subtitle(_subtitle, false);
            }
        }

        public List<FixItem> InitializeRules()
        {
            var ce = Configuration.Settings.CommonErrors;
            var l = Configuration.Settings.Language.FixCommonErrors;

            var list = new List<FixItem>
            {
                new FixItem(l.RemovedEmptyLinesUnsedLineBreaks, string.Empty, () => new FixEmptyLines().Fix(GetSubtitle(), this), ce.EmptyLinesTicked),
                new FixItem(l.FixOverlappingDisplayTimes, string.Empty, () => new FixOverlappingDisplayTimes().Fix(GetSubtitle(), this), ce.OverlappingDisplayTimeTicked),
                new FixItem(l.FixShortDisplayTimes, string.Empty, () => new FixShortDisplayTimes().Fix(GetSubtitle(), this), ce.TooShortDisplayTimeTicked),
                new FixItem(l.FixLongDisplayTimes, string.Empty, () => new FixLongDisplayTimes().Fix(GetSubtitle(), this), ce.TooLongDisplayTimeTicked),
                new FixItem(l.FixInvalidItalicTags, l.FixInvalidItalicTagsExample, () => new FixInvalidItalicTags().Fix(GetSubtitle(), this), ce.InvalidItalicTagsTicked),
                new FixItem(l.RemoveUnneededSpaces, l.RemoveUnneededSpacesExample, () => new FixUnneededSpaces().Fix(GetSubtitle(), this), ce.UnneededSpacesTicked),
                new FixItem(l.RemoveUnneededPeriods, l.RemoveUnneededPeriodsExample, () => new FixUnneededPeriods().Fix(GetSubtitle(), this), ce.UnneededPeriodsTicked),
                new FixItem(l.FixMissingSpaces, l.FixMissingSpacesExample, () => new FixMissingSpaces().Fix(GetSubtitle(), this), ce.MissingSpacesTicked),
                new FixItem(l.BreakLongLines, string.Empty, () => new FixLongLines().Fix(GetSubtitle(), this), ce.BreakLongLinesTicked),
                new FixItem(l.RemoveLineBreaks, string.Empty, () => new FixShortLines().Fix(GetSubtitle(), this), ce.MergeShortLinesTicked),
                new FixItem(l.RemoveLineBreaksAll, string.Empty, () => new FixShortLinesAll().Fix(GetSubtitle(), this), ce.MergeShortLinesAllTicked),
                new FixItem(l.FixDoubleApostrophes, string.Empty, () => new FixDoubleApostrophes().Fix(GetSubtitle(), this), ce.DoubleApostropheToQuoteTicked),
                new FixItem(l.FixMusicNotation, l.FixMusicNotationExample, () => new FixMusicNotation().Fix(GetSubtitle(), this), ce.FixMusicNotationTicked),
                new FixItem(l.AddPeriods, string.Empty, () => new FixMissingPeriodsAtEndOfLine().Fix(GetSubtitle(), this), ce.AddPeriodAfterParagraphTicked),
                new FixItem(l.StartWithUppercaseLetterAfterParagraph, string.Empty, () => new FixStartWithUppercaseLetterAfterParagraph().Fix(GetSubtitle(), this), ce.StartWithUppercaseLetterAfterParagraphTicked),
                new FixItem(l.StartWithUppercaseLetterAfterPeriodInsideParagraph, string.Empty, () => new FixStartWithUppercaseLetterAfterPeriodInsideParagraph().Fix(GetSubtitle(), this), ce.StartWithUppercaseLetterAfterPeriodInsideParagraphTicked),
                new FixItem(l.StartWithUppercaseLetterAfterColon, string.Empty, () => new FixStartWithUppercaseLetterAfterColon().Fix(GetSubtitle(), this), ce.StartWithUppercaseLetterAfterColonTicked),
                new FixItem(l.AddMissingQuotes, l.AddMissingQuotesExample, () => new AddMissingQuotes().Fix(GetSubtitle(), this), ce.AddMissingQuotesTicked),
                new FixItem(l.FixHyphens, string.Empty, () => new FixHyphensRemove().Fix(GetSubtitle(), this), ce.FixHyphensTicked),
                new FixItem(l.FixHyphensAdd, string.Empty, () => new FixHyphensAdd().Fix(GetSubtitle(), this), ce.FixHyphensAddTicked),
                new FixItem(l.Fix3PlusLines, string.Empty, () => new Fix3PlusLines().Fix(GetSubtitle(), this), ce.Fix3PlusLinesTicked),
                new FixItem(l.FixDoubleDash, l.FixDoubleDashExample, () => new FixDoubleDash().Fix(GetSubtitle(), this), ce.FixDoubleDashTicked),
                new FixItem(l.FixDoubleGreaterThan, l.FixDoubleGreaterThanExample, () => new FixDoubleGreaterThan().Fix(GetSubtitle(), this), ce.FixDoubleGreaterThanTicked),
                new FixItem(l.FixEllipsesStart, l.FixEllipsesStartExample, () => new FixEllipsesStart().Fix(GetSubtitle(), this), ce.FixEllipsesStartTicked),
                new FixItem(l.FixMissingOpenBracket, l.FixMissingOpenBracketExample, () => new FixMissingOpenBracket().Fix(GetSubtitle(), this), ce.FixMissingOpenBracketTicked),
                //       new FixItem(l.FixCommonOcrErrors, l.FixOcrErrorExample, () => FixOcrErrorsViaReplaceList(threeLetterIsoLanguageName), ce.FixOcrErrorsViaReplaceListTicked),
                new FixItem(l.FixUppercaseIInsindeLowercaseWords, l.FixUppercaseIInsindeLowercaseWordsExample, () => new FixUppercaseIInsideWords().Fix(GetSubtitle(), this), ce.UppercaseIInsideLowercaseWordTicked),
                new FixItem(l.RemoveSpaceBetweenNumber, l.FixSpaceBetweenNumbersExample, () => new RemoveSpaceBetweenNumbers().Fix(GetSubtitle(), this), ce.RemoveSpaceBetweenNumberTicked),
                new FixItem(l.FixDialogsOnOneLine, l.FixDialogsOneLineExample, () => new FixDialogsOnOneLine().Fix(GetSubtitle(), this), ce.FixDialogsOnOneLineTicked)
            };

            if (Language == "en")
            {
                _indexAloneLowercaseIToUppercaseIEnglish = list.Count;
                list.Add(new FixItem(l.FixLowercaseIToUppercaseI, l.FixLowercaseIToUppercaseIExample, () => new FixAloneLowercaseIToUppercaseI().Fix(GetSubtitle(), this), ce.AloneLowercaseIToUppercaseIEnglishTicked));
            }
            if (Language == "tr")
            {
                _turkishAnsiIndex = list.Count;
                list.Add(new FixItem(l.FixTurkishAnsi, "Ý > İ, Ð > Ğ, Þ > Ş, ý > ı, ð > ğ, þ > ş", () => new FixTurkishAnsiToUnicode().Fix(GetSubtitle(), this), ce.TurkishAnsiTicked));
            }

            if (Language == "da")
            {
                _danishLetterIIndex = list.Count;
                list.Add(new FixItem(l.FixDanishLetterI, "Jeg synes i er søde. -> Jeg synes I er søde.", () => new FixDanishLetterI().Fix(GetSubtitle(), this), ce.DanishLetterITicked));
            }

            if (Language == "es")
            {
                _spanishInvertedQuestionAndExclamationMarksIndex = list.Count;
                list.Add(new FixItem(l.FixSpanishInvertedQuestionAndExclamationMarks, "Hablas bien castellano? -> ¿Hablas bien castellano?", () => new FixSpanishInvertedQuestionAndExclamationMarks().Fix(GetSubtitle(), this), ce.SpanishInvertedQuestionAndExclamationMarksTicked));
            }

            return list;
        }

        private void InitializeRulesTable(NSTableView table)
        {
            var columns = table.TableColumns();
            columns[0].SetIdentifier(FixCommonErrorsTableDelegate.CellIdentifiers[0]);
            columns[0].MinWidth = 50;
            columns[0].MaxWidth = 200;
            columns[0].Width = 60;
            var bc = new NSButtonCell();
            bc.SetButtonType(NSButtonType.OnOff);
            columns[0].Title = Configuration.Settings.Language.MultipleReplace.Enabled;
            columns[1].SetIdentifier(FixCommonErrorsTableDelegate.CellIdentifiers[1]);
            columns[1].MinWidth = 100;
            columns[1].MaxWidth = 600;
            columns[1].Width = 250;
            columns[1].Title = Configuration.Settings.Language.MultipleReplace.FindWhat;
            table.AddColumn(new NSTableColumn(FixCommonErrorsTableDelegate.CellIdentifiers[2])
                {
                    MinWidth = 100,
                    MaxWidth = 2000,
                    Width = 250,
                    Title = Configuration.Settings.Language.MultipleReplace.ReplaceWith,
                });

            _fixActions = InitializeRules();
            ShowFixRules();
        }

        private void ShowFixRules()
        {
            var ds = new FixCommonErrorsRulesTableDataSource(_fixActions);
            _tableRules.DataSource = ds;
            _tableRules.Delegate = new FixCommonErrorsTableDelegate(ds, (WindowController as FixCommonErrorsController));
        }

        private void InitializePreviewTable(NSTableView table)
        {
            var columns = table.TableColumns();
            columns[0].SetIdentifier(FixCommonErrorsPreviewTableDelegate.CellIdentifiers[0]);
            columns[0].MinWidth = 50;
            columns[0].MaxWidth = 200;
            columns[0].Width = 60;
            var bc = new NSButtonCell();
            bc.SetButtonType(NSButtonType.OnOff);
            columns[0].Title = Configuration.Settings.Language.General.Apply;
            columns[1].SetIdentifier(FixCommonErrorsPreviewTableDelegate.CellIdentifiers[1]);
            columns[1].MinWidth = 100;
            columns[1].MaxWidth = 600;
            columns[1].Width = 250;
            columns[1].Title = Configuration.Settings.Language.FixCommonErrors.Fixes;
            table.AddColumn(new NSTableColumn(FixCommonErrorsPreviewTableDelegate.CellIdentifiers[2])
                {
                    MinWidth = 100,
                    MaxWidth = 2000,
                    Width = 250,
                    Title = Configuration.Settings.Language.General.Before,
                });
            table.AddColumn(new NSTableColumn(FixCommonErrorsPreviewTableDelegate.CellIdentifiers[3])
                {
                    MinWidth = 100,
                    MaxWidth = 2000,
                    Width = 250,
                    Title = Configuration.Settings.Language.General.After,
                });
        }

        private void InitializeSubtitleTable(NSTableView table)
        {
            var columns = table.TableColumns();
            columns[0].SetIdentifier(SubtitleTableDelegate.CellIdentifiers[0]);
            columns[0].MinWidth = 20;
            columns[0].MaxWidth = 100;
            columns[0].Width = 40;
            columns[0].Title = Configuration.Settings.Language.General.LineNumber;
            columns[1].SetIdentifier(SubtitleTableDelegate.CellIdentifiers[1]);
            columns[1].MinWidth = 50;
            columns[1].MaxWidth = 200;
            columns[1].Width = 90;
            columns[1].Title = Configuration.Settings.Language.General.StartTime;
            table.AddColumn(new NSTableColumn(SubtitleTableDelegate.CellIdentifiers[2])
                {
                    MinWidth = 50,
                    MaxWidth = 200,
                    Width = 90,
                    Title = Configuration.Settings.Language.General.EndTime,
                });
            table.AddColumn(new NSTableColumn(SubtitleTableDelegate.CellIdentifiers[3])
                {
                    MinWidth = 40,
                    MaxWidth = 150,
                    Width = 60,
                    Title = Configuration.Settings.Language.General.Duration,
                });
            table.AddColumn(new NSTableColumn(SubtitleTableDelegate.CellIdentifiers[4])
                {
                    MinWidth = 100,
                    MaxWidth = 90000,
                    Width = 1000,
                    Title = Configuration.Settings.Language.General.Text,
                });
            ShowSubtitle();
        }

        private void ShowSubtitle()
        {
            nint selectRow = 0;
            if (_tableSubtitle.SelectedRows.Count > 0)
            {
                selectRow = _tableSubtitle.SelectedRow;
            }

            var ds = new SubtitleTableDataSource (_subtitle, null);
            _tableSubtitle.DataSource = ds;
            _tableSubtitle.Delegate = new SubtitleTableDelegate (ds, this);

            ShowSubtitleRow(selectRow);
        }

        #region ISubtitleTextChanged implementation

        public void SubtitleTextChanged()
        {
            if (_tableSubtitle.SelectedRows.Count > 0) 
            {
                nint selectRow = _tableSubtitle.SelectedRow;
                var p = _subtitle.Paragraphs[(int)selectRow];
                p.Text = _subtitleText.StringValue;
                ShowSubtitle();
                _buttonNext.Enabled = true;
            }
        }

        #endregion

        #region IChangeSubtitleTableSelection implementation

        public void ChangeSubtitleTableSelection()
        {
            if (_tableSubtitle.SelectedRows.Count > 0) 
            {
                nint selectRow = _tableSubtitle.SelectedRow;
                var p = _subtitle.Paragraphs[(int)selectRow];
                _subtitleText.StringValue = p.Text;
                //Window.SetTimeCode(_selectedParagraph);
            }
        }

        #endregion

        public void PreviewTableSelectionChanged()
        {
            if (_tablePreview.SelectedRows.Count > 0) 
            {
                nint index = _tablePreview.SelectedRow;
                if (index < 0 && index >= _previewItems.Count)
                {
                    return;
                }

                var previewItem = _previewItems[(int)index];
                for (int i = 0; i < _subtitle.Paragraphs.Count; i++)
                {
                    if (previewItem.Id == _subtitle.Paragraphs[i].ID)
                    {
                        ShowSubtitleRow((nint)i);
                        break;
                    }
                }                   
            }
        }

        public nint ShowSubtitleRow (nint rowNumber)
        {
            if (_tableSubtitle.RowCount == 0)
                return -1;

            if (rowNumber >= _tableSubtitle.RowCount)
                rowNumber = _tableSubtitle.RowCount - 1;

            if (rowNumber < 0)
                rowNumber = 0;

            _tableSubtitle.SelectRow (rowNumber, false);
            _tableSubtitle.ScrollRowToVisible (rowNumber);
            _subtitleText.StringValue = (_tableSubtitle.DataSource as SubtitleTableDataSource).Subtitle.Paragraphs [(int)rowNumber].Text;
            return rowNumber;
        }

        private void InitializeLanguage(Encoding encoding)
        {
            _autoDetectGoogleLanguage = LanguageAutoDetect.AutoDetectGoogleLanguage(encoding); // Guess language via encoding
            if (string.IsNullOrEmpty(_autoDetectGoogleLanguage))
                _autoDetectGoogleLanguage = LanguageAutoDetect.AutoDetectGoogleLanguage(_subtitle); // Guess language based on subtitle contents
            if (_autoDetectGoogleLanguage.Equals("zh", StringComparison.OrdinalIgnoreCase))
                _autoDetectGoogleLanguage = "zh-CHS"; // Note that "zh-CHS" (Simplified Chinese) and "zh-CHT" (Traditional Chinese) are neutral cultures
            CultureInfo ci = CultureInfo.GetCultureInfo(_autoDetectGoogleLanguage);
            string threeLetterIsoLanguageName = ci.ThreeLetterISOLanguageName;

            _popUpLanguage.RemoveAllItems();
            foreach (CultureInfo x in CultureInfo.GetCultures(CultureTypes.NeutralCultures))
                _languages.Add(x);
            _languages = _languages.Where(p=>!string.IsNullOrEmpty(p.EnglishName) && p.TwoLetterISOLanguageName != "iv").OrderBy(p => p.ToString()).ToList();

            int languageIndex = 0;
            int j = 0;
            foreach (var xci in _languages)
            {
                _popUpLanguage.AddItem(xci.EnglishName);
                if (xci.TwoLetterISOLanguageName == ci.TwoLetterISOLanguageName)
                {
                    languageIndex = j;
                }
                j++;
            }
            _popUpLanguage.SelectItem((nint)languageIndex);
            _popUpLanguage.Activated += (object sender, EventArgs e) => 
            {
                _fixActions = InitializeRules();
                ShowFixRules();
            }; 
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();

            this.WillClose += (object sender, EventArgs e) =>
            { 
                NSApplication.SharedApplication.StopModal(); 
            };

            _subtitle = (WindowController as FixCommonErrorsController).Subtitle;

            var l = Configuration.Settings.Language.FixCommonErrors;
            Title = l.Title;
            _boxStep1.Title = l.Step1;
            _boxStep2.Title = l.Step2;
            _boxStep2.Hidden = true;

            // Resize step1 + step2 to fill window
            _boxStep1.SetFrameSize(new CoreGraphics.CGSize(_boxStep1.Frame.Left + _boxStep1.Frame.Width - _boxStep2.Frame.Left, _boxStep1.Frame.Height));
            _boxStep1.SetFrameOrigin(new CoreGraphics.CGPoint(_boxStep2.Frame.Left, _boxStep2.Frame.Top));
            _boxStep2.SetFrameSize(new CoreGraphics.CGSize(_boxStep1.Frame.Width, _boxStep2.Frame.Height));
            _boxStep2.SetFrameOrigin(new CoreGraphics.CGPoint(_boxStep1.Frame.Left, _boxStep2.Frame.Top));

            _subtitleText.Delegate = new SubtitleTextDelegate(this);  

            InitializeLanguage(Encoding.UTF8);

            InitializeRulesTable(_tableRules);

            InitializePreviewTable(_tablePreview);

            InitializeSubtitleTable(_tableSubtitle);

            _buttonSelectAll.Title = l.SelectAll.RemoveWindowsShortCut();
            _buttonSelectAll.Activated += (object sender, EventArgs e) =>
            {
                foreach (var r in _fixActions)
                {
                    r.Checked = true;    
                }
                ShowFixRules();
                SaveRulesCheckedState();
            };

            _buttonInvertSelection.Title = l.InverseSelection.RemoveWindowsShortCut();
            _buttonInvertSelection.Activated += (object sender, EventArgs e) =>
            {
                foreach (var r in _fixActions)
                {
                    r.Checked = !r.Checked;    
                }
                ShowFixRules();
                SaveRulesCheckedState();
            };

            _buttonCancel.Title = Configuration.Settings.Language.General.Cancel;
            _buttonCancel.Activated += (object sender, EventArgs e) =>
            {
                Close();
            };

            _buttonNext.Title = l.Next.RemoveWindowsShortCut();
            _buttonNext.Activated += (object sender, EventArgs e) =>
            {
                if (_step == StepRules)
                {
                    _step = StepPreview;
                    _buttonNext.Enabled = false;
                    _buttonBack.Enabled = true;
                    _boxStep1.Hidden = true;
                    _boxStep2.Hidden = false;
                    GeneratePreview();
                    _buttonNext.Title = Configuration.Settings.Language.General.Ok.RemoveWindowsShortCut();
                }
                else
                {
                    (WindowController as FixCommonErrorsController).OkPressed();
                    Close();
                }
            };

            _buttonBack.Title = l.Back.RemoveWindowsShortCut();
            _buttonBack.Activated += (object sender, EventArgs e) =>
            {
                _step = StepRules;
                _buttonNext.Enabled = true;
                _buttonBack.Enabled = false;
                _boxStep1.Hidden = false;
                _boxStep2.Hidden = true;
                _buttonNext.Title = l.Next.RemoveWindowsShortCut();                                                
            };

            _buttonApplySelectedFixes.Title = l.ApplyFixes;
            _buttonApplySelectedFixes.Activated += (object sender, EventArgs e) =>
            {
                var oldPreviewItems = _previewItems;
                _doFix = true;
                _subtitle = ApplySelectedFixes();
                _doFix = false;
                _buttonNext.Enabled = true;
                GeneratePreview();
                foreach (var pi in _previewItems)
                {
                    if (oldPreviewItems.Any(p=>p.Id == pi.Id && !p.Apply && p.Action == pi.Action))
                    {
                            pi.Apply = false;
                    }
                }
                ShowPreview();
                ShowSubtitle();
            };

            _buttonSelectAllFixes.Title = l.SelectAll;
            _buttonSelectAllFixes.Activated += (object sender, EventArgs e) =>
            {
                foreach (var previewItem in _previewItems)
                {
                    previewItem.Apply = true;
                }
                ShowPreview();
            };

            _buttonInvertFixes.Title = l.InverseSelection;
            _buttonInvertFixes.Activated += (object sender, EventArgs e) =>
            {
                foreach (var previewItem in _previewItems)
                {
                    previewItem.Apply = !previewItem.Apply;
                }
                ShowPreview();
            };
        }

        private void SaveRulesCheckedState()
        {
            for (int i = 0; i < _fixActions.Count; i++)
            {
                SaveRuleState(i, _fixActions[i].Checked);
            }
        }

        private void GeneratePreview()
        {
            _totalErrors = 0;
            _totalFixes = 0;
            _deleteIDs = new List<string>();
            _previewItems = new List<FixCommonErrorsPreviewItem>();

            foreach (var fixAction in _fixActions)
            {
                if (fixAction.Checked)
                {
                    fixAction.Action.Invoke();
                }
            }
            ShowPreview();
        }

        private void ShowPreview()
        {
            _previewItems = _previewItems.OrderBy(p => int.Parse(p.LineNumber)).ToList();
            var ds = new FixCommonErrorsPreviewTableDataSource(_previewItems);
            _tablePreview.DataSource = ds;
            _tablePreview.Delegate = new FixCommonErrorsPreviewTableDelegate(ds, (WindowController as FixCommonErrorsController));
        }

        public Subtitle ApplySelectedFixes()
        {
            _totalErrors = 0;
            _totalFixes = 0;
            foreach (var fixAction in _fixActions)
            {
                if (fixAction.Checked)
                {
                    fixAction.Action.Invoke();
                }
            }
            _subtitle.RemoveParagraphsByIds(_deleteIDs);
            return _subtitle;
        }

        public Subtitle FixedSubtitle
        {
            get
            { 
                return _subtitle;
            }
        }

        public void SaveRuleState(int index, bool b)
        {
            var ce = Configuration.Settings.CommonErrors;
            switch (index)
            {
                case 0: 
                    ce.EmptyLinesTicked = b;
                    break;
                case 1: 
                    ce.OverlappingDisplayTimeTicked = b;
                    break;
                case 2: 
                    ce.TooShortDisplayTimeTicked = b;
                    break;
                case 3: 
                    ce.TooLongDisplayTimeTicked = b;
                    break;
                case 4: 
                    ce.InvalidItalicTagsTicked = b;
                    break;
                case 5: 
                    ce.UnneededSpacesTicked = b;
                    break;
                case 6: 
                    ce.UnneededPeriodsTicked = b;
                    break;
                case 7: 
                    ce.MissingSpacesTicked = b;
                    break;
                case 8: 
                    ce.BreakLongLinesTicked = b;
                    break;
                case 9: 
                    ce.MergeShortLinesTicked = b;
                    break;
                case 10: 
                    ce.MergeShortLinesAllTicked = b;
                    break;
                case 11: 
                    ce.DoubleApostropheToQuoteTicked = b;
                    break;
                case 12: 
                    ce.FixMusicNotationTicked = b;
                    break;
                case 13: 
                    ce.AddPeriodAfterParagraphTicked = b;
                    break;
                case 14: 
                    ce.StartWithUppercaseLetterAfterParagraphTicked = b;
                    break;
                case 15: 
                    ce.StartWithUppercaseLetterAfterPeriodInsideParagraphTicked = b;
                    break;
                case 16: 
                    ce.StartWithUppercaseLetterAfterColonTicked = b;
                    break;
                case 17: 
                    ce.AddMissingQuotesTicked = b;
                    break;
                case 18: 
                    ce.FixHyphensTicked = b;
                    break;
                case 19: 
                    ce.FixHyphensAddTicked = b;
                    break;
                case 20: 
                    ce.Fix3PlusLinesTicked = b;
                    break;
                case 21: 
                    ce.FixDoubleDashTicked = b;
                    break;
                case 22: 
                    ce.FixDoubleGreaterThanTicked = b;
                    break;
                case 23: 
                    ce.FixEllipsesStartTicked = b;
                    break;
                case 24: 
                    ce.FixMissingOpenBracketTicked = b;
                    break;
            //                                case 25 : 
            //                                    ce.FixOcrErrorsViaReplaceListTicked = b;
            //                                    break;
                case 25: 
                    ce.UppercaseIInsideLowercaseWordTicked = b;
                    break;
                case 26: 
                    ce.RemoveSpaceBetweenNumberTicked = b;
                    break;
                case 27: 
                    ce.FixDialogsOnOneLineTicked = b;
                    break;        
                default:
                    break;
            }
            if (index == _indexAloneLowercaseIToUppercaseIEnglish)
            {
                ce.AloneLowercaseIToUppercaseIEnglishTicked = b;
            }
            if (index == _turkishAnsiIndex)
            {
                ce.TurkishAnsiTicked = b;
            }
            if (index == _danishLetterIIndex)
            {
                ce.DanishLetterITicked = b;
            }
            if (index == _spanishInvertedQuestionAndExclamationMarksIndex)
            {
                ce.SpanishInvertedQuestionAndExclamationMarksTicked = b;
            }                
        }

        #region IFixCallbacks implementation

        public bool AllowFix(Paragraph paragraph, string action)
        {
            if (_doFix)
            {
                var pi = _previewItems.FirstOrDefault(p => p.Id == paragraph.ID);
                return pi != null && pi.Apply;
            }
            return true;
        }

        public void AddFixToListView(Paragraph p, string action, string before, string after)
        {
            _previewItems.Add(new FixCommonErrorsPreviewItem(p.ID, true, p.Number.ToString(), action, before, after));
        }

        public void LogStatus(string sender, string message)
        {
            
        }

        public void LogStatus(string sender, string message, bool isImportant)
        {
            
        }

        public void UpdateFixStatus(int fixes, string message, string xMessage)
        {
            _totalFixes += fixes;
        }

        public bool IsName(string candidate)
        {
            MakeSureNamesListIsLoaded();
            return _namesEtcList.Contains(candidate);
        }

        private void MakeSureNamesListIsLoaded()
        {
            if (_namesEtcList == null)
            {
                _namesEtcList = new List<string>();
                string languageTwoLetterCode = LanguageAutoDetect.AutoDetectGoogleLanguage(_subtitle);

                // Will contains both one word names and multi names
                var namesList = new NamesList(Configuration.DictionariesFolder, languageTwoLetterCode, Configuration.Settings.WordLists.UseOnlineNamesEtc, Configuration.Settings.WordLists.NamesEtcUrl);
                _namesEtcList = namesList.GetAllNames();
            }
        }

        public HashSet<string> GetAbbreviations()
        {
            if (_abbreviationList != null)
                return _abbreviationList;

            MakeSureNamesListIsLoaded();
            _abbreviationList = new HashSet<string>();
            foreach (string name in _namesEtcList)
            {
                if (name.EndsWith('.'))
                    _abbreviationList.Add(name);
            }
            return _abbreviationList;
        }

        public void AddToTotalErrors(int count)
        {
            _totalErrors += count;
        }

        public void AddToDeleteIndices(int index)
        {
            var p = _subtitle.GetParagraphOrDefault(index);
            if (p != null)
            {
                _deleteIDs.Add(p.ID);
            }         
        }

        public Nikse.SubtitleEdit.Core.SubtitleFormats.SubtitleFormat Format
        {
            get
            {
                return new SubRip();
            }
        }

        public System.Text.Encoding Encoding
        {
            get
            {
                return Encoding.UTF8;
            }
        }

        public string Language
        {
            get
            {
                var idx = _popUpLanguage.IndexOfSelectedItem;
                if (idx >= 0 && idx < _languages.Count)
                {
                    return _languages[(int)idx].TwoLetterISOLanguageName;
                }
                return "en";
            }
        }

        #endregion
    }
}
