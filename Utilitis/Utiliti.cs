using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

namespace HelpUtility
{
    public class Utility
    {
        public static Color HSVtoRGB(double h, double s, double v)
        {
            Color c;
            int r, g, b;
            if (s == 0)
            {
                r = (int)v;
                c = Color.FromArgb(r, r, r);

                return c;
            }
            h /= 60;

            int i = (int)(Math.Floor(h));
            double f = h - i;
            double p = v * (1 - s);
            double q = v * (1 - s * f);
            double t = v * (1 - s * (1 - f));
            switch (i)
            {
                case 0:
                    r = (int)(v * 255);
                    g = (int)(t * 255);
                    b = (int)(p * 255);
                    break;
                case 1:
                    r = (int)(q * 255);
                    g = (int)(v * 255);
                    b = (int)(p * 255);
                    break;
                case 2:
                    r = (int)(p * 255);
                    g = (int)(v * 255);
                    b = (int)(t * 255);
                    break;
                case 3:
                    r = (int)(p * 255);
                    g = (int)(q * 255);
                    b = (int)(v * 255);
                    break;
                case 4:
                    r = (int)(t * 255);
                    g = (int)(p * 255);
                    b = (int)(v * 255);
                    break;
                default:
                    r = (int)(v * 255);
                    g = (int)(p * 255);
                    b = (int)(q * 255);
                    break;
            }
            c = Color.FromArgb(r, g, b);
            return c;
        }

        public static IEnumerable<string> ReadLines(string path)
        {
            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, 0x1000, FileOptions.SequentialScan))
            using (var sr = new StreamReader(fs, Encoding.UTF8))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    yield return line;
                }
            }
        }

        public static double GetPan(double sx, double sy, double ex, double ey)
        {
            double w = (Math.Atan2(ey - sy, ex - sx) * 180.0) / Math.PI;
            w = w + 180;
            while (w < 0)
            {
                w = w + 360;
            }
            while (w > 360)
            {
                w = w - 360;
            }
            return w;
        }
        public static double GetTilt(double sx, double sy, double ex, double ey, double h)
        {
            double dx = Math.Abs(ex - sx);
            double dy = Math.Abs(ey - sy);
            double dist = Math.Sqrt((dx * dx) + (dy * dy));
            if (h <= 0)
                return 0;
            double w = (Math.Atan(dist / h) * 180.0) / Math.PI;
            return -(90 - w);
        }
        
        public static double GetTiltDist(double alpha, double h)
        {
            if (alpha >= 0)
                return 50;
            double w = ((alpha + 90.0) * Math.PI) / 180.0;
            return Math.Tan(w) * h;
        }

        public static List<string> GetFilesFromPath(string path, string extension, bool includesubfolder,bool replacepath,bool showsubpath, bool showextension)
        {
            List<string> result=new List<string>();
            if (!path.EndsWith("\\"))
                path = path + "\\";
            if (!Directory.Exists(path))
                return result;
            string[] temp = Directory.GetFiles(path, "*." + extension,includesubfolder?SearchOption.AllDirectories:SearchOption.TopDirectoryOnly);
            foreach (string s in temp)
            {
                string f = s;
                if (showextension == false)
                {
                    int pos = f.LastIndexOf(".");
                    if (pos > 0)
                        f = f.Remove(pos);             
                }
                
                if (replacepath)
                {              
                    f = f.Replace(path, "");
                }
                if (showsubpath == false)
                {
                    f = Path.GetFileName(f);
                }
                result.Add(f);
            }
            return result;
        }    


    }
}
