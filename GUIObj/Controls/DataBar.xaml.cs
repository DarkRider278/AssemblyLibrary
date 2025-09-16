// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataBar.xaml.cs" company="ST Spoeraervice GmbH">
//   2021
// </copyright>
// <summary>
//   Interaktionslogik für DataBar.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
using System.Xml.Serialization;
using GUIObj.Structs;
using log4net;
using Path = System.IO.Path;

namespace GUIObj.Controls
{
    /// <summary>
    /// The data bar variable.
    /// </summary>
    public class DataBarVariable
    {
        /// <summary>
        /// The _varname.
        /// </summary>
        private string _varname;

        /// <summary>
        /// The _data.
        /// </summary>
        private object _data;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataBarVariable"/> class.
        /// </summary>
        public DataBarVariable()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataBarVariable"/> class.
        /// </summary>
        /// <param name="varname">
        /// The varname.
        /// </param>
        /// <param name="data">
        /// The data.
        /// </param>
        public DataBarVariable(string varname, object data)
        {
            _varname = varname;
            _data = data;
        }

        /// <summary>
        /// Gets or sets the varname.
        /// </summary>
        public string Varname
        {
            get
            {
                return _varname;
            }

            set
            {
                _varname = value;
            }
        }

        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        public object Data
        {
            get
            {
                return _data;
            }

            set
            {
                _data = value;
            }
        }
    }

    /// <summary>
    /// The data bar variable list.
    /// </summary>
    public class DataBarVariableList
    {
        /// <summary>
        /// The _var.
        /// </summary>
        private List<DataBarVariable> _var;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataBarVariableList"/> class.
        /// </summary>
        public DataBarVariableList()
        {
            _var = new List<DataBarVariable>();
        }

        /// <summary>
        /// Gets or sets the var.
        /// </summary>
        public List<DataBarVariable> Var
        {
            get
            {
                return _var;
            }

            set
            {
                _var = value;
            }
        }
    }

    /// <summary>
    /// Interaktionslogik für DataBar.xaml
    /// </summary>
    public partial class DataBar
    {
        /// <summary>
        /// The log.
        /// </summary>
        private static readonly ILog Log = LogManager.GetLogger("GUIObj");

        /// <summary>
        /// The log.
        /// </summary>
        private static readonly ILog LogDbg = LogManager.GetLogger("GUIObjDBG");

        /// <summary>
        /// The _items.
        /// </summary>
        private readonly List<DataBarStorage> _items;

        /// <summary>
        /// The _itemsfilename.
        /// </summary>
        private readonly List<string> _itemsfilename;

        /// <summary>
        /// The _savegroup.
        /// </summary>
        private string _savegroup;

        /// <summary>
        /// The _del confirm.
        /// </summary>
        private bool _delConfirm;

        /// <summary>
        /// The _variables.
        /// </summary>
        private DataBarVariableList _variables;
        
        /// <summary>
        /// The save group property.
        /// </summary>
        public static readonly DependencyProperty SaveGroupProperty = DependencyProperty.Register(
            "Savegroup",
            typeof(string),
            typeof(DataBar),
            new FrameworkPropertyMetadata("", OnGroupChange));

        /// <summary>
        /// The del confirm property.
        /// </summary>
        public static readonly DependencyProperty DelConfirmProperty = DependencyProperty.Register("DelConfirm",
            typeof(bool), typeof(DataBar), new FrameworkPropertyMetadata(false, OnDelConfirmChange));

        /// <summary>
        /// The del show property.
        /// </summary>
        public static readonly DependencyProperty DelShowProperty = DependencyProperty.Register("DelShow",
            typeof(bool), typeof(DataBar), new FrameworkPropertyMetadata(false, OnDelShowChange));

        /// <summary>
        /// Initializes a new instance of the <see cref="DataBar"/> class.
        /// </summary>
        public DataBar()
        {
            DelConfirm = false;
            Savegroup = "NONAME";
            InitializeComponent();
            _items = new List<DataBarStorage>();
            _itemsfilename = new List<string>();
            _variables = new DataBarVariableList();
        }

        /// <summary>
        /// The after data load event.
        /// </summary>
        public delegate void AfterDataLoadEvent();

        /// <summary>
        /// The before data save event.
        /// </summary>
        public delegate void BeforeDataSaveEvent();

        /// <summary>
        /// The after data load.
        /// </summary>
        public event AfterDataLoadEvent AfterDataLoad;

