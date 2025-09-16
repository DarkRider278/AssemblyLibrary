using System.Collections.Generic;
using System.Xml.Serialization;
using GUIObj.Controls;

namespace GUIObj.Structs
{
    public enum DataBarStrorageTyp
    {
        Textbox,
        Checkbox,
        Combobox,
        Radiobutton,
        CustomTable,
        CustomTableHeader,
        DicVariable
    }
    public class DatabarStorageItem
    {

        private DataBarStrorageTyp _elementtype;
        private string _elementname;
        private object _data;

        public DatabarStorageItem()
        {
            _elementname = "";
            _data = null;
            _elementtype=DataBarStrorageTyp.Textbox;
        }

        public DatabarStorageItem(string elementname, DataBarStrorageTyp elementtype, object data)
        {
            _elementname = elementname;
            _elementtype = elementtype;
            _data = data;
        }

        public DataBarStrorageTyp Elementtype
        {
            get { return _elementtype; }
            set { _elementtype = value; }
        }

        public string Elementname
        {
            get { return _elementname; }
            set { _elementname = value; }
        }

        public object Data
        {
            get { return _data; }
            set { _data = value; }
        }


        public object GetData()
        {
            return Data;
        }

        public bool GetDataBool()
        {
            return (bool) Data;
        }

        public string GetDataString()
        {
            return (string)Data;
        }
        
        public int GetDataInt()
        {
            return (int)Data;
        }
        public double GetDataDouble()
        {
            return (double)Data;
        }
    }

    [XmlInclude(typeof(CustomTableData))]
    [XmlInclude(typeof(CustomTableHeaderList))]
    [XmlInclude(typeof(CustomTableHeader))]
    [XmlInclude(typeof(CustomTableColumnType))]
    [XmlInclude(typeof(DataBarVariable))]
    [XmlInclude(typeof(DataBarVariableList))]
    public class DataBarStorage
    {
        private string _name;
        private string _controlname;
        private List<DatabarStorageItem> _items;

        public DataBarStorage()
        {
            _name = _controlname = "";
            Items = new List<DatabarStorageItem>();
        }

        public DataBarStorage(string name, string controlname)
        {
            Name = name;
            Controlname = controlname;
            Items=new List<DatabarStorageItem>();
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public string Controlname
        {
            get { return _controlname; }
            set { _controlname = value; }
        }

        public List<DatabarStorageItem> Items
        {
            get { return _items; }
            set { _items = value; }
        }

        public void AddItem(string elementname, DataBarStrorageTyp datatyp, object data)
        {
            DatabarStorageItem dbsi=new DatabarStorageItem(elementname,datatyp,data);
            _items.Add(dbsi);
        }
    }
}
