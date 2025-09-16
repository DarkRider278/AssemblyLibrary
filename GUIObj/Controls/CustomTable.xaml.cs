// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CustomTableHeaderList.xaml.cs" company="">
//   2021
// </copyright>
// <summary>
//   The custom table column type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using GUIObj.Structs;
using HelpUtility.DataTypes;
using log4net;

[assembly: log4net.Config.XmlConfigurator(ConfigFile = "logconfig.xml", Watch = true)]

namespace GUIObj.Controls
{
    /// <summary>
    /// The custom table column type.
    /// </summary>
    public enum CustomTableColumnType
    {
        /// <summary>
        /// The sorter.
        /// </summary>
        Sorter,

        /// <summary>
        /// The textbox.
        /// </summary>
        Textbox,

        /// <summary>
        /// The checkbox.
        /// </summary>
        Checkbox,

        /// <summary>
        /// The combobox.
        /// </summary>
        Combobox,

        Labelbox
    }

    public enum CustomTableSortType
    {
        TEXT,
        INDEX,
        BOOL,
        CUSTOM
    }


    internal class SortHelper
    {
        public int Pos;
        public string Sortdata;
    }
    /// <summary>
    /// The custom table header list.
    /// </summary>
    public class CustomTableHeaderList
    {
        /// <summary>
        /// The _headerlist.
        /// </summary>
        private List<CustomTableHeader> _headerlist;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomTableHeaderList"/> class.
        /// </summary>
        public CustomTableHeaderList()
        {
            _headerlist = new List<CustomTableHeader>();
        }

        /// <summary>
        /// Gets or sets the headerlist.
        /// </summary>
        public List<CustomTableHeader> Headerlist
        {
            get
            {
                return _headerlist;
            }

            set
            {
                _headerlist = value;
            }
        }
    }

    /// <summary>
    /// The custom table header.
    /// </summary>
    public class CustomTableHeader
    {
        /// <summary>
        /// The _header title.
        /// </summary>
        private string _headerTitle;

        /// <summary>
        /// The _headerwidth percent.
        /// </summary>
        private int _headerwidthPercent;

        /// <summary>
        /// The _headerwidth.
        /// </summary>
        private int _headerwidth;

        /// <summary>
        /// The _column type.
        /// </summary>
        private CustomTableColumnType _columnType;

        /// <summary>
        /// The _combo box is editable.
        /// </summary>
        private bool _comboBoxIsEditable;

        /// <summary>
        /// The _combo box items.
        /// </summary>
        private List<string> _comboBoxItems;

        /// <summary>
        /// The _textbox is read only.
        /// </summary>
        private bool _textboxIsReadOnly;

        /// <summary>
        /// The _col key.
        /// </summary>
        private string _colKey;

        private string _tooltip;

        private Color _headerColor;

        private bool _nextSortAscending;

        private CustomTableSortType _sortType;