        /// <summary>
        /// The before data save.
        /// </summary>
        public event BeforeDataSaveEvent BeforeDataSave;

        /// <summary>
        /// Gets or sets the savegroup.
        /// </summary>
        public string Savegroup
        {
            get { return (string)GetValue(SaveGroupProperty); }
            set { SetValue(SaveGroupProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether del confirm.
        /// </summary>
        public bool DelConfirm
        {
            get { return (bool)GetValue(DelConfirmProperty); }
            set { SetValue(DelConfirmProperty, value); }
        }

        /// <summary>
        /// The on group change.
        /// </summary>
        /// <param name="d">
        /// The d.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private static void OnGroupChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataBar b = d as DataBar;
            if (b != null)
                b.UpdateSaveGroup((string)e.NewValue);
        }

        /// <summary>
        /// The on del confirm change.
        /// </summary>
        /// <param name="d">
        /// The d.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private static void OnDelConfirmChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataBar b = d as DataBar;
            if (b != null)
                b.UpdateDelConfirm((bool)e.NewValue);
        }

        private static void OnDelShowChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataBar b = d as DataBar;
            if (b != null)
                b.btn_delete.Visibility = (bool)e.NewValue ? Visibility.Visible : Visibility.Hidden;

        }

        /// <summary>
        /// The update save group.
        /// </summary>
        /// <param name="n">
        /// The n.
        /// </param>
        public void UpdateSaveGroup(string n)
        {
            _savegroup = n;
        }

        /// <summary>
        /// The update del confirm.
        /// </summary>
        /// <param name="n">
        /// The n.
        /// </param>
        public void UpdateDelConfirm(bool n)
        {
            _delConfirm = n;
        }

        /// <summary>
        /// The exits entry.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        private int ExitsEntry(string name)
        {
            for (int i = 0; i < _items.Count; i++)
            {
                if (_items[i].Name == name)
                    return i;
            }

            return -1;
        }

        /// <summary>
        /// The btn_save_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void BtnSaveClick(object sender, RoutedEventArgs e)
        {
            
            string name = cb_name.Text.Trim();
            LogDbg.Debug("DB Save " + name);
            if (name == "")
                return;
            _savegroup = Savegroup;
            if (BeforeDataSave != null)
                BeforeDataSave();
            try
            {
                if (!Directory.Exists(_savegroup))
                    Directory.CreateDirectory(_savegroup);
                DataBarStorage dbs;
                string filename;
                int i  = ExitsEntry(name);
                if (i < 0)
                {
                    dbs = new DataBarStorage(name, Name); 
                    DateTime dt = DateTime.Now;
                    filename = string.Format(
                        "{0:0000}{1:00}{2:00}_{3:00}{4:00}{5:00}_{6}.dbsi",
                        dt.Year,
                        dt.Month,
                        dt.Day,
                        dt.Hour,
                        dt.Minute,
                        dt.Second,
                        name.Replace(" ", "_").Replace(":", "~").Replace("\\", "_").Replace("/", "_"));
                }
                else
                {
                    dbs = _items[i];
                    dbs.Items.Clear();
                    filename = _itemsfilename[i];
                }

                GetValue(ref dbs, Parent as Panel);
                XmlWriterSettings xws = new XmlWriterSettings { Indent = true, IndentChars = "  " };
                XmlWriter xw = XmlWriter.Create(Path.Combine(_savegroup, filename), xws);
                XmlSerializer xs = new XmlSerializer(typeof(DataBarStorage));

                xs.Serialize(xw, dbs);
                xw.Close();
                if (i < 0)
                {
                    _itemsfilename.Add(filename);
                    _items.Add(dbs);
                    cb_name.Items.Add(new ComboBoxItemEx(name));
                }
            }
            catch (Exception exc)
            {
                Log.ErrorFormat("DB Save: {0}", exc.Message);
                Log.ErrorFormat("Trace:{0}", e);
            }
        }

