using System;
using System.Collections.Generic;
using System.IO;

namespace CPLoader
{

    public class CpLoader
    {
        #region Properties
        string _dataFile;
        string _dataPath;
        string _name;
        int _version;
        string _printFormat;
        private List<CpTable> _tabels;

        public string DataFile
        {
            get { return _dataFile; }
            set { _dataFile = value; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public int Version
        {
            get { return _version; }
            set { _version = value; }
        }

        public string PrintFormat
        {
            get { return _printFormat; }
            set { _printFormat = value; }
        }

        public string DataPath
        {
            get { return _dataPath; }
            set { _dataPath = value; }
        }

        #endregion
        public CpLoader()
        {
            _dataFile = "";
            _dataPath = ".";
            Name = "";
            Version = 0;
            PrintFormat = "";
            _tabels=new List<CpTable>();
        }

        public void LoadFile()
        {
            string f = Path.Combine(_dataPath, _dataFile);
            if(!File.Exists(f))
                return;
            string[] data = File.ReadAllLines(f);
            int maxlines = data.Length;
            int currentline = 0;
            string line;
            string currentblock = "";
            bool cdb, cversion, cprint, cdef;
            cdb = cversion = cprint = cdef = false;
            string[] paraV;
            while (currentline < maxlines)
            {
                line = data[currentline].Trim();
                if (line.StartsWith("[") || line == "")
                {
                    switch (currentblock.ToLower())
                    {
                        case "[database]":
                            cdb = true;
                            break;
                        case "[version]":
                            cversion = true;
                            break;
                        case "[print]":
                            cprint = true;
                            break;
                        case "[definition]":
                            cdef = true;
                            break;
                    }
                    currentblock = line;
                }
                else
                {
                    paraV = SplitPara(line);
                    if(paraV.Length!=2)
                        continue;

                    if (currentblock.ToLower() == "[definition]")
                    {
                        CpTable t=new CpTable(paraV[0],paraV[1]);
                        _tabels.Add(t);
                    }
                    else
                    {
                         switch (paraV[0].ToLower())
                        {
                            case "databasename":
                                Name = paraV[1];
                                break;
                            case "def":
                                try
                                {
                                    Version=Int32.Parse(paraV[1]);
                                }
                                catch
                                {
                                    Version = 1;
                                }
                                break;
                            case "formatfile":
                                PrintFormat = paraV[1];
                                break;
                        }
                    }

                   
                }

                if (cdb && cversion && cprint && cdef)
                    break;
                currentline++;
            }
            UpdatedData();
        }

        public void SaveFile()
        {
            List<string> data=new List<string>();
            data.Add("[Database]");
            data.Add("databasename="+_name);
            data.Add("");
            data.Add("[Version]");
            data.Add("def=" + _version);
            data.Add("");
            data.Add("[Print]");
            data.Add("FormatFile=" + _printFormat);
            data.Add("");
            data.Add("[Definition]");

            foreach (CpTable cpTable in _tabels)
            {
                data.Add(cpTable.Name+"=" +string.Join(";",cpTable.Headers));
            }
            data.Add("");

            foreach (CpTable cpTable in _tabels)
            {
                data.Add("["+cpTable.Name + "]");
                foreach (string cpTableValue in cpTable.Values)
                {
                    data.Add(cpTableValue);
                }
                data.Add("");
            }

            

            File.WriteAllLines(Path.Combine(_dataPath,_dataFile),data);
        }

        public void UpdatedData()
        {
            string f = Path.Combine(_dataPath, _dataFile);
            if (!File.Exists(f))
                return;
            string[] data = File.ReadAllLines(f);
            int maxlines = data.Length;
            int currentline = 0;
            string line;
            string currentblock = "";
            int currenttable=-1;
            while (currentline < maxlines)
            {
                line = data[currentline].Trim();
                if (line.StartsWith("[") || line == "")
                {
                    switch (currentblock.ToLower())
                    {
                        case "[database]":
                        case "[version]":
                        case "[print]":
                        case "[definition]":
                            currentblock = "";
                            break;
                        default:
                            currentblock = line;
                            break;
                    }
                    currenttable = -1;
                }
                else if(currentblock!="")
                {
                    if (currenttable == -1)
                    {
                        currenttable = GetTable(currentblock);
                        if (currenttable != -1)
                            _tabels[currenttable].Clear();
                    }

                    if (currenttable != -1)
                        _tabels[currenttable].Add(line);
                }
                currentline++;
            }
        }

        public List<string[]> SelectData(string table,string what="*", string where = "", WhereSelector ws = WhereSelector.EQ,string value = "")
        {
            int i = GetTable(table);
            if(i<0)
                return new List<string[]>();
            return _tabels[i].SelectData(what, where, ws, value);
        }

        private string[] SplitPara(string data)
        {
            string[] sep = {"="};
            return data.Split(sep, StringSplitOptions.None);
        }
        
        private int GetTable(string name)
        {
            for (int i = 0; i < _tabels.Count; i++)
            {
                if (_tabels[i].Name == name.Replace("[", "").Replace("]", ""))
                    return i;
            }
            return -1;
        }

        public List<string> GetTableNames()
        {
            List<string> s=new List<string>();
            foreach (CpTable tabel in _tabels)
            {
                s.Add(tabel.Name);
            }
            return s;
        }

        public List<string> GetTableHeader(string table)
        {
            List<string> s = new List<string>();
            int i = GetTable(table);
            foreach (string header in _tabels[i].Headers)
            {
                s.Add(header);
            }
            return s;
        }

        public int GetTableHeaderIndex(string table,string header)
        {
            int i = GetTable(table);
            if (i > -1)
                return _tabels[i].GetHeaderIndex(header);
            return -1;
        }

        public int GetTableHeaderCount(string table)
        {
            int i = GetTable(table);
            if (i > -1)
                return _tabels[i].Headers.Length;
            return -1;
        }
        public void AddTable(string tablename, params string[] headers)
        {
            CpTable t = new CpTable(tablename, string.Join(";", headers));
            _tabels.Add(t);
        }

        public void ClearAllData()
        {
            foreach (CpTable cpTable in _tabels)
            {
                cpTable.Clear();
            }
        }

        public void ClearData(string tablename)
        {
            int i = GetTable(tablename);
            if(i>-1)
                _tabels[i].Clear();
        }

        public void AddData(string tablename, params string[] values)
        {
            int i = GetTable(tablename);
            if (i > -1)
                _tabels[i].Add(string.Join(";",values));
        }
    }

    

    public enum WhereSelector
    {
        EQ,
        NE,
        LT,
        GT,
        LE,
        GE
    }
}
