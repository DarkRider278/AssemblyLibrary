using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using HelpUtility;

namespace GUIObj.Dialogs
{
    /// <summary>
    /// Interaction logic for TranslateWindow.xaml
    /// </summary>
    public partial class TranslateWindow : Window
    {
        private TranlaseDictonary _traslatedata;
        private bool _isclosing = false;
        public TranslateWindow()
        {
            Translatedata = new TranlaseDictonary();
            InitializeComponent();
        }

        public TranlaseDictonary Translatedata
        {
            get { return _traslatedata; }
            set { _traslatedata = value; }
        }
        public ObservableCollection<TranlaseDictonaryData> TranslateList
        {
            get { return _traslatedata.Data; }
            set { _traslatedata.Data = value; }
        }

        private void Btn_translate_save_Click(object sender, RoutedEventArgs e)
        {
            SerializeHelper.SaveXml<TranlaseDictonary>(Translatedata, "translate.xml");
        }

        private void Btn_translate_load_Click(object sender, RoutedEventArgs e)
        {
            LoadTranlateTable();
        }

        public void LoadTranlateTable()
        {
            dg_table.ItemsSource = null;
            _traslatedata = SerializeHelper.LoadXml<TranlaseDictonary>("translate.xml");
            dg_table.ItemsSource = _traslatedata.Data;
        }

        public string GetTranlation(string key)
        {
            foreach (TranlaseDictonaryData td in _traslatedata.Data)
            {
                if (td.TKey == key)
                {
                    return td.TValue;
                }
            }

            return "";
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!_isclosing)
            {
                this.Visibility = Visibility.Hidden;
                e.Cancel = true;
            }
        }

        public void CloseDlg()
        {
            _isclosing = true;
            Close();
        }
    }

    public class TranlaseDictonaryData
    {
        private string _tKey;
        private string _tValue;

        public TranlaseDictonaryData()
        {
            TKey = TValue = "";
        }

        public string TKey
        {
            get { return _tKey; }
            set { _tKey = value; }
        }

        public string TValue
        {
            get { return _tValue; }
            set { _tValue = value; }
        }
    }

    public class TranlaseDictonary
    {
        private ObservableCollection<TranlaseDictonaryData> _data;

        public TranlaseDictonary()
        {
            Data = new ObservableCollection<TranlaseDictonaryData>();
        }

        public ObservableCollection<TranlaseDictonaryData> Data
        {
            get { return _data; }
            set { _data = value; }
        }
    }
}