        /// <summary>
        /// The load saved data.
        /// </summary>
        public void LoadSavedData()
        {
            try
            {
                if (string.IsNullOrEmpty(_savegroup))
                    return;
                if (!Directory.Exists(_savegroup))
                    Directory.CreateDirectory(_savegroup);
                _items.Clear();
                _itemsfilename.Clear();
                cb_name.Items.Clear();
                string[] files = Directory.GetFiles(_savegroup, "*.dbsi");
                foreach (string file in files)
                {
                    try
                    {
                        XmlSerializer xs = new XmlSerializer(typeof(DataBarStorage));
                        DataBarStorage dbs = (DataBarStorage)xs.Deserialize(new StreamReader(file));
                        _items.Add(dbs);
                        _itemsfilename.Add(Path.GetFileName(file));
                        cb_name.Items.Add(new ComboBoxItemEx(dbs.Name));
                    }
                    catch (Exception e2)
                    {
                        Log.ErrorFormat("DB Load: {0}", e2.Message);
                        Log.ErrorFormat("Trace:{0}", e2);
                    }
                }
            }
            catch (Exception e)
            {
                Log.ErrorFormat("DB Load: {0}", e.Message);
                Log.ErrorFormat("Trace:{0}", e);
            }
        }

        /// <summary>
        /// The get value.
        /// </summary>
        /// <param name="dbs">
        /// The dbs.
        /// </param>
        /// <param name="panel">
        /// The panel.
        /// </param>
        private void GetValue(ref DataBarStorage dbs, Panel panel)
        {
            try
            {           
                for (int i = 0; i < panel.Children.Count; i++)
                {
                    if (panel.Children[i].GetType() == typeof(Grid))
                    {
                        GetValue(ref dbs, panel.Children[i] as Panel);
                    }
                    else if (panel.Children[i].GetType() == typeof(TextBox))
                    {
                        dbs.AddItem(
                            ((TextBox)panel.Children[i]).Name,
                            DataBarStrorageTyp.Textbox,
                            ((TextBox)panel.Children[i]).Text);
                    }
                    else if (panel.Children[i].GetType() == typeof(ComboBox))
                    {
                        dbs.AddItem(((ComboBox)panel.Children[i]).Name, DataBarStrorageTyp.Combobox, ((ComboBox)panel.Children[i]).Text);
                    }
                    else if (panel.Children[i].GetType() == typeof(CheckBox))
                    {
                        dbs.AddItem(
                            ((CheckBox)panel.Children[i]).Name,
                            DataBarStrorageTyp.Checkbox,
                            ((CheckBox)panel.Children[i]).IsChecked == true);
                    }
                    else if (panel.Children[i].GetType() == typeof(RadioButton))
                    {
                        dbs.AddItem(
                            ((RadioButton)panel.Children[i]).Name,
                            DataBarStrorageTyp.Radiobutton,
                            ((RadioButton)panel.Children[i]).IsChecked == true);
                    }
                    else if (panel.Children[i].GetType() == typeof(CustomTable))
                    {
                        dbs.AddItem(((CustomTable)panel.Children[i]).Name, DataBarStrorageTyp.CustomTableHeader, ((CustomTable)panel.Children[i]).GetHeaderList());
                        dbs.AddItem(((CustomTable)panel.Children[i]).Name, DataBarStrorageTyp.CustomTable, ((CustomTable)panel.Children[i]).GetAllData());
                    }
                }

                dbs.AddItem("###VARIABLE###", DataBarStrorageTyp.DicVariable, _variables);
            }
            catch (Exception e)
            {
                Log.ErrorFormat("DB Get Data: {0}", e.Message);
                Log.ErrorFormat("Trace:{0}", e);
            }
        }

        /// <summary>
        /// The set value.
        /// </summary>
        /// <param name="dbs">
        /// The dbs.
        /// </param>
        /// <param name="panel">
        /// The panel.
        /// </param>
        private void SetValue(DataBarStorage dbs, Panel panel)
        {
            try
            {          
                foreach (DatabarStorageItem storageItem in dbs.Items)
                {
                    if (storageItem.Elementtype == DataBarStrorageTyp.DicVariable)
                    {
                        _variables = (DataBarVariableList)storageItem.Data;
                        continue;
                    }

                    object d = panel.FindName(storageItem.Elementname);
                    if (d == null)
                        continue;
                    switch (storageItem.Elementtype)
                    {
                        case DataBarStrorageTyp.Textbox:
                            ((TextBox)d).Text = storageItem.GetDataString();
                            break;
                        case DataBarStrorageTyp.Checkbox:
                            ((CheckBox)d).IsChecked = storageItem.GetDataBool();
                            break;
                        case DataBarStrorageTyp.Combobox:
                            ((ComboBox)d).Text = storageItem.GetDataString();
                            break;
                        case DataBarStrorageTyp.Radiobutton:
                            ((RadioButton)d).IsChecked = storageItem.GetDataBool();
                            break;
                        case DataBarStrorageTyp.CustomTable:
                            ((CustomTable)d).SetAllData((CustomTableData)storageItem.GetData());
                            break;
                        case DataBarStrorageTyp.CustomTableHeader:
                            ((CustomTable)d).SetHeaderList((CustomTableHeaderList)storageItem.GetData());
                            break;
                    }
                }

                if (AfterDataLoad != null)
                    AfterDataLoad();
            }
            catch (Exception e)
            {
                Log.ErrorFormat("DB Set Data: {0}", e.Message);
                Log.ErrorFormat("Trace:{0}", e);
            }
        }


