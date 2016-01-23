using System;

using Foundation;
using AppKit;
using Nikse.SubtitleEdit.Core;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Nikse.SubtitleEdit.UILogic;
using System.Linq;

namespace Edit
{
    public partial class MultipleReplace : NSWindow
    {

        public class ReplaceExpression
        {

            public string FindWhat { get; set; }

            public string ReplaceWith { get; set; }

            public int SearchType { get; set; }

            public bool Checked { get; set; }


            public ReplaceExpression(string findWhat, string replaceWith, int searchType)
            {
                FindWhat = findWhat;
                ReplaceWith = replaceWith;
                SearchType = searchType;
            }
        }

        private const string MultipleSearchAndReplaceItem = "MultipleSearchAndReplaceItem";
        private const string RuleEnabled = "Enabled";
        private const string FindWhat = "FindWhat";
        private const string ReplaceWith = "ReplaceWith";
        private const string SearchType = "SearchType";

        public const string SearchNormalText = "Normal";
        public const string SearchRegExText = "RegularExpression";
        public const string SearchCaseSensitiveText = "CaseSensitive";

        public const int SearchTypeNormal = 0;
        public const int SearchTypeCaseSensitive = 1;
        public const int SearchTypeRegularExpression = 2;
        public List<ReplaceExpression> MultipleSearchAndReplaceList = new List<ReplaceExpression>();
        private readonly Dictionary<string, Regex> _compiledRegExList = new Dictionary<string, Regex>();
        List<PreviewItem> _previewItems = new List<PreviewItem>();

        public List<string> DeleteIDs { get; private set; }

        public int FixCount { get; private set; }

        public MultipleReplace(IntPtr handle)
            : base(handle)
        {
        }

        [Export("initWithCoder:")]
        public MultipleReplace(NSCoder coder)
            : base(coder)
        {
        }

        private void InitializeRulesTable(NSTableView table)
        {
            var columns = table.TableColumns();
            columns[0].SetIdentifier(MultipleRulesTableDelegate.CellIdentifiers[0]);
            columns[0].MinWidth = 50;
            columns[0].MaxWidth = 200;
            columns[0].Width = 60;
            var bc = new NSButtonCell();
            bc.SetButtonType(NSButtonType.OnOff);
            columns[0].Title = Configuration.Settings.Language.MultipleReplace.Enabled;
            columns[1].SetIdentifier(MultipleRulesTableDelegate.CellIdentifiers[1]);
            columns[1].MinWidth = 100;
            columns[1].MaxWidth = 600;
            columns[1].Width = 250;
            columns[1].Title = Configuration.Settings.Language.MultipleReplace.FindWhat;
            table.AddColumn(new NSTableColumn(MultipleRulesTableDelegate.CellIdentifiers[2])
                {
                    MinWidth = 100,
                    MaxWidth = 2000,
                    Width = 250,
                    Title = Configuration.Settings.Language.MultipleReplace.ReplaceWith,
                });
            table.AddColumn(new NSTableColumn(MultipleRulesTableDelegate.CellIdentifiers[3])
                {
                    MinWidth = 100,
                    MaxWidth = 2000,
                    Width = 250,
                    Title = Configuration.Settings.Language.MultipleReplace.SearchType,
                });       
            
            _rulesContextMenuDelete.Activated += (object sender, EventArgs e) =>
            {
                var index = (int)_rulesTable.SelectedRow;
                if (index >= 0 && index < MultipleSearchAndReplaceList.Count)
                {
                    MultipleSearchAndReplaceList.RemoveAt(index);
                    ShowRules(MultipleSearchAndReplaceList);
                    if (index >= MultipleSearchAndReplaceList.Count)
                    {
                        index--;
                    }
                    if (index >= 0)
                    {
                        _rulesTable.SelectRow((nint)index, false);
                    }
                }
            };

            _rulesContextMenuMoveUp.Activated += (object sender, EventArgs e) =>
            {
                var index = (int)_rulesTable.SelectedRow;
                if (index > 0 && index < MultipleSearchAndReplaceList.Count)
                {
                    var item = MultipleSearchAndReplaceList[index];
                    MultipleSearchAndReplaceList.RemoveAt(index);
                    MultipleSearchAndReplaceList.Insert(index - 1, item);
                    ShowRules(MultipleSearchAndReplaceList);
                    _rulesTable.SelectRow((nint)index - 1, false);
                }
            };

            _rulesContextMenuMoveDown.Activated += (object sender, EventArgs e) =>
            {
                var index = (int)_rulesTable.SelectedRow;
                if (index >= 0 && index < MultipleSearchAndReplaceList.Count - 1)
                {
                    var item = MultipleSearchAndReplaceList[index];
                    MultipleSearchAndReplaceList.RemoveAt(index);
                    MultipleSearchAndReplaceList.Insert(index + 1, item);
                    ShowRules(MultipleSearchAndReplaceList);
                    _rulesTable.SelectRow((nint)index + 1, false);
                }
            };
        }

