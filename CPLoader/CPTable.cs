using System;
using System.Collections.Generic;

namespace CPLoader
{
    internal class CpTable
    {
        private readonly string _name;
        private readonly string[] _headers;
        private readonly List<string> _values;

        public CpTable(string name,string headers)
        {
            _name = name;
            _values=new List<string>();
            _headers = SplitSemicolon(headers);
        }

        public string Name
        {
            get { return _name; }
        }

        public string[] Headers
        {
            get { return _headers; }
        }

        public List<string> Values
        {
            get { return _values; }
        }

        private string[] SplitSemicolon(string data)
        {
            string[] sep = { ";" };
            return data.Split(sep, StringSplitOptions.None);
        }
        private string[] SplitComma(string data)
        {
            string[] sep = { "," };
            return data.Split(sep, StringSplitOptions.None);
        }

        public void Add(string values)
        {
            Values.Add(values);
        }

        public int GetHeaderIndex(string header)
        {
            for (int i = 0; i < Headers.Length; i++)
            {
                if (Headers[i] == header)
                    return i;
            }
            return -1;
        }

        public List<string[]> SelectData(string what="*", string where = "",WhereSelector ws=WhereSelector.EQ,string value="")
        {
            List<int> indi=new List<int>();
            if (what == "*")
            {
                for(int i=0;i<Headers.Length;i++)
                    indi.Add(i);
            }
            else
            {
                string[] t = SplitComma(what);
                foreach (string s in t)
                {
                    int i = GetHeaderIndex(s);
                    if(i>-1)
                        indi.Add(i);
                }
            }
            List<string[]> r = new List<string[]>();
            if (where == "")
            {
                
                foreach (string s in Values)
                {
                    string[] data = SplitSemicolon(s);
                    string[] rets=new string[indi.Count];
                    int ni = 0;
                    foreach (int i in indi)
                    {
                        if (i >= data.Length)
                            rets[ni] = "";
                        else
                            rets[ni] = data[i];
                        ni++;
                    }
                    r.Add(rets);
                }

            }
            else
            {
                int wi = GetHeaderIndex(where);
                if (wi < 0)
                    return r;
                foreach (string s in Values)
                {
                    string[] data = SplitSemicolon(s);
                    if (wi >= data.Length)
                        continue;
                    if (IsIt(data[wi], value, ws))
                    {
                        string[] rets = new string[indi.Count];
                        int ni = 0;
                        foreach (int i in indi)
                        {
                            if (i >= data.Length)
                                rets[ni] = "";
                            else
                                rets[ni] = data[i];
                            ni++;
                        }

                        r.Add(rets);
                    }
                }
            }
            return r;
        }

        private bool IsIt(string v1, string v2, WhereSelector ws)
        {
            int i1, i2;
            bool b1 = Int32.TryParse(v1, out i1);
            bool b2 = Int32.TryParse(v2, out i2);
            if (b1 && b2) //Int
            {
                switch (ws)
                {
                    case WhereSelector.EQ:
                        return i1 == i2;
                    case WhereSelector.NE:
                        return i1!=i2;
                    case WhereSelector.GT:
                        return i1 > i2;
                    case WhereSelector.LT:
                        return i1 < i2;
                    case WhereSelector.GE:
                        return i1 >= i2;
                    case WhereSelector.LE:
                        return i1 <= i2;
                }
            }
            else
            {
                decimal d1, d2;
                b1 = decimal.TryParse(v1, out d1);
                b2 = decimal.TryParse(v2, out d2);
                if (b1 && b2) //deciaml
                {
                    switch (ws)
                    {
                        case WhereSelector.EQ:
                            return d1 == d2;
                        case WhereSelector.NE:
                            return d1 != d2;
                        case WhereSelector.GT:
                            return d1 > d2;
                        case WhereSelector.LT:
                            return d1 < d2;
                        case WhereSelector.GE:
                            return d1 >= d2;
                        case WhereSelector.LE:
                            return d1 <= d2;
                    }
                }
                else //string
                {
                    switch (ws)
                    {
                        case WhereSelector.EQ:
                            return v1 == v2;
                        case WhereSelector.NE:
                            return v1 != v2;
                        case WhereSelector.GT:
                            return v1.Length > v2.Length;
                        case WhereSelector.LT:
                            return v1.Length < v2.Length;
                        case WhereSelector.GE:
                            return v1.Length >= v2.Length;
                        case WhereSelector.LE:
                            return v1.Length <= v2.Length;
                    }
                }
            }

            return false;
        }

        public void Clear()
        {
            Values.Clear();
        }
    }
}