        /// <summary>
        /// The btn delete click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void BtnDeleteClick(object sender, RoutedEventArgs e)
        {           
            try
            {      
                string name = cb_name.Text;
                LogDbg.Debug("DB Del " + name);
                if (name == "")
                    return;
                
                ComboBoxItemEx comboBoxItem = cb_name.Items.OfType<ComboBoxItemEx>().FirstOrDefault(x => x.Data == name);
                if (comboBoxItem != null)
                {
                    int index = cb_name.Items.IndexOf(comboBoxItem);
                    if (index < 0)
                        return;
                    if (_delConfirm)
                    {
                        if (MessageBox.Show(
                                string.Format("Delete entry:{0} ?", name),
                                "Delete Confirm",
                                MessageBoxButton.YesNo,
                                MessageBoxImage.Warning) == MessageBoxResult.No)
                        {
                            return;
                        }
                    }

                    _items.RemoveAt(index);
                    if (File.Exists(Path.Combine(_savegroup, _itemsfilename[index])))
                        File.Delete(Path.Combine(_savegroup, _itemsfilename[index]));
                    _itemsfilename.RemoveAt(index);
                    cb_name.SelectedIndex = -1;
                    cb_name.Items.RemoveAt(index);
                }
            }
            catch (Exception exception)
            {
                Log.ErrorFormat("DB Delete: {0}", exception.Message);
                Log.ErrorFormat("Trace:{0}", e);
            }
        }

        /// <summary>
        /// The btn refresh click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void BtnRefreshClick(object sender, RoutedEventArgs e)
        {
            try
            {
                string name = cb_name.Text;
                LogDbg.Debug("DB Refresh " + name);
                if (name == "")
                    return;
                ComboBoxItemEx comboBoxItem = cb_name.Items.OfType<ComboBoxItemEx>().FirstOrDefault(x => x.Data == name);
                if (comboBoxItem != null)
                {
                    int index = cb_name.Items.IndexOf(comboBoxItem);

                    // int index = cb_name.Items.IndexOf(new ComboBoxItemEx(name));
                    if (index < 0)
                        return;
                    SetValue(_items[index], Parent as Panel);
                }
            }
            catch (Exception exception)
            {
                Log.ErrorFormat("DB Refresh: {0}", exception.Message);
                Log.ErrorFormat("Trace:{0}", e);
            }           
        }

        /// <summary>
        /// The cb name selection changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void CbNameSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = cb_name.SelectedIndex;
            if (index < 0)
                return;
            SetValue(_items[index], Parent as Panel);
        }

        /// <summary>
        /// The uc databar loaded.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void UcDatabarLoaded(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(Name))
                Name = Uid;
            _savegroup = Savegroup;
            LoadSavedData();
        }

        /// <summary>
        /// The add variable.
        /// </summary>
        /// <param name="varname">
        /// The varname.
        /// </param>
        /// <param name="data">
        /// The data.
        /// </param>
        public void AddVariable(string varname, object data)
        {
            foreach (DataBarVariable variable in _variables.Var)
            {
                if (variable.Varname == varname)
                {
                    variable.Data = data;
                    return;
                }
            }

            _variables.Var.Add(new DataBarVariable(varname, data));
        }

        /// <summary>
        /// The get vaiable.
        /// </summary>
        /// <param name="varname">
        /// The varname.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public object GetVariable(string varname)
        {
            foreach (DataBarVariable variable in _variables.Var)
            {
                if (variable.Varname == varname)
                {
                    return variable.Data;
                }
            }

            return null;
        }

        private void Cb_name_DropDownOpened(object sender, EventArgs e)
        {
            LoadSavedData();
        }
    }
}