        private void InitializePreviewTable(NSTableView table)
        {
            var columns = table.TableColumns();
            columns[0].SetIdentifier(PreviewTableDelegate.CellIdentifiers[0]);
            columns[0].MinWidth = 50;
            columns[0].MaxWidth = 200;
            columns[0].Width = 60;
            var bc = new NSButtonCell();
            bc.SetButtonType(NSButtonType.OnOff);
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

            InitializeRulesTable(_rulesTable);
            InitializePreviewTable(_previewTable);

            this.WillClose += (object sender, EventArgs e) =>
            { 
                NSApplication.SharedApplication.StopModal(); 
            };
            var l = Configuration.Settings.Language.MultipleReplace;
            Title = l.Title;
            _findLabel.StringValue = l.FindWhat;
            _replaceLabel.StringValue = l.ReplaceWith;

            _findType.RemoveAllItems();
            _findType.AddItem(Configuration.Settings.Language.MultipleReplace.Normal);
            _findType.AddItem(Configuration.Settings.Language.MultipleReplace.CaseSensitive);
            _findType.AddItem(Configuration.Settings.Language.MultipleReplace.RegularExpression);
           
            _buttonOk.Activated += (object sender, EventArgs e) =>
            {
                (WindowController as MultipleReplaceController).OkPressed();
                Close();    
            };

            _buttonCancel.Activated += (object sender, EventArgs e) =>
            {
                Close();    
            };

            _addButton.Activated += (object sender, EventArgs e) =>
            {
                string findText = _findText.StringValue.RemoveControlCharacters();
                if (findText.Length > 0)
                {
                    int searchType = (int)_findType.IndexOfSelectedItem;
                    if (searchType == SearchTypeRegularExpression)
                    {
                        if (!Utilities.IsValidRegex(findText))
                        {                                
                            MessageBox.Show(Configuration.Settings.Language.General.RegularExpressionIsNotValid);
                            _findText.BecomeFirstResponder();
                            return;
                        }
                    }
                    var rule = new ReplaceExpression(findText, _replaceText.StringValue.RemoveControlCharacters(), searchType)
                    {
                        Checked = true,
                    };
                    MultipleSearchAndReplaceList.Add(rule);
                    ShowRules(MultipleSearchAndReplaceList);
                    _findText.StringValue = string.Empty;
                    _replaceText.StringValue = string.Empty;
                    (WindowController as MultipleReplaceController).GeneratePreview();
                    _findText.BecomeFirstResponder();
                    SaveReplaceList(false);
                }                
            };

            _updateButton.Activated += (object sender, EventArgs e) =>
            {
                var index = (int)_rulesTable.SelectedRow;
                if (index >= 0 && index < MultipleSearchAndReplaceList.Count)
                {
                    var item = MultipleSearchAndReplaceList[index];
                    string findText = _findText.StringValue.RemoveControlCharacters();
                    int searchType = (int)_findType.IndexOfSelectedItem;
                    if (searchType == SearchTypeRegularExpression)
                    {
                        if (!Utilities.IsValidRegex(findText))
                        {                                
                            MessageBox.Show(Configuration.Settings.Language.General.RegularExpressionIsNotValid);
                            _findText.BecomeFirstResponder();
                            return;
                        }
                    }
                    item.FindWhat = findText;
                    item.ReplaceWith = _replaceText.StringValue;
                    item.SearchType = searchType;
                    ShowRules(MultipleSearchAndReplaceList);
                    _rulesTable.SelectRow((nint)index, false);
                } 
            };

            MultipleSearchAndReplaceList = LoadRules();
            ShowRules(MultipleSearchAndReplaceList);
            (WindowController as MultipleReplaceController).GeneratePreview();           
        }

        private static List<ReplaceExpression> LoadRules()
        {
            var list = new List<ReplaceExpression>();
            foreach (var setting in Configuration.Settings.MultipleSearchAndReplaceList)
            {
                var rule = new ReplaceExpression(setting.FindWhat, setting.ReplaceWith, EnglishSearchTypeToLocal(setting.SearchType));
                rule.Checked = setting.Enabled;
                list.Add(rule);
            }
            return list;
        }

        private void SaveReplaceList(bool saveToDisk)
        {
            Configuration.Settings.MultipleSearchAndReplaceList = new List<MultipleSearchAndReplaceSetting>();
            foreach (var item in MultipleSearchAndReplaceList)
            {
                Configuration.Settings.MultipleSearchAndReplaceList.Add(new MultipleSearchAndReplaceSetting
                    {
                        Enabled = item.Checked,
                        FindWhat = item.FindWhat,
                        ReplaceWith = item.ReplaceWith,
                        SearchType = LocalSearchTypeToEnglish(item.SearchType)
                    });
            }
            if (saveToDisk)
            {
                Configuration.Settings.Save();
            }
        }