        private string _customSort;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomTableHeader"/> class.
        /// </summary>
        public CustomTableHeader()
        {
            _headerTitle = "";
            _headerwidthPercent = -1;
            _headerwidth = 50;
            _columnType = CustomTableColumnType.Textbox;
            _comboBoxIsEditable = false;
            _comboBoxItems = new List<string>();
            _textboxIsReadOnly = false;
            _headerColor=Color.FromRgb(0,0,0);
            _nextSortAscending = true;
            _sortType = CustomTableSortType.TEXT;
            _customSort = "";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomTableHeader"/> class.
        /// </summary>
        /// <param name="title">
        /// The title.
        /// </param>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <param name="width">
        /// The width.
        /// </param>
        /// <param name="widthP">
        /// The width p.
        /// </param>
        public CustomTableHeader(
            string title,
            CustomTableColumnType type = CustomTableColumnType.Textbox,
            int width = 50,
            int widthP = -1)
        {
            _headerTitle = title;
            _colKey = title.Replace(" ", "").ToUpper();
            _headerwidthPercent = widthP;
            _headerwidth = width;
            _columnType = type;
            _comboBoxIsEditable = false;
            _comboBoxItems = new List<string>();
            _textboxIsReadOnly = false;
            _headerColor = Color.FromRgb(0, 0, 0);
            _nextSortAscending = true;
            _sortType = CustomTableSortType.TEXT;
            _customSort = "";
        }

        /// <summary>
        /// Gets or sets the header title.
        /// </summary>
        public string HeaderTitle
        {
            get { return _headerTitle; }
            set { _headerTitle = value; }
        }

        /// <summary>
        /// Gets or sets the headerwidth percent.
        /// </summary>
        public int HeaderwidthPercent
        {
            get { return _headerwidthPercent; }
            set { _headerwidthPercent = value; }
        }

        /// <summary>
        /// Gets or sets the custom table column type.
        /// </summary>
        public CustomTableColumnType CustomTableColumnType
        {
            get { return _columnType; }
            set
            {
                _columnType = value;
                switch (_columnType)
                {
                    case CustomTableColumnType.Textbox:
                        _sortType = CustomTableSortType.TEXT;
                        break;
                    case CustomTableColumnType.Checkbox:
                        _sortType = CustomTableSortType.BOOL;
                        break;
                    case CustomTableColumnType.Combobox:
                        _sortType = CustomTableSortType.INDEX;
                        break;
                    case CustomTableColumnType.Labelbox:
                        _sortType = CustomTableSortType.TEXT;
                        break;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether combo box is editable.
        /// </summary>
        public bool ComboBoxIsEditable
        {
            get { return _comboBoxIsEditable; }
            set { _comboBoxIsEditable = value; }
        }

        /// <summary>
        /// Gets or sets the combo box items.
        /// </summary>
        public List<string> ComboBoxItems
        {
            get { return _comboBoxItems; }
            set { _comboBoxItems = value; }
        }

        /// <summary>
        /// Gets or sets the headerwidth.
        /// </summary>
        public int Headerwidth
        {
            get { return _headerwidth; }
            set { _headerwidth = value; }
        }

        /// <summary>
        /// The _textbox is read only.
        /// </summary>
        public bool TextboxIsReadOnly
        {
            get { return _textboxIsReadOnly; }
            set { _textboxIsReadOnly = value; }
        }

        public string ColKey
        {
            get
            {
                return _colKey;
            }
            set
            {
                _colKey = value;
            }
        }

        public string Tooltip
        {
            get { return _tooltip; }
            set { _tooltip = value; }
        }

        public Color HeaderColor
        {
            get { return _headerColor; }
            set { _headerColor = value; }
        }

        public bool NextSortAscending
        {
            get => _nextSortAscending;
            set => _nextSortAscending = value;
        }

        public CustomTableSortType SortType
        {
            get => _sortType;
            set => _sortType = value;
        }

        public string CustomSort
        {
            get => _customSort;
            set => _customSort = value;
        }
    }


    /// <summary>
    /// Interaktionslogik für UserControl1.xaml
    /// </summary>
    public partial class CustomTable
    {
        /// <summary>
        /// The log.
        /// </summary>
        private static readonly ILog Log = LogManager.GetLogger("GUIObj");
        
        #region Properties

        /// <summary>
        /// The _headers.
        /// </summary>
        private List<CustomTableHeader> _headers;

        /// <summary>
        /// The _mouse sort enable.
        /// </summary>
        private bool _mouseSortEnable;

        /// <summary>
        /// The _is down.
        /// </summary>
        private bool _isDown;

        /// <summary>
        /// The _is dragging.
        /// </summary>
        private bool _isDragging;

        /// <summary>
        /// The _start point.
        /// </summary>
        private Point _startPoint;

        /// <summary>
        /// The _real drag source.
        /// </summary>
        private UIElement _realDragSource;

        /// <summary>
        /// The _dummy drag source.
        /// </summary>
        private readonly UIElement _dummyDragSource = new UIElement();

        /// <summary>
        /// The _isloaded.
        /// </summary>
        private bool _isloaded;

        /// <summary>
        /// The _last data.
        /// </summary>
        private CustomTableData _lastData;

        /// <summary>
        /// The _selected row.
        /// </summary>
        private int _selectedRow;

        /// <summary>
        /// The last selected row.
        /// </summary>
        private int _lastSelectedRow;

        private bool _raiseTextClickEveryTimeSameRow;

        private MultiDictonary<int, int> _tabgroup;

        /// <summary>
        /// The _selected colum.
        /// </summary>
        private int _selectedColum;

        /// <summary>
        /// The _use ver tab.
        /// </summary>
        private bool _useVerTab;

        // private bool _disableCellChangeEvents=false;

        /// <summary>
        /// The _loading data.
        /// </summary>
        private bool _loadingData;

        private List<string> _additionalSortOrder;

        /// <summary>
        /// Gets or sets the headers.
        /// </summary>
        public List<CustomTableHeader> Headers
        {
            get { return (List<CustomTableHeader>)GetValue(HeadersProperty); }
            set { SetValue(HeadersProperty, value); }
        }

        /// <summary>
        /// Gets or sets the rows.
        /// </summary>
        public int Rows
        {
            get
            {
                return (int)GetValue(RowsProperty);
            }

            set
            {
                SetValue(RowsProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether vertical tab.
        /// </summary>
        public bool VerticalTab
        {
            get
            {
                return (bool)GetValue(VerticalTabProperty);
            }

            set
            {
                SetValue(VerticalTabProperty, value);
            }
        }

        /// <summary>
        /// Gets the row count.
        /// </summary>
        public int RowCount
        {
            get
            {
                return sp_rows.Children.Count;
            } 
        }

        /// <summary>
        /// Gets the column count.
        /// </summary>
        public int ColumnCount
        {
            get
            {
                return _headers.Count;
            }
        }

        /// <summary>
        /// Gets the selected colum.
        /// </summary>
        public int SelectedColum
        {
            get
            {
                return _selectedColum;
            }
        }

        public bool RaiseTextClickEveryTimeSameRow
        {
            get { return _raiseTextClickEveryTimeSameRow; }
            set { _raiseTextClickEveryTimeSameRow = value; }
        }

        public List<string> AdditionalSortOrder
        {
            get => _additionalSortOrder;
            set => _additionalSortOrder = value;
        }

        #endregion

        #region Event
        public event ComboSelectionChangeEvent OnComboBoxSelectionChange;
        public event TextChangeEvent OnTextBoxTextChange;
        public event TextDoubleClickEvent OnTextDoubleClick;
        public event TextClickEvent OnTextClick;
        public event AfterLoadEvent AfterLoad;
        public event AfterDeleteEvent AfterDelete;
        public event AfterSortEvent AfterSort;

        public delegate void ComboSelectionChangeEvent(int row,int column,string value);
        public delegate void TextChangeEvent(int row, int column, string value);

        public delegate void AfterLoadEvent();
        public delegate void AfterDeleteEvent(int deletedrow);

        public delegate void AfterSortEvent(int newrowindex);

        public delegate void TextDoubleClickEvent(int row, int column);
        public delegate void TextClickEvent(int row, int column);

        /// <summary>
        /// The cob on selection changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="selectionChangedEventArgs">
        /// The selection changed event args.
        /// </param>
        private void CobOnSelectionChanged(object sender, SelectionChangedEventArgs selectionChangedEventArgs)
        {
            if(_loadingData)
                return;
            try
            {     
                var d = ((ComboBox)sender).SelectedItem as string;
                var c = (int)((ComboBox)sender).Tag;
                var r = (int)((StackPanel)((ComboBox)sender).Parent).Tag;
                if (OnComboBoxSelectionChange != null)
                    OnComboBoxSelectionChange(r, c, d);

            }
            catch (Exception e)
            {
                Log.ErrorFormat("CT CB Sel Change: {0}", e.Message);
                Log.ErrorFormat("Trace:{0}", e);
            }
        }

        /// <summary>
        /// The tb on text changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void TbOnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (_loadingData)
                return;
            try
            {
                var d = ((TextBox)sender).Text;
                var c = (int)((TextBox)sender).Tag;
                var r = (int)((StackPanel)((TextBox)sender).Parent).Tag;
                if (OnTextBoxTextChange!=null)
                    OnTextBoxTextChange(r, c, d);
            }
            catch (Exception exception)
            {
                Log.ErrorFormat("CT TB Text Change: {0}", exception.Message);
                Log.ErrorFormat("Trace:{0}", e);
            }
            
        }

        /// <summary>
        /// The tb on mouse double click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void TbOnMouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                var c = (int)((TextBox)sender).Tag;
                var r = (int)((StackPanel)((TextBox)sender).Parent).Tag;
                //if (_lastSelectedRow == r && !_raiseTextClickEveryTimeSameRow)
                //    return;
                _lastSelectedRow = r;
                if (OnTextDoubleClick != null)
                    OnTextDoubleClick(r, c);
            }
            catch (Exception exception)
            {
                Log.ErrorFormat("CT DblClick: {0}", exception.Message);
                Log.ErrorFormat("Trace:{0}", e);
            }
            
        }

        /// <summary>
        /// The tb on mouse click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void TbOnMouseClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                var c = (int)((TextBox)sender).Tag;
                var r = (int)((StackPanel)((TextBox)sender).Parent).Tag;
                if(_lastSelectedRow==r && !_raiseTextClickEveryTimeSameRow)
                    return;
                _lastSelectedRow = r;
                if (OnTextClick != null)
                    OnTextClick(r, c);
            }
            catch (Exception exception)
            {
                Log.ErrorFormat("CT Click: {0}", exception.Message);
                Log.ErrorFormat("Trace:{0}", e);
            }

        }


        #endregion

        #region Intern Setters

        /// <summary>
        /// The headers property.
        /// </summary>
        public static readonly DependencyProperty HeadersProperty = DependencyProperty.Register("Header",typeof(List<CustomTableHeader>), typeof(CustomTable),  new FrameworkPropertyMetadata(new List<CustomTableHeader>(), OnHeaderChange));

        /// <summary>
        /// The rows property.
        /// </summary>
        public static readonly DependencyProperty RowsProperty = DependencyProperty.Register("Rows", typeof(int), typeof(CustomTable), new FrameworkPropertyMetadata(0, OnRowChange));

        /// <summary>
        /// The vertical tab property.
        /// </summary>
        public static readonly DependencyProperty VerticalTabProperty = DependencyProperty.Register("VerticalTab", typeof(bool), typeof(CustomTable), new FrameworkPropertyMetadata(false, OnVerTabChange));

        /// <summary>
        /// The on row change.
        /// </summary>
        /// <param name="d">
        /// The d.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private static void OnRowChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                CustomTable c = d as CustomTable;
                if (c != null)
                    c.UpdateRows((int)e.NewValue);
            }
            catch (Exception exception)
            {
                Log.ErrorFormat("CT Row Change: {0}", exception.Message);
                Log.ErrorFormat("Trace:{0}", e);
            }
            
        }

