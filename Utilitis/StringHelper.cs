using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace HelpUtility
{
    public class StringHelper
    {
        public static string GetSubString(string scr, string start, string end, string end2 = "")
        {
            int s, e, e2 = -1;
            s = scr.IndexOf(start, StringComparison.Ordinal);
            if (s > -1)
            {
                s = s + start.Length;
                e = scr.IndexOf(end, s, StringComparison.Ordinal);
                if (end2 != "")
                    e2 = scr.IndexOf(end2, s, StringComparison.Ordinal);
                if (e > -1)
                {
                    return scr.Substring(s, e - s);
                }

                if (e2 > -1)
                {
                    return scr.Substring(s, e2 - s);
                }

                return scr.Substring(s);
            }
            return "";
        }

        public static List<string> GetSubStrings(string scr, string start, string end,bool includeStartEnd=false)
        {
            List<string> r = new List<string>();
            int s, e = 0;
            
            while (true)
            {
                s = scr.IndexOf(start, e, StringComparison.Ordinal);
                if (s > -1)
                {
                    if (!includeStartEnd)
                        s = s + start.Length;
                    e = scr.IndexOf(end, s, StringComparison.Ordinal);
                    if (e > -1)
                    {
                        if (includeStartEnd)
                            e = e + end.Length;
                        r.Add(scr.Substring(s, e - s));
                        continue;
                    }

                    r.Add(scr.Substring(s));
                    break;
                }
                else
                {
                    break;
                }
            }
            return r;
        }

        public static List<string> GetSubStringsRegEx(string scr, string start, string end, bool includeStartEnd = false)
        {
            List<string> r = new List<string>();
            int s, e = 0;
            
            Regex starts = new Regex(start, RegexOptions.IgnoreCase);
            Regex ends = new Regex(end, RegexOptions.IgnoreCase);
            MatchCollection mcs = starts.Matches(scr);
            for (int i = 0; i < mcs.Count; i++)
            {
                Match ms = mcs[i];

                s = ms.Index;
                if (!includeStartEnd)
                    s = s + ms.Length;
                Match me = ends.Match(scr, s);
                if (me.Success)
                {
                    e = me.Index;
                    if (includeStartEnd)
                        e = e + me.Length;
                    r.Add(scr.Substring(s, e - s));
                    continue;
                }

                r.Add(scr.Substring(s));
                break;
            }
            
            return r;
        }

        public static string GetSubStringIncludeStartEnd(string scr, string start, string end, string end2 = "")
        {
            int s, e, e2 = -1;
            s = scr.IndexOf(start, StringComparison.Ordinal);
            if (s > -1)
            {
                e = scr.IndexOf(end, s, StringComparison.Ordinal);
                if (end2 != "")
                    e2 = scr.IndexOf(end2, s, StringComparison.Ordinal);
                if (e > -1)
                {
                    e =e+end.Length;
                    return scr.Substring(s, e - s);
                }

                if (e2 > -1)
                {
                    e2 =e2+end2.Length;
                    return scr.Substring(s, e2 - s);
                }

                return scr.Substring(s);
            }
            return "";
        }

        public static List<string> SplitbyChars(string scr, int charcount)
        {
            var parts = new List<string>();

            for (int i = 0; i < scr.Length; i += charcount)
            {
                if (scr.Length - i >= charcount)
                    parts.Add(scr.Substring(i, charcount));
                else
                    parts.Add(scr.Substring(i, scr.Length - i));
            }
            return parts;
        }

        public static List<string> SplitbySpace(string scr, int maxlen)
        {
            var parts = new List<string>();
            string p = "";
            int pos = -1;
            string[] subs=scr.Split(new string[] { " " }, StringSplitOptions.None);
            foreach (string ss in subs)
            {
                if (ss.Length > maxlen)
                {
                    parts.Add(p);
                    p = ss + " ";
                }
                else if((p.Length+ss.Length+1)>maxlen)
                {
                    parts.Add(p);
                    p = ss+" ";
                }
                else
                {
                    p = p + ss + " ";
                }
            }

            p.Remove(p.Length - 1, 1);
            parts.Add(p);
            return parts;
        }

        private static bool NeedEscape(string src, int i)
        {
            char c = src[i];
            return c < 32 || c == '"' || c == '\\'
                   // Broken lead surrogate
                   || (c >= '\uD800' && c <= '\uDBFF' &&
                       (i == src.Length - 1 || src[i + 1] < '\uDC00' || src[i + 1] > '\uDFFF'))
                   // Broken tail surrogate
                   || (c >= '\uDC00' && c <= '\uDFFF' &&
                       (i == 0 || src[i - 1] < '\uD800' || src[i - 1] > '\uDBFF'))
                   // To produce valid JavaScript
                   || c == '\u2028' || c == '\u2029'
                   // Escape "</" for <script> tags
                   || (c == '/' && i > 0 && src[i - 1] == '<');
        }



        public static string EscapeStringJSon(string src)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            int start = 0;
            for (int i = 0; i < src.Length; i++)
                if (NeedEscape(src, i))
                {
                    sb.Append(src, start, i - start);
                    switch (src[i])
                    {
                        case '\b': sb.Append("\\b"); break;
                        case '\f': sb.Append("\\f"); break;
                        case '\n': sb.Append("\\n"); break;
                        case '\r': sb.Append("\\r"); break;
                        case '\t': sb.Append("\\t"); break;
                        case '\"': sb.Append("\\\""); break;
                        case '\\': sb.Append("\\\\"); break;
                        case '/': sb.Append("\\/"); break;
                        default:
                            sb.Append("\\u");
                            sb.Append(((int)src[i]).ToString("x04"));
                            break;
                    }
                    start = i + 1;
                }
            sb.Append(src, start, src.Length - start);
            return sb.ToString();
        }


        public static string GenerateVizArrayString(params object[] values)
        {
            return "{"+ string.Join("} {", values)+"}";
        }
        public static string GenerateVizArrayString(params int[] values)
        {
            return "{" + string.Join("} {", values) + "}";
        }
        public static string GenerateVizArrayString(params string[] values)
        {
            return "{" + string.Join("} {", values) + "}";
        }

        public static string NameFormatter(string name)
        {
            if (!String.IsNullOrEmpty(name))
            {
                return Regex.Replace(
                    name,
                    @"(?<=\b(?:mc|mac)?|_)[a-zA-ZÀ-ÿ](?<!'s\b)",
                    m => m.Value.ToUpper());
            }
            return name;
        }

        public static string NormalizeEmptyString(string v, string defaultreturn = "0")
        {
            if (string.IsNullOrEmpty(v)||string.IsNullOrWhiteSpace(v))
                return defaultreturn;
            return v;
        }
    }
}
