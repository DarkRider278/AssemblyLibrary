using System;
using System.Collections.Generic;
using HelpUtility;

namespace GUIObj.Structs
{
    public class CustomTableData
    {
        //private int _rows;
        //private int _columns;
        private List<RowItem> _row;

        public CustomTableData()
        {
            Row = new List<RowItem>();
        }

        public int Rows
        {
            get { return Row.Count; }
        }

        public int Columns
        {
            get
            {
                if (Row.Count > 0)
                {
                    return Row[0].Column.Count;
                }
                return 0;
            }
        }

        public List<RowItem> Row
        {
            get { return _row; }
            set { _row = value; }
        }

        public string GetJson()
        {
            int rc = 0;
            string tr="";
            foreach (RowItem rowItem in _row)
            {
                var cc = 0;
                var tc = "";
                foreach (object o in rowItem.Column)
                {
                    if (tc == "")
                        tc = string.Format("\"C{0}\": \"{1}\"", cc++, EscapeString((string)o));
                    else
                        tc = string.Format("{2},\"C{0}\": \"{1}\"", cc++, EscapeString((string)o), tc);
                }
                if (tr == "")
                    tr = string.Format("\"R{0}\":{{{1}}}", rc++, tc);
                else
                    tr = string.Format("{2},\"R{0}\":{{{1}}}", rc++, tc, tr);
            }

            return string.Format("{{\"ROWS\":{0},\"COLUMNS\":{1},{2}}}", Rows, Columns, tr);
        }

        public void SetJson(string data)
        {
            if(string.IsNullOrEmpty(data))
                return;
            try
            {
                _row.Clear();
                string row;
                string col;
                int rows = int.Parse(StringHelper.GetSubString(data, "ROWS\":", ","));
                int columns = int.Parse(StringHelper.GetSubString(data, "COLUMNS\":", ","));
                for (int r = 0; r < rows; r++)
                {
                    row = StringHelper.GetSubString(data,"R" + r + "\":", "}");
                    RowItem ri=new RowItem();
                    for (int c = 0; c < columns;c++)
                    {
                        col = StringHelper.GetSubString(row, "C" + c + "\": \"", "\",","\"");
                        ri.Column.Add(UnEscapeString(col));
                    }
                    _row.Add(ri);
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }

        /// <summary>
        /// The get entry.
        /// </summary>
        /// <param name="r">
        /// The r.
        /// </param>
        /// <param name="c">
        /// The c.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string GetEntry(int r, int c)
        {
            if (r >= Rows)
                return "";
            if (c >= Columns)
                return "";
            return (string)_row[r].Column[c];
        }

        private string EscapeString(string org)
        {
            if (org == null)
                return "";
            string s=org;
            s = s.Replace("\\", "\\\\");
            s = s.Replace("\"", "\\\"");            
            return s;
        }

        private string UnEscapeString(string org)
        {
            if (org == null)
                return "";
            string s = org;
            s = s.Replace("\\\"", "\"");
            s = s.Replace("\\\\",  "\\");
            return s;
        }

    }

    public class RowItem
    {
        private List<object> _column;

        public RowItem()
        {
            Column=new List<object>();
        }

        public List<object> Column
        {
            get { return _column; }
            set { _column = value; }
        }
    }

    
}