        private static string LocalSearchTypeToEnglish(int searchType)
        {
            if (searchType == SearchTypeRegularExpression)
                return SearchRegExText;
            if (searchType == SearchTypeCaseSensitive)
                return SearchCaseSensitiveText;
            return SearchNormalText;
        }

        private static int EnglishSearchTypeToLocal(string searchType)
        {
            if (searchType == SearchRegExText)
                return SearchTypeRegularExpression;
            if (searchType == SearchCaseSensitiveText)
                return SearchTypeCaseSensitive;
            return SearchTypeNormal;
        }

        public void GeneratePreview(Subtitle subtitle, List<ReplaceExpression> replaceRules)
        {
            var fixedSubtitle = new Subtitle(subtitle);           
            _previewItems = new List<PreviewItem>();
            FixCount = 0;
            var replaceExpressions = new HashSet<ReplaceExpression>();
            foreach (var item in replaceRules)
            {
                if (item.Checked)
                {
                    string findWhat = item.FindWhat;
                    if (!string.IsNullOrEmpty(findWhat)) // allow space(s)
                    {
                        replaceExpressions.Add(item);
                        if (item.SearchType == SearchTypeRegularExpression && !_compiledRegExList.ContainsKey(findWhat))
                        {
                            _compiledRegExList.Add(findWhat, new Regex(findWhat, RegexOptions.Compiled | RegexOptions.Multiline));
                        }
                    }
                }
            }

            foreach (Paragraph p in subtitle.Paragraphs)
            {
                bool hit = false;
                string newText = p.Text;
                foreach (ReplaceExpression item in replaceExpressions)
                {
                    if (item.SearchType == SearchTypeCaseSensitive)
                    {
                        if (newText.Contains(item.FindWhat))
                        {
                            hit = true;
                            newText = newText.Replace(item.FindWhat, item.ReplaceWith);
                        }
                    }
                    else if (item.SearchType == SearchTypeRegularExpression)
                    {
                        Regex r = _compiledRegExList[item.FindWhat];
                        if (r.IsMatch(newText))
                        {
                            hit = true;
                            newText = r.Replace(newText, item.ReplaceWith);
                        }
                    }
                    else
                    {
                        int index = newText.IndexOf(item.FindWhat, StringComparison.OrdinalIgnoreCase);
                        if (index >= 0)
                        {
                            hit = true;
                            do
                            {
                                newText = newText.Remove(index, item.FindWhat.Length).Insert(index, item.ReplaceWith);
                                index = newText.IndexOf(item.FindWhat, index + item.ReplaceWith.Length, StringComparison.OrdinalIgnoreCase);
                            }
                            while (index >= 0);
                        }
                    }
                }
                if (hit && newText != p.Text)
                {
                    FixCount++;
                    _previewItems.Add(new PreviewItem(p.ID, true, p.Number.ToString(), p.Text, newText));
                    int index = subtitle.GetIndex(p);
                    fixedSubtitle.Paragraphs[index].Text = newText;                   
                }
            }
            _previewBox.Title = string.Format(Configuration.Settings.Language.MultipleReplace.LinesFoundX, FixCount);
            ShowPreview(_previewItems);
        }

        public void ShowRules(List<ReplaceExpression> rules)
        {
            var ds = new MultipleReplaceRulesTableDataSource(rules);
            _rulesTable.DataSource = ds;
            _rulesTable.Delegate = new MultipleRulesTableDelegate(ds, (WindowController as MultipleReplaceController));
        }

        public void ShowPreview(List<PreviewItem> previewItems)
        {
            var ds = new PreviewTableDataSource(previewItems);
            _previewTable.DataSource = ds;
            _previewTable.Delegate = new PreviewTableDelegate(ds);
        }

        public void ShowEditRule()
        {
            var index = (int)_rulesTable.SelectedRow;
            var rule = MultipleSearchAndReplaceList[index];
            _findText.StringValue = rule.FindWhat;
            _replaceText.StringValue = rule.ReplaceWith;
            _findType.SelectItem((nint)rule.SearchType);
        }                

        public Subtitle GetFixedSubtitle(Subtitle subtitle)
        {
            FixCount = 0;
            DeleteIDs = new List<string>();
            var fixedSubtitle = new Subtitle(subtitle, false);
            foreach (var previewItem in _previewItems)
            {
                if (previewItem.Apply)
                {
                    var p = fixedSubtitle.Paragraphs.First(pa => pa.ID == previewItem.Id);
                    p.Text = previewItem.After.Replace(Configuration.Settings.General.ListViewLineSeparatorString, Environment.NewLine);
                    FixCount++;

                    if (!string.IsNullOrWhiteSpace(p.Text) && (string.IsNullOrWhiteSpace(previewItem.After) || string.IsNullOrWhiteSpace(HtmlUtil.RemoveHtmlTags(previewItem.After, true))))
                    {
                        DeleteIDs.Add(p.ID);
                    }
                }
            }
            fixedSubtitle.RemoveParagraphsByIds(DeleteIDs);
            return fixedSubtitle;
        }

    }
}