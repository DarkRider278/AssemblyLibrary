using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GUIObj.Dialogs
{
    /// <summary>
    /// Interaktionslogik für ListWindow.xaml
    /// </summary>
    public partial class ListWindow
    {

        private readonly List<int> _index;
        public ListWindow(bool multiselection=true)
        {
            InitializeComponent();
            _index=new List<int>();
            if (!multiselection)
                lb_elements.SelectionMode = SelectionMode.Single;
        }

        public List<int> Index
        {
            get { return _index; }
        }

        public int FirstIndex
        {
            get
            {
                if (_index.Count == 0)
                    return -1;
                return _index[0];
            }
        }

        public void ClearList()
        {
            lb_elements.Items.Clear();
        }

        public void AddValue(string value)
        {
            lb_elements.Items.Add(value);
        }
        public void AddValues(object[] values)
        {
            foreach (object o in values)
            {
                lb_elements.Items.Add(o.ToString());
            }
            
        }

        public void SetSort()
        {
            lb_elements.Items.SortDescriptions.Add(new System.ComponentModel.SortDescription("", System.ComponentModel.ListSortDirection.Ascending));
        }

        private void lb_elements_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
           
        }

        private void btn_cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void btn_ok_Click(object sender, RoutedEventArgs e)
        {
            _index.Clear();
            foreach (object o in lb_elements.SelectedItems)
                _index.Add(lb_elements.Items.IndexOf(o));
            DialogResult = true;
            Close();
        }
    }
}
