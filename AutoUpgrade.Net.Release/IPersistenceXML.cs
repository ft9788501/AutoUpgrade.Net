using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace AutoUpgrade.Net.Release
{
    /// <summary>
    /// 对象xml序列化
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class IPersistenceXML<T> where T : IPersistenceXML<T>, new()
    {
        /// <summary> 根据类型获取持久化文件的名称
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private static string GetPersistenceFileName()
        {
            return AppDomain.CurrentDomain.BaseDirectory + typeof(T).Name;
        }
        /// <summary> 是否存在默认路径的XML文档
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public bool ExistDefaultXMLConfig()
        {
            string path = GetPersistenceFileName() + ".xml";
            return File.Exists(path);
        }
        /// <summary> 将对象保存为XML文档
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="persistenceXML"></param>
        public void SaveAsXML()
        {
            string path = GetPersistenceFileName() + ".xml";
            using (FileStream fileStream = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                new XmlSerializer(typeof(T)).Serialize(fileStream, this);
            }
        }
        /// <summary> 加载XML文件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="createInstanceWhenNonExists">如果文件不存在，指定一个创建函数</param>
        /// <returns>返回对象</returns>
        public static T LoadFromXML(Func<T> createInstanceWhenNonExists = null)
        {
            string path = GetPersistenceFileName() + ".xml";
            if (!File.Exists(path))
            {
                return createInstanceWhenNonExists?.Invoke() ?? new T();
            }
            using (StreamReader streamReader = new StreamReader(path))
            {
                try
                {
                    return new XmlSerializer(typeof(T)).Deserialize(streamReader) as T;
                }
                catch(Exception ex)
                {
                    return new T();
                }
            }
        }
    }
}
