using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace GUIObj.SubControls
{
    /// <summary>
    /// Interaktionslogik für ComboBoxImageItem.xaml
    /// </summary>

    public partial class ComboBoxImageItem : UserControl
    {
        private string _text ;

        private string _image;

        public ComboBoxImageItem(string image, string text)
        {
            InitializeComponent();
            la_text.Content = text;
            _text = text;
            _image = image;
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.UriSource = new Uri(image);
            bi.EndInit();
            img_image.Source = bi;
        }

        public string Text
        {
            get { return _text; }
            set
            {
                _text = value;
                // NotifyPropertyChanged("Name");
            }
        }

        public string Image
        {
            get
            {
                return _image;
            }
            set
            {
                _image = value;
            }
        }
    }
}
