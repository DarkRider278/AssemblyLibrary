namespace GUIObj.Structs
{
    using System.IO;

    public class ComboBoxImageItemEx
    {
        private string _text;

        private string _image;

        public ComboBoxImageItemEx()
        {
        }

        public ComboBoxImageItemEx(string image, string text)
        {
            _text = text;
            _image = image;
        }

        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
                _text = value;
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

        public string GetFileName
        {
            get
            {
                return Path.GetFileName(_image);
            }
        }
    }
}
