using System.Windows;

namespace GUIObj.Dialogs
{
    /// <summary>
    /// Interaktionslogik für PropertyWindow.xaml
    /// </summary>
    public partial class PropertyWindow
    {
        private readonly object _data;

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyWindow"/> class.
        /// </summary>
        public PropertyWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyWindow"/> class.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        public PropertyWindow(object data,int width=300,int height=-1):this()
        {
            _data = data;
            pg_data.SelectedObject = _data;
            if (height < 0)
            {
                int c = data.GetType().GetProperties().Length;
                c = (c * 25);
                if (c < 75)
                    c = 75;
                if (c > 250)
                    c = 250;
                c = c + 190;

                this.Height = c;
            }
            else
            {
                this.Height = height;
            }

            this.Width = width;

        }

        public object Data
        {
            get { return _data; }
        }

        private void btn_ok_Click(object sender, RoutedEventArgs e)
        {

            DialogResult = true;
        }

        private void btn_cancle_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