        /// <summary>
        /// The on header change.
        /// </summary>
        /// <param name="d">
        /// The d.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private static void OnHeaderChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                CustomTable c = d as CustomTable;
                if (c != null)
                    c.UpdateHeader((List<CustomTableHeader>)e.NewValue);
            }
            catch (Exception exception)
            {
                Log.ErrorFormat("CT Header Change: {0}", exception.Message);
                Log.ErrorFormat("Trace:{0}", e);
            }
            
        }

        /// <summary>
        /// The on ver tab change.
        /// </summary>
        /// <param name="d">
        /// The d.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private static void OnVerTabChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                CustomTable c = d as CustomTable;
                 if (c != null)
                    c.UpdateVerTab();

            }
            catch (Exception)
            {
                // ignored
            }
        }

        /// <summary>
        /// The update ver tab.
        /// </summary>
        public void UpdateVerTab()
        {
            _useVerTab = VerticalTab;
        }

        /// <summary>
        /// The update header.
        /// </summary>
        /// <param name="h">
        /// The h.
        /// </param>
        public void UpdateHeader(List<CustomTableHeader> h)
        {
            _headers = h;
            GenerateHeader();
        }

        public void UpdateHeader()
        {
            GenerateHeader();
        }

        /// <summary>
        /// The update rows.
        /// </summary>
        /// <param name="i">
        /// The i.
        /// </param>
        public void UpdateRows(int i)
        {
            CreateRows();
        } 
        #endregion

        #region Build Table

        /// <summary>
        /// The generate header.
        /// </summary>
        private void GenerateHeader()
        {
            if(!_isloaded)
                return;
            if(sp_header==null)
                return;
            sp_header.Children.Clear();
            _mouseSortEnable = false;
            if(_headers==null)
                return;
            double w = sp_main.ActualWidth - SystemParameters.VerticalScrollBarWidth-2;
            if (w < 1)
                w = 1;
            int headercount = 0;
            foreach (CustomTableHeader header in _headers)
            {
                double nw;
                if (header.HeaderwidthPercent > -1)
                    nw = (w / 100.0) * header.HeaderwidthPercent;
                else
                    nw = header.Headerwidth;
                Label lbl = new Label
                {
                    Content = header.HeaderTitle,
                    Width = nw,
                    BorderBrush = new LinearGradientBrush(Color.FromRgb(0,0,0),header.HeaderColor,new Point(0.5,0),new Point(0.5,1)  ),
                    //BorderBrush = Brushes.Black,
                    BorderThickness = new Thickness(1, 1, 1, 2),
                    ToolTip = string.IsNullOrEmpty(header.Tooltip)?header.HeaderTitle:header.Tooltip,
                    Tag = headercount++
                };
                
                if (header.CustomTableColumnType == CustomTableColumnType.Sorter)
                {
                    _mouseSortEnable = true;
                    lbl.Content = "˄˅";
                }
                else
                    lbl.MouseLeftButtonUp += Lbl_MouseLeftButtonUp;
                sp_header.Children.Add(lbl);     
            }
        }

        private void Lbl_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            int col = (int)((Label)sender).Tag;
            SortColumn(col, _headers[col].NextSortAscending);
        }

        /// <summary>
        /// The create rows.
        /// </summary>
        private void CreateRows()
        {
            DeleteRows();
            if (_headers == null)
                return;
            int r = Rows;
            for (int i = 0; i < r; i++)
            {
                AddRowI();
            }
            GenerateVerticalTab();
        }

        /// <summary>
        /// The delete rows.
        /// </summary>
        public void DeleteRows()
        {
            try
            {
                sp_rows.Children.Clear();
                _selectedRow = -1;
                _selectedColum = -1;
                _lastSelectedRow = -1;

            }
            catch (Exception e)
            {
                Log.ErrorFormat("CT Delete Rows: {0}", e.Message);
                Log.ErrorFormat("Trace:{0}", e);
            }
            
        }

        /// <summary>
        /// The delete row.
        /// </summary>
        /// <param name="row">
        /// The row.
        /// </param>
        public void DeleteRow(int row = -1)
        {
            int r = row;
            try
            {
                if (r == -1)
                    r = sp_rows.Children.Count - 1;
                if (r < 0 || r >= sp_rows.Children.Count)
                    return;
                sp_rows.Children.RemoveAt(r);
                _selectedRow = -1;
                _selectedColum = -1;
                _lastSelectedRow = -1;
            }
            catch (Exception e)
            {
                Log.ErrorFormat("CT Delete Row: {0}", e.Message);
                Log.ErrorFormat("Trace:{0}", e);
            }

            if (row != -1)
            {
                UpdateRowColor();
                RefreshRowIndex();
            }

            if (AfterDelete != null)
                AfterDelete(r);
        }

        /// <summary>
        /// The delete selected row.
        /// </summary>
        public void DeleteSelectedRow()
        {
            if (_selectedRow > -1)
            {
                DeleteRow(_selectedRow);
            }
        }

        /// <summary>
        /// The update row color.
        /// </summary>
        private void UpdateRowColor()
        {
            try
            {
                for (int i = 0; i < sp_rows.Children.Count;i++)
                {
                    ((StackPanel)sp_rows.Children[i]).Background = i % 2 == 0 ? Brushes.AntiqueWhite : Brushes.Beige;
                }
            }
            catch (Exception e)
            {
                Log.ErrorFormat("CT Update Row Color: {0}", e.Message);
                Log.ErrorFormat("Trace:{0}", e);
            }
            
       }

        /// <summary>
        /// The add rows.
        /// </summary>
        /// <param name="count">
        /// The count.
        /// </param>
        public void AddRows(int count)
        {
            for (int i = 0; i < count; i++)
            {
                AddRowI();              
            }

            GenerateVerticalTab();
        }

        /// <summary>
        /// The add row.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int AddRow()
        {
            int i = AddRowI();
            GenerateVerticalTab();
            return i;
        }

        /// <summary>
        /// The add row.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        private int AddRowI()
        {
            try
            {
                if(!_isloaded)
                    return -1;
                int rownumber = sp_rows.Children.Count;
                StackPanel sp = new StackPanel();
                double w = sp_main.ActualWidth - SystemParameters.VerticalScrollBarWidth - 2;
                if (w < 0)
                    w = 1;
                sp.Name = "R" + rownumber;
                sp.Tag = rownumber;
                //double wsp = sp_main.ActualWidth;
                //if (wsp < 0)
                //    wsp = 1;

                //sp.Width = wsp;

                sp.Orientation = Orientation.Horizontal;
                sp.HorizontalAlignment = HorizontalAlignment.Stretch;
                sp.Height = 25;
                sp.Background = rownumber % 2 == 0 ? Brushes.AntiqueWhite : Brushes.Beige;
                
                for (int i = 0; i < _headers.Count; i++)
                {
                    switch (_headers[i].CustomTableColumnType)
                    {
                        case CustomTableColumnType.Textbox:
                            TextBox tb = new TextBox();
                            if (_headers[i].HeaderwidthPercent > -1)
                                tb.Width = (w / 100.0) * _headers[i].HeaderwidthPercent;
                            else
                                tb.Width = _headers[i].Headerwidth;
                            tb.Name = "C" + i;
                            tb.Tag = i;
                            tb.Background = i % 2 == 0 ? Brushes.White : Brushes.LightGray;
                            tb.IsReadOnly = _headers[i].TextboxIsReadOnly;
                            tb.TextChanged += TbOnTextChanged;
                            tb.MouseDoubleClick += TbOnMouseDoubleClick;
                            tb.PreviewMouseDown += TbOnMouseClick;
                            tb.GotKeyboardFocus += TbGotKeyboardFocus;
                            tb.GotFocus += TbGotFocus;
                            tb.LostFocus += TbLostFocus;
                            tb.GotFocus += CtrlGotFocus;
                            sp.Children.Add(tb);
                            break;
                        case CustomTableColumnType.Labelbox:
                            Label tb2 = new Label();
                            if (_headers[i].HeaderwidthPercent > -1)
                                tb2.Width = (w / 100.0) * _headers[i].HeaderwidthPercent;
                            else
                                tb2.Width = _headers[i].Headerwidth;
                            tb2.Name = "C" + i;
                            tb2.Tag = i;

                            tb2.Background = new LinearGradientBrush(Color.FromRgb(168, 168, 168),
                                Color.FromRgb(212, 212, 212), new Point(0.5, 0), new Point(0.5, 1));
                            //tb.GotFocus += CtrlGotFocus;
                            sp.Children.Add(tb2);
                            break;
                        case CustomTableColumnType.Checkbox:
                            CheckBox cb = new CheckBox();
                            if (_style_cb != null)
                                cb.Style = _style_cb;
                            if (_headers[i].HeaderwidthPercent > -1)
                                cb.Width = (w / 100.0) * _headers[i].HeaderwidthPercent;
                            else
                                cb.Width = _headers[i].Headerwidth;
                            cb.Name = "C" + i;
                            cb.GotFocus += CtrlGotFocus;
                            cb.Tag = i;
                            cb.Background = i % 2 == 0 ? Brushes.White : Brushes.LightGray;
                            sp.Children.Add(cb);
                            break;
                        case CustomTableColumnType.Combobox:
                            ComboBox cob = new ComboBox();
                            if (_headers[i].HeaderwidthPercent > -1)
                                cob.Width = (w / 100.0) * _headers[i].HeaderwidthPercent;
                            else
                                cob.Width = _headers[i].Headerwidth;
                            cob.Name = "C" + i;
                            cob.IsEditable = _headers[i].ComboBoxIsEditable;
                            cob.SelectionChanged += CobOnSelectionChanged;
                            cob.GotFocus += CtrlGotFocus;
                            foreach (string comboBoxItem in _headers[i].ComboBoxItems)
                            {
                                cob.Items.Add(comboBoxItem);
                            }
                            cob.Tag = i;
                            sp.Children.Add(cob);
                            break;
                        case CustomTableColumnType.Sorter:
                            Label lbl = new Label();
                            if (_headers[i].HeaderwidthPercent > -1)
                                lbl.Width = (w / 100.0) * _headers[i].HeaderwidthPercent;
                            else
                                lbl.Width = _headers[i].Headerwidth;
                            lbl.Height = 0.1;

                            //lbl.Background = Brushes.Blue;
                            lbl.Name = "C" + i;
                            lbl.Tag = i;
                            sp.Children.Add(lbl);
                            break;
                            
                    }
                }
                sp_rows.Children.Add(sp);
                
                return rownumber;
            }
            catch (Exception e)
            {
                Log.ErrorFormat("CT Add Row: {0}", e.Message);
                Log.ErrorFormat("Trace:{0}", e);
            }

            return -1;
        }

        private void RefreshRowIndex()
        {
            for (int i = 0; i < sp_rows.Children.Count; i++)
            {
                ((StackPanel)sp_rows.Children[i]).Name = "R" + i;
                ((StackPanel)sp_rows.Children[i]).Tag = i;
            }
            _lastSelectedRow = -1;
            RefreshTab();
        }

        /// <summary>
        /// The ctrl got focus.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void CtrlGotFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_loadingData)
                    return;
                _selectedColum = (int)((Control)sender).Tag;
                _selectedRow = (int)((StackPanel)((Control)sender).Parent).Tag;
            }
            catch (Exception exception)
            {
                Log.ErrorFormat("CT Got Focus: {0}", exception.Message);
                Log.ErrorFormat("Trace:{0}", e);
            }
            
        }

        /// <summary>
        /// The tb lost focus.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void TbLostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                int i = (int)((TextBox)sender).Tag;
                ((TextBox)sender).Background = i % 2 == 0 ? Brushes.White : Brushes.LightGray;
            }
            catch (Exception exception)
            {
                Log.ErrorFormat("CT TB Lost Focus: {0}", exception.Message);
                Log.ErrorFormat("Trace:{0}", e);
            }
            
        }

        private void TbGotFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                ((TextBox)sender).Background = Brushes.LightGoldenrodYellow;
            }
            catch (Exception exception)
            {
                Log.ErrorFormat("CT TB Got Focus: {0}", exception.Message);
                Log.ErrorFormat("Trace:{0}", e);
            }
            
        }

        private void TbGotKeyboardFocus(object sender, System.Windows.Input.KeyboardFocusChangedEventArgs e)
        {
            try
            {
                ((TextBox)sender).SelectAll();
            }
            catch (Exception exception)
            {
                Log.ErrorFormat("CT TB HB Focus: {0}", exception.Message);
                Log.ErrorFormat("Trace:{0}", e);
            }
            
        }

        public void RefreshTab()
        {
            if (_useVerTab)
            {
                GenerateVerticalTab();
                return;
            }
            try
            {
                if (!_useVerTab)
                    return;
                int rows = sp_rows.Children.Count;
                int columns = _headers.Count;
                int tab = TabIndex + 1;
                for (int r = 0; r < rows; r++)              
                {
                    for (int c = 0; c < columns; c++)
                    {
                        StackPanel sp = (StackPanel)sp_rows.Children[r];
                        UIElement u = sp.Children[c];
                        Type t = u.GetType();
                        if (t == typeof(TextBox))
                        {
                            ((TextBox)u).TabIndex = tab++;
                        }
                        else if (t == typeof(ComboBox))
                        {

                            ((ComboBox)u).TabIndex = tab++;

                        }
                        else if (t == typeof(CheckBox))
                        {
                            ((CheckBox)u).TabIndex = tab++;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.ErrorFormat("CT Generate VT: {0}", e.Message);
                Log.ErrorFormat("Trace:{0}", e);
            }
        }
        /// <summary>
        /// The generate vertical tab.
        /// </summary>
        private void GenerateVerticalTab()
        {
            try
            {
                if(!_useVerTab)
                    return;
                int rows = sp_rows.Children.Count;
                int columns = _headers.Count;
                int tab = TabIndex + 1;
                for (int c = 0; c < columns; c++)
                {
                    for (int r = 0; r < rows; r++)
                    {
                        StackPanel sp = (StackPanel)sp_rows.Children[r];
                        if(_tabgroup.ContainsValue(c))
                            continue;
                        UIElement u = sp.Children[c];
                        Type t = u.GetType();
                        if (t == typeof(TextBox))
                        {
                            ((TextBox)u).TabIndex = tab++;
                        }
                        else if (t == typeof(ComboBox))
                        {

                            ((ComboBox)u).TabIndex = tab++;

                        }
                        else if (t == typeof(CheckBox))
                        {
                            ((CheckBox)u).TabIndex = tab++;
                        }

                        if (_tabgroup.ContainsKey(c))
                        {
                            foreach (int i in _tabgroup.GetValues(c))
                            {
                                if(i<0 || i>=sp.Children.Count)
                                    continue;
                                u = sp.Children[i];
                                t = u.GetType();
                                if (t == typeof(TextBox))
                                {
                                    ((TextBox)u).TabIndex = tab++;
                                }
                                else if (t == typeof(ComboBox))
                                {

                                    ((ComboBox)u).TabIndex = tab++;

                                }
                                else if (t == typeof(CheckBox))
                                {
                                    ((CheckBox)u).TabIndex = tab++;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.ErrorFormat("CT Generate VT: {0}", e.Message);
                Log.ErrorFormat("Trace:{0}", e);
            }
            
        }

        /// <summary>
        /// The clear.
        /// </summary>
        /// <param name="defaultTextbox">
        /// The default textbox.
        /// </param>
        /// <param name="defaultCheckbox">
        /// The default checkbox.
        /// </param>
        /// <param name="defaultCombobox">
        /// The default combobox.
        /// </param>
        public void Clear(string defaultTextbox = "", bool defaultCheckbox = false, int defaultCombobox = -2)
        {
            int rows = sp_rows.Children.Count;
            int columns = _headers.Count;
            try
            {
                _loadingData = true;
                for (int r = 0; r < rows; r++)
                {
                    StackPanel sp = (StackPanel)sp_rows.Children[r];
                    for (int c = 0; c < columns; c++)
                    {
                        UIElement u = sp.Children[c];
                        Type t = u.GetType();
                        if (t == typeof(TextBox))
                        {
                            ((TextBox)u).Text = defaultTextbox;
                        }
                        else if (t == typeof(ComboBox))
                        {
                            if (defaultCombobox > -2)
                            {
                                try
                                {
                                    ((ComboBox) u).SelectedIndex = defaultCombobox;
                                }
                                catch (Exception)
                                {
                                    // ignored
                                }
                            }

                        }
                        else if (t == typeof(CheckBox))
                        {
                            ((CheckBox)u).IsChecked=defaultCheckbox;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.ErrorFormat("CT Clear: {0}", e.Message);
                Log.ErrorFormat("Trace:{0}", e);
            }
            _selectedColum = -1;
            _selectedRow = -1;
            _lastSelectedRow = -1;
            _loadingData = true;
        }

        /// <summary>
        /// The update combo box items.
        /// </summary>
        /// <param name="column">
        /// The column.
        /// </param>
        /// <param name="items">
        /// The items.
        /// </param>
        public void UpdateComboBoxItems(int column, List<string> items)
        {
            try
            {
                if (column >= _headers.Count)
                    return;
                if (_headers[column].CustomTableColumnType != CustomTableColumnType.Combobox)
                    return;
                _headers[column].ComboBoxItems.Clear();
                _headers[column].ComboBoxItems.AddRange(items);
                foreach (StackPanel child in sp_rows.Children)
                {
                    ((ComboBox) child.Children[column]).Items.Clear();
                    foreach (string comboBoxItem in items)
                    {
                        ((ComboBox) child.Children[column]).Items.Add(comboBoxItem);
                    }
                    
                }
            }
            catch (Exception e)
            {
                Log.ErrorFormat("CT Update CB : {0}", e.Message);
                Log.ErrorFormat("Trace:{0}", e);
            }
            
        }

        #endregion

        #region Data

        public object GetData(int r, int c)
        {
            try
            {
                if (r > sp_rows.Children.Count - 1 || c > _headers.Count - 1)
                    return "";
                UIElement u = ((StackPanel) sp_rows.Children[r]).Children[c];
                Type t = u.GetType();
                if (t == typeof(TextBox))
                {
                    return ((TextBox) u).Text;
                }
                if (t == typeof(ComboBox))
                {
                    return ((ComboBox)u).Text;
                }
                if (t == typeof(CheckBox))
                {
                    return ((CheckBox)u).IsChecked==true;
                }
                if (t == typeof(Label))
                {
                    return ((Label)u).Content;
                }
            }
            catch (Exception e)
            {
                Log.ErrorFormat("CT Get Data: {0}", e.Message);
                Log.ErrorFormat("Trace:{0}", e);
            }
           
            return "";
        }

        public string GetDataString(int r, int c)
        {
            try
            {
                if (r < 0 || c < 0)
                    return "";
                if (r > sp_rows.Children.Count - 1 || c > _headers.Count - 1)
                    return "";
                UIElement u = ((StackPanel)sp_rows.Children[r]).Children[c];
                Type t = u.GetType();
                if (t == typeof(TextBox))
                {
                    return ((TextBox)u).Text;
                }
                if (t == typeof(ComboBox))
                {
                    return ((ComboBox)u).Text;
                }
                if (t == typeof(CheckBox))
                {
                    return (((CheckBox)u).IsChecked == true).ToString();
                }
            }
            catch (Exception e)
            {
                Log.ErrorFormat("CT Get Data String: {0}", e.Message);
                Log.ErrorFormat("Trace:{0}", e);
            }
           
            return "";
        }

        public string GetDataString(int r, string c)
        {
            return GetDataString(r, GetHeaderIndex(c));
        }

        public int GetDataInt(int r, int c)
        {
            try
            {
                if (r > sp_rows.Children.Count - 1 || c > _headers.Count - 1)
                    return 0;
                UIElement u = ((StackPanel)sp_rows.Children[r]).Children[c];
                Type t = u.GetType();
                string tmp;
                if (t == typeof(TextBox))
                {
                    tmp = ((TextBox) u).Text;
                    try
                    {
                        return Int32.Parse(tmp);
                    }
                    catch
                    {
                        return 0;
                    }
                }
                if (t == typeof(ComboBox))
                {
                    tmp = ((ComboBox)u).Text;
                    try
                    {
                        return Int32.Parse(tmp);
                    }
                    catch
                    {
                        return 0;
                    }
                }
                if (t == typeof(CheckBox))
                {
                    bool b=((CheckBox)u).IsChecked == true;
                    return b ? 1 : 0;
                }
            }
            catch (Exception e)
            {
                Log.ErrorFormat("CT Get Data Int: {0}", e.Message);
                Log.ErrorFormat("Trace:{0}", e);
            }
            
            return 0;
        }

        public int GetDataInt(int r, string c)
        {
            return GetDataInt(r, GetHeaderIndex(c));
        }

        public int GetDataIndex(int r, int c)
        {
            try
            {
                if (r > sp_rows.Children.Count - 1 || c > _headers.Count - 1)
                    return -1;
                UIElement u = ((StackPanel)sp_rows.Children[r]).Children[c];
                Type t = u.GetType();
                if (t == typeof(TextBox))
                {
                    return -1;
                }
                if (t == typeof(ComboBox))
                {
                    return ((ComboBox)u).SelectedIndex;
                }
                if (t == typeof(CheckBox))
                {
                    return -1;
                }
            }
            catch (Exception e)
            {
                Log.ErrorFormat("CT Get Data Index: {0}", e.Message);
                Log.ErrorFormat("Trace:{0}", e);
            }
            
            return -1;
        }

        public int GetDataIndex(int r, string c)
        {
            return GetDataIndex(r, GetHeaderIndex(c));
        }

        public void SetData(int r, string c, object data)
        {
            SetData(r, GetHeaderIndex(c), data);
        }

        public void SetData(string c, object data)
        {
            SetData(sp_rows.Children.Count - 1, GetHeaderIndex(c), data);
        }

        public void SetData(int c, object data)
        {
            SetData(sp_rows.Children.Count - 1, c, data);
        }

        public void SetData(int r, int c, object data)
        {
            try
            {
                if(r<0 || c<0)
                    return;
                if (r > sp_rows.Children.Count - 1 || c > _headers.Count - 1)
                    return;
                _loadingData = true;
                UIElement u = ((StackPanel)sp_rows.Children[r]).Children[c];
                Type t = u.GetType();
                if (t == typeof(TextBox))
                {
                    ((TextBox)u).Text = data.ToString();
                }
                else if (t == typeof(ComboBox))
                {

                    if (data is int)
                        ((ComboBox)u).SelectedIndex = (int)data;
                    else
                        ((ComboBox)u).Text = (string)data;
                }
                else if (t == typeof(CheckBox))
                {
                    ((CheckBox)u).IsChecked = (bool?)data;
                }
                if (t == typeof(Label))
                {
                    ((Label)u).Content = data.ToString();
                }
            }
            catch (Exception e)
            {
                Log.ErrorFormat("CT Set Data: {0}", e.Message);
                Log.ErrorFormat("Trace:{0}", e);
            }
            
            _loadingData = false;
        }

        /// <summary>
        /// The get all data.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string GetAllDataS(string seprow="$",string sepcol="%")
        {
            string rs = "";

            int rows = sp_rows.Children.Count;
            int columns = _headers.Count;
            try
            {
                for (int r = 0; r < rows; r++)
                {
                    if (r > 0)
                        rs = rs + seprow;
                    StackPanel sp = (StackPanel)sp_rows.Children[r];
                    var cs = "";
                    for (int c = 1; c < columns; c++)
                    {
                        if (c > 0)
                            cs = cs + sepcol;
                        UIElement u = sp.Children[c];
                        Type t = u.GetType();

                        if (t == typeof(TextBox))
                        {
                            cs=cs+((TextBox)u).Text;
                        }
                        else if (t == typeof(ComboBox))
                        {
                            cs = cs + ((ComboBox)u).Text;
                        }
                        else if (t == typeof(CheckBox))
                        {
                            cs = cs + (((CheckBox)u).IsChecked == true);
                        }
                        else if (t == typeof(Label))
                        {
                            cs = cs + "SORTER";
                        }
                    }
                    rs = rs + cs;
                }
                return rs;
            }
            catch (Exception e)
            {
                Log.ErrorFormat("CT Get All Data String: {0}", e.Message);
                Log.ErrorFormat("Trace:{0}", e);
            }

            return rs;
        }


        /// <summary>
        /// The get all data.
        /// </summary>
        /// <returns>
        /// The <see cref="CustomTableData"/>.
        /// </returns>
        public CustomTableData GetAllData()
        {
            int rows = sp_rows.Children.Count;
            int columns = _headers.Count;
            CustomTableData ctd = new CustomTableData();
            try
            {
                //ctd.Rows = rows;
                //ctd.Columns = columns;
                for (int r = 0; r < rows; r++)
                {
                    StackPanel sp = (StackPanel)sp_rows.Children[r];
                    RowItem ri = new RowItem();
                    for (int c = 0; c < columns; c++)
                    {
                        UIElement u = sp.Children[c];
                        Type t = u.GetType();
                        
                        if (t == typeof(TextBox))
                        {
                            ri.Column.Add(((TextBox) u).Text);
                        }
                        else if (t == typeof(ComboBox))
                        {
                            ri.Column.Add(((ComboBox)u).SelectedItem as string);
                        }
                        else if (t == typeof(CheckBox))
                        {
                            ri.Column.Add(((CheckBox)u).IsChecked == true);
                        }
                        else if (t == typeof(Label))
                        {
                            ri.Column.Add("SORTER");
                        }
                    }
                    ctd.Row.Add(ri);
                }
                return ctd;
            }
            catch (Exception e)
            {
                Log.ErrorFormat("CT Get Data: {0}", e.Message);
                Log.ErrorFormat("Trace:{0}", e);
            }
            return new CustomTableData();
        }

        /// <summary>
        /// The set all data.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        /// <param name="updaterowcount">
        /// The updaterowcount.
        /// </param>
        /// <param name="seprow">
        /// Row Separator
        /// </param>
        /// <param name="sepcol">
        /// Column Separator
        /// </param>
        public void SetAllData(string data, bool updaterowcount = true, string seprow = "$", string sepcol = "%")
        {
            try
            {
                string[] sepr = { seprow };
                string[] sepc = { sepcol };
                CustomTableData ctd = new CustomTableData();
                string[] rows = data.Split(sepr, StringSplitOptions.None);
                //ctd.Rows = rows.Length;
                foreach (string row in rows)
                {
                    
                    RowItem r = new RowItem();
                    string[] cols = row.Split(sepc, StringSplitOptions.None);
                    //ctd.Columns = cols.Length;
                    foreach (string col in cols)
                    {
                        r.Column.Add(col);
                    }
                    ctd.Row.Add(r);
                }
                SetAllData(ctd, updaterowcount);
            }
            catch (Exception e)
            {
                Log.ErrorFormat("CT Set All Data String: {0}", e.Message);
                Log.ErrorFormat("Trace:{0}", e);
            }          
        }

        /// <summary>
        /// The set all data.
        /// </summary>
        /// <param name="ctd">
        /// The ctd.
        /// </param>
        /// <param name="updaterowcount">
        /// The updaterowcount.
        /// </param>
        public void SetAllData(CustomTableData ctd, bool updaterowcount = true)
        {
            if (!_isloaded)
            {
                _lastData = ctd;
                return;
            }
            try          
            {
                _loadingData = true;
                int rows = sp_rows.Children.Count;
                var or = ctd.Rows;
                var oc = ctd.Columns;
                if (updaterowcount) // && rows != or)
                {
                    Rows = or;
                    CreateRows();
                    rows = or;
                }
                
                for (int r = 0; r < or; r++)
                {
                    if (r >= rows)
                        break;
                    StackPanel sp = (StackPanel)sp_rows.Children[r];
                    for (int c = 0; c < oc; c++)
                    {
                        if (c >= sp.Children.Count)
                            break;
                        UIElement u = sp.Children[c];
                        Type t = u.GetType();
                        if (t == typeof(TextBox))
                        {
                            ((TextBox)u).Text = (string)ctd.Row[r].Column[c];
                        }
                        else if (t == typeof(ComboBox))
                        {
                            try
                            {
                                ((ComboBox)u).SelectedItem = (string)ctd.Row[r].Column[c];
                            }
                            catch
                            {
                                ((ComboBox)u).SelectedIndex = 0;
                            }
                            
                        }
                        else if (t == typeof(CheckBox))
                        {
                            ((CheckBox)u).IsChecked = (bool?)ctd.Row[r].Column[c];
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.ErrorFormat("CT Set All Data: {0}", e.Message);
                Log.ErrorFormat("Trace:{0}", e);
            }

            _selectedColum = -1;
            _selectedRow = -1;
            _lastSelectedRow = -1;
            _loadingData = false;
        }

        private string GetSortString(int r, int c)
        {
            string rs = "";
            CustomTableSortType st=_headers[c].SortType;

            try
            {
                StackPanel sp = (StackPanel)sp_rows.Children[r];
                UIElement u = sp.Children[c];
                Type t = u.GetType();

                if (t == typeof(TextBox))
                {
                    if (st == CustomTableSortType.CUSTOM)
                        rs = GetCustomSortPosition(((TextBox)u).Text, _headers[c].CustomSort);
                    else
                        rs = ((TextBox)u).Text;
                }
                else if (t == typeof(ComboBox))
                {
                    if (st == CustomTableSortType.TEXT)
                        rs = ((ComboBox)u).Text;
                    else if (st == CustomTableSortType.CUSTOM)
                        rs = GetCustomSortPosition(((ComboBox)u).Text, _headers[c].CustomSort);
                    else
                        rs = ((ComboBox)u).SelectedIndex.ToString();
                }
                else if (t == typeof(CheckBox))
                {
                    rs = ((CheckBox)u).IsChecked == true?"1":"0";
                }
                else if (t == typeof(Label))
                {
                    if (_headers[c].CustomTableColumnType == CustomTableColumnType.Labelbox)
                        if (st == CustomTableSortType.CUSTOM)
                            rs = GetCustomSortPosition(((TextBox)u).Text, _headers[c].CustomSort);
                        else
                            rs = ((Label)u).Content.ToString();
                    else
                        rs = "*";
                }

                foreach (string s in _additionalSortOrder)
                {
                    int ac = GetHeaderIndex(s);
                    if(ac==c)
                        continue;
                    st = _headers[ac].SortType;
                    sp = (StackPanel)sp_rows.Children[r];
                    u = sp.Children[ac];
                    t = u.GetType();

                    if (t == typeof(TextBox))
                    {
                        if (st == CustomTableSortType.CUSTOM)
                            rs = rs + "$$" + GetCustomSortPosition(((TextBox)u).Text, _headers[ac].CustomSort);
                        else
                            rs = rs + "$$" + ((TextBox)u).Text;
                    }
                    else if (t == typeof(ComboBox))
                    {
                        if (st == CustomTableSortType.TEXT)
                            rs =rs+"$$"+ ((ComboBox)u).Text;
                        else if (st == CustomTableSortType.CUSTOM)
                            rs = rs + "$$" + GetCustomSortPosition(((ComboBox)u).Text, _headers[ac].CustomSort);
                        else
                            rs = rs + "$$" + ((ComboBox)u).SelectedIndex;
                    }
                    else if (t == typeof(CheckBox))
                    {
                        rs = rs + "$$" + (((CheckBox)u).IsChecked == true ? "1" : "0");
                    }
                    else if (t == typeof(Label))
                    {
                        if (_headers[ac].CustomTableColumnType == CustomTableColumnType.Labelbox)
                            if (st == CustomTableSortType.CUSTOM)
                                rs = rs + "$$" + GetCustomSortPosition(((TextBox)u).Text, _headers[ac].CustomSort);
                            else
                                rs = rs + "$$" + ((Label)u).Content.ToString();
                        else
                            rs = rs + "$$" + "*";
                    }
                }

                return rs;
            }
            catch (Exception e)
            {
                Log.ErrorFormat("CT Get All Data String: {0}", e.Message);
                Log.ErrorFormat("Trace:{0}", e);
            }

            return rs;
        }

        private string GetCustomSortPosition(string text, string customSort)
        {
            string[] cs = customSort.Split(";", StringSplitOptions.None);
            int p = cs.Length;
            for (int i = 0; i < cs.Length; i++)
            {
                if (text == cs[i])
                {
                    p = i;
                    break;
                }
            }

            return p.ToString();
        }

        #endregion

        private Style _style_cb;

        public CustomTable()
        {
            _isloaded = false;
            _lastData = null;
            _tabgroup = new MultiDictonary<int, int>();
            SetValue(HeadersProperty,new List<CustomTableHeader>());
            InitializeComponent();
            _style_cb = this.FindResource("CheckBoxStyle_CT_BG_MID") as Style;
            _additionalSortOrder = new List<string>();
        }

        private void sp_main_Loaded(object sender, RoutedEventArgs e)
        {
            if (_isloaded)
                return;
            _isloaded = true;
            _headers = Headers;
            GenerateHeader();
            CreateRows();
            if (_lastData != null)
                SetAllData(_lastData);
            _lastData = null;

            if (AfterLoad != null)
                AfterLoad();
        }


        #region Sort
        private void sp_rows_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (Equals(e.Source, sp_rows))
            {
            }
            else
            {
                if(!_mouseSortEnable)
                    return;
                UIElement ui = (UIElement)e.Source;
                if (ui.GetType() != typeof(StackPanel))
                    return;
                _isDown = true;
                _startPoint = e.GetPosition(sp_rows);
            }
        }

        private void sp_rows_PreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _isDown = false;
            _isDragging = false;
            if (_realDragSource != null)
                _realDragSource.ReleaseMouseCapture();
        }

        private void sp_rows_PreviewMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (_isDown)
            {
                if (_isDragging == false && ((Math.Abs(e.GetPosition(sp_rows).X - _startPoint.X) > SystemParameters.MinimumHorizontalDragDistance) ||
                                               (Math.Abs(e.GetPosition(sp_rows).Y - _startPoint.Y) > SystemParameters.MinimumVerticalDragDistance)))
                {
                    _isDragging = true;
                    _realDragSource = (UIElement)  e.Source;
                    _realDragSource.CaptureMouse();
                    DragDrop.DoDragDrop(_dummyDragSource, new DataObject("UIElement", e.Source, true), DragDropEffects.Move);
                }
            }
        }

        /// <summary>
        /// The sp_rows_ drag enter.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void sp_rows_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("UIElement"))
            {
                e.Effects = DragDropEffects.Move;
            }
        }

        /// <summary>
        /// The sp_rows_ drop.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void sp_rows_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("UIElement"))
            {             
                UIElement droptarget = e.Source as UIElement;             
                int droptargetIndex=-1, i =0;
                foreach (UIElement element in sp_rows.Children)
                {
                    if (element.Equals(droptarget))
                    {
                        droptargetIndex = i;
                        break;
                    }
                    i++;
                }
                if (droptargetIndex != -1)
                {
                    sp_rows.Children.Remove(_realDragSource);
                    sp_rows.Children.Insert(droptargetIndex, _realDragSource);
                }

                _isDown = false;
                _isDragging = false;
                _realDragSource.ReleaseMouseCapture();
                UpdateRowColor();
                RefreshRowIndex();
                if (AfterSort != null)
                    AfterSort(droptargetIndex);
            }
        }
        #endregion

        private void uc_customtable_SizeChanged(object sender, SizeChangedEventArgs e)
        {    
            if(_isloaded)
                ResizeTable();
        }

        private void ResizeTable()
        {
            if(sp_main.ActualWidth<=0)
                return;
            double w = sp_main.ActualWidth - SystemParameters.VerticalScrollBarWidth - 2;
            var width = new double[_headers.Count];
            for (int i = 0; i < _headers.Count; i++)
            {
                if (_headers[i].HeaderwidthPercent > -1)
                {
                    width[i] = (w / 100.0) * _headers[i].HeaderwidthPercent;
                    if (width[i] < 1)
                        width[i] = 1;
                    ((FrameworkElement) sp_header.Children[i]).Width = width[i];
                }
                else
                {
                     width[i]=_headers[i].Headerwidth;
                }
            }
            int rows = sp_rows.Children.Count;
            int columns = _headers.Count;
            for (int r = 0; r < rows; r++)
            {
                StackPanel sp = (StackPanel)sp_rows.Children[r];
                for (int c = 0; c < columns; c++)
                {
                    FrameworkElement u = (FrameworkElement) sp.Children[c];
                    u.Width = width[c];
                }
            }
        }

        private void sv_rows_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (e.VerticalChange == 0 && e.HorizontalChange == 0)
            {
                return;
            }
            /*if(sv_rows.ComputedVerticalScrollBarVisibility==Visibility.Visible)
                sv_header.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
            else
                sv_header.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;*/
            //Log.InfoFormat("{0} - {1}",sv_rows.HorizontalOffset,e.HorizontalOffset);
            sv_header.ScrollToHorizontalOffset(sv_rows.HorizontalOffset);
        }

        public CustomTableHeaderList GetHeaderList()
        {
            CustomTableHeaderList cthl = new CustomTableHeaderList();
            foreach (CustomTableHeader header in _headers)
            {
                cthl.Headerlist.Add(header);
            }

            return cthl;
        }

        public void SetHeaderList(CustomTableHeaderList cthl)
        {
            _headers.Clear();
            foreach (CustomTableHeader header in cthl.Headerlist)
            {
                _headers.Add(header);
            }
            GenerateHeader();
        }

        public int GetHeaderIndex(string headertitle)
        {
            for (int i = 0; i < _headers.Count; i++)
            {
                if (_headers[i].HeaderTitle == headertitle)
                    return i;
            }

            return -1;
        }

        public void AddTabGroup(int firstcolumn, params int[] additionalcolumns)
        {
            foreach (int additionalcolumn in additionalcolumns)
            {
                _tabgroup.Add(firstcolumn, additionalcolumn);
            }
        }

        public void AddTabGroup(string firstcolumn, params string[] additionalcolumns)
        {
            int i = GetHeaderIndex(firstcolumn);
            foreach (string additionalcolumn in additionalcolumns)
            {
                _tabgroup.Add(i, GetHeaderIndex(additionalcolumn));
            }
        }

        public void RemoveTabGroup(int firstcolumn)
        {
            _tabgroup.RemoveKey(firstcolumn);
        }
        public void RemoveTabGroup(string firstcolumn)
        {
            RemoveTabGroup(GetHeaderIndex(firstcolumn));
        }
        public void RemoveColumnFromTabGroup(int column)
        {
            _tabgroup.RemoveValues(column);
        }

        public void RemoveColumnFromTabGroup(string column)
        {
            RemoveColumnFromTabGroup(GetHeaderIndex(column));
        }

        public void ClearAllTabGroups()
        {
            _tabgroup.Clear();
        }

        public int FindFirstRow(string match, int c = 0)
        {
            if (c > _headers.Count - 1 || c<0)
                    return -1;
            try
            {
                for (int i = 0; i < sp_rows.Children.Count; i++)
                {
                    UIElement u = ((StackPanel)sp_rows.Children[i]).Children[c];
                    Type t = u.GetType();
                    if (t == typeof(TextBox))
                    {
                        if (((TextBox)u).Text == match)
                            return i;
                    }

                    if (t == typeof(Label))
                    {
                        if (((Label)u).Content.ToString() == match)
                            return i;
                    }
                }
            }
            catch (Exception e)
            {
                Log.ErrorFormat("CT Find Row: {0}", e.Message);
                Log.ErrorFormat("Trace:{0}", e);
            }

            return -1;
        }
        public int FindFirstRow(string match, string c)
        {
            return FindFirstRow(match, GetHeaderIndex(c));
        }

        public void SortColumn(int col, bool ascending = true)
        {
            List<SortHelper> sitems = new List<SortHelper>();
            int rows = sp_rows.Children.Count;
            for (int i = 0; i < rows; i++)
            {
                sitems.Add(new SortHelper(){Pos = i,Sortdata = GetSortString(i,col)});
            }

            sitems.Sort(Comparison);
            if(ascending==false)
                sitems.Reverse();
            _headers[col].NextSortAscending = !ascending;
            List<UIElement> n = new List<UIElement>();
            for (int i = 0; i < sitems.Count; i++)
            {
                n.Add(sp_rows.Children[sitems[i].Pos]);

            }
            sp_rows.Children.Clear();
            foreach (UIElement uiElement in n)
            {
                sp_rows.Children.Add(uiElement);
            }
            UpdateRowColor();
            RefreshRowIndex();
            if (AfterSort != null)
                AfterSort(-1);

        }

        private int Comparison(SortHelper x, SortHelper y)
        {
            if (x == null && y == null) return 0;
            if (x == null) return -1;
            if (y == null) return 1;

            var partsX = x.Sortdata.Split(new[] { "$$" }, StringSplitOptions.None);
            var partsY = y.Sortdata.Split(new[] { "$$" }, StringSplitOptions.None);

            int maxParts = Math.Max(partsX.Length, partsY.Length);

            for (int i = 0; i < maxParts; i++)
            {
                string partX = i < partsX.Length ? partsX[i] : "";
                string partY = i < partsY.Length ? partsY[i] : "";

                bool isNumX = int.TryParse(partX, out int numX);
                bool isNumY = int.TryParse(partY, out int numY);

                if (isNumX && isNumY)
                {
                    // Beide sind Zahlen → aufsteigend sortieren
                    int numCompare = numX.CompareTo(numY);
                    if (numCompare != 0) return numCompare;
                }
                else
                {
                    // Mindestens eins ist Text → alphabetisch sortieren (aufsteigend)
                    int strCompare = string.Compare(partX, partY, StringComparison.OrdinalIgnoreCase);
                    if (strCompare != 0) return strCompare;
                }
            }

            return 0;
        }
    }
}
