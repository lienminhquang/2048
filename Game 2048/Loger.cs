using System;
using System.IO;
using System.Xml.Serialization;

namespace ver3
{
    [Serializable]
    public static class Loger
    {
        private static void Serialize(string fileName, object data)
        {
            FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write);
            XmlSerializer sr = new XmlSerializer(typeof(GameInfo));
            sr.Serialize(fs, data);
            fs.Close();
        }

        private static T Deseiralize<T>(string fileName)
        {
            FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            XmlSerializer sr = new XmlSerializer(typeof(T));
            object o = sr.Deserialize(fs);
            fs.Close();
            return (T)o;
        }

        /// <summary>
        /// Load gameinfo
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static T LoadData<T>(string fileName)
        {
            return  Deseiralize<T>(fileName);
        }

        /// <summary>
        /// Save gameInfo
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="data"></param>
        public static void SaveData(string fileName, object data)
        {
            Loger.Serialize(fileName, data);
        }
    }
}
