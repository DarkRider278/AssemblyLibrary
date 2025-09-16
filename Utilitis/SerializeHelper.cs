using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using log4net;

[assembly: log4net.Config.XmlConfigurator(ConfigFile = "logconfig.xml", Watch = true)]

namespace HelpUtility
{
    

    /// <summary>
    /// The serialize helper.
    /// </summary>
    public class SerializeHelper
    {
        /// <summary>
        /// Log4Net object.
        /// </summary>
        public static ILog Log = LogManager.GetLogger("MAIN");

        /// <summary>
        /// The dic i.
        /// </summary>
        public class Dic<T,TV>
        {
            [XmlAttribute("ID")]
            public T ID;
            [XmlAttribute("Value")]
            public TV Value;
        }

        /// <summary>
        /// The serialize dictonary.
        /// </summary>
        /// <param name="dic">
        /// The dic.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string SerializeDictonary<T,TV> (Dictionary<T, TV> dic)
        {
            try
            {
                using (var stringWriter = new StringWriter())
                {
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(Dic<T,TV>[]));//,new XmlRootAttribute(){ElementName = "stats"});

                    xmlSerializer.Serialize(stringWriter, dic.Select(kv => new Dic<T,TV>() { ID = kv.Key, Value = kv.Value }).ToArray());
                    return stringWriter.ToString();
                }
            }
            catch (Exception)
            {
                // ignored
            }

            return "";
        }

        /// <summary>
        /// The de serialize dictonary.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <returns>
        /// The <see cref="Dictionary"/>.
        /// </returns>
        public static Dictionary<T,TV> DeSerializeDictonary<T,TV>(string text)
        {
            try
            {
                if (text.Trim() != "")
                    using (var stringReader = new StringReader(text))
                    {
                        XmlSerializer xmlSerializer = new XmlSerializer(typeof(Dic<T,TV>[]));
                        var dic = ((Dic<T,TV>[])xmlSerializer.Deserialize(stringReader)).ToDictionary(i => i.ID, i => i.Value);
                        return dic;
                    }
            }
            catch (Exception)
            {
                // ignored
            }

            return new Dictionary<T, TV>();
        }


        /// <summary>
        /// The load xml.
        /// </summary>
        /// <param name="file">
        /// The file.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        public static T LoadXml<T>(string file = "setting.xml")
            where T : new()
        {
            try
            {
                if (!File.Exists(file))
                {
                    return new T();
                }
                using (FileStream fstream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    XmlSerializer xs = new XmlSerializer(typeof(T));
                    return (T)xs.Deserialize(fstream);
                }
            }
            catch (Exception exc)
            {
                Log.Error("Load XML: " + exc.Message);
                return new T();
            }
        }

        public static void SaveXml<T>(T data, string file = "setting.xml")
        {
            try
            {
                using (FileStream fstream = new FileStream(file, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
                {
                    
                    XmlSerializer xs = new XmlSerializer(typeof(T));
                    using (StreamWriter sw = new StreamWriter(fstream, Encoding.UTF8))
                        using (StringWriter ssw=new Utf8StringWriter())
                    {
                        xs.Serialize(ssw, data);
                        sw.Write(ssw.ToString());
                    }

                        //xs.Serialize(fstream, data);
                }
            }
            catch (Exception exc)
            {
                Log.Error("Save XML: " + exc.Message);
            }
        }

        public class Utf8StringWriter : StringWriter
        {
            public override Encoding Encoding => Encoding.UTF8;
        }
    }
}
