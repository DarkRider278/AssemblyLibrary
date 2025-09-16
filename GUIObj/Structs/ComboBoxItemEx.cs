namespace GUIObj.Structs
{
    

    public class ComboBoxItemEx
    {
        private int _id=-1;
        private string _data="";

        public ComboBoxItemEx()
        {
        }

        public ComboBoxItemEx(string data)
        {
            _data = data;
        }

        public int ID
        {
            get { return _id; }
            set { _id = value; }
        }

        public string Data
        {
            get { return _data; }
            set
            {
                _data = value;
            }
        }

        public override string ToString()
        {
            return _data;
        }
    }
}
