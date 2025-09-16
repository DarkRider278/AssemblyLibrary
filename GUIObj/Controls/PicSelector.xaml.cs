using System;
using System.IO;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace GUIObj.Controls
{
	/// <summary>
	/// Interaction logic for PicSelector.xaml
	/// </summary>
	public partial class PicSelector
	{
	    private string _path;
        private readonly char[] _sepchar = new char[1];
		public PicSelector()
		{
            _sepchar[0] = '\\';
			InitializeComponent();

		}

        public void LoadFolder(string path)
        {
            if (!Directory.Exists(path))
                return;
            _path = path;
            string[] dirs = Directory.GetDirectories(path);
            
            lb_dir.Items.Clear();
            lb_img.Items.Clear();
            lb_dir.Items.Add("ROOT");
            foreach (string dir in dirs)
            {
                lb_dir.Items.Add(dir.Replace(_path, "").Trim(_sepchar));
            }
        }

        private void lb_dir_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lb_dir.SelectedItem == null)
                return;
            string folder = lb_dir.SelectedValue.ToString()=="ROOT" ? _path : Path.Combine(_path, lb_dir.SelectedValue.ToString());
            
            string[] files = Directory.GetFiles(folder, "*.png", SearchOption.TopDirectoryOnly);
            lb_img.Items.Clear();
            foreach (string file in files)
            {
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                bi.UriSource = new Uri(file);
                bi.EndInit();
                Image img = new Image();
                img.Width = img.Height = 80;
                img.Source = bi;
                img.ToolTip = file.Replace(folder, "").Replace(".png", "").Trim(_sepchar);
                lb_img.Items.Add(img);

            }
            /*_lastpath = folder;
            if (_selectitem)
            {
                _selectitem = false;
                foreach (Image item in lb_img.Items)
                {
                    if (item.ToolTip.ToString() == _selectedfile)
                    {
                        lb_img.SelectedItem = item;
                        break;
                    }
                    //item.IsSelected = true;
                }
            }*/
        }

        public delegate void ImageSelectedEventHandler(object sender, string image,string shortimg);
        public event ImageSelectedEventHandler ImageSelected;

        private void lb_img_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Image i = (Image) lb_img.SelectedItem;
            if (i == null)
                return;
            if (lb_dir.SelectedItem == null)
                return;
            string folder = lb_dir.SelectedValue.ToString() == "ROOT" ? _path : Path.Combine(_path, lb_dir.SelectedValue.ToString());
           
            if (ImageSelected != null)
                ImageSelected(this,Path.Combine(folder,i.ToolTip+".png"),Path.Combine(lb_dir.SelectedValue.ToString() == "ROOT" ?"":lb_dir.SelectedValue.ToString(),i.ToolTip.ToString()));
        }
	}
}