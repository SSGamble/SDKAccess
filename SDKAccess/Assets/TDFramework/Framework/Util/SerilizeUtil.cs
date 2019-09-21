/****************************************************
    文件：SerilizeUtil.cs
	作者：TravelerTD
    日期：2019/08/26 16:17:38
	功能：序列化
*****************************************************/

using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using UnityEngine;

namespace TDFramework {

    public class SerilizeUtil : MonoBehaviour {

        /// <summary>
        /// XML 正向序列化(类 转 XML)
        /// </summary>
        /// <param name="path">序列化文件的路径</param>
        /// <param name="obj">类</param>
        /// <returns></returns>
        public static bool Xmlserialize(string path, System.Object obj) {
            try {
                using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite)) {
                    using (StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.UTF8)) {
                        // xml 的命名空间
                        //XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
                        //namespaces.Add(string.Empty, string.Empty);
                        XmlSerializer xs = new XmlSerializer(obj.GetType());
                        xs.Serialize(sw, obj);
                    }
                }
                return true;
            }
            catch (Exception e) {
                Debug.LogError("此类无法转换成 xml " + obj.GetType() + "，" + e);
            }
            return false;
        }

        /// <summary>
        /// XML 反向序列化(读 XML，XML 转 类)，编辑器使用
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path">xml 的路径</param>
        /// <returns></returns>
        public static T XmlDeserialize<T>(string path) where T : class {
            T t = default(T);
            try {
                using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite)) {
                    XmlSerializer xs = new XmlSerializer(typeof(T));
                    t = (T)xs.Deserialize(fs);
                }
            }
            catch (Exception e) {
                Debug.LogError("此 xml 无法转成 类: " + path + "，" + e);
            }
            return t;
        }

        /// <summary>
        /// XML 反向序列化(读 XML，XML 转 类)
        /// </summary>
        /// <param name="path">xml 的路径</param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static System.Object XmlDeserialize(string path, Type type) {
            System.Object obj = null;
            try {
                using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite)) {
                    XmlSerializer xs = new XmlSerializer(type);
                    obj = xs.Deserialize(fs);
                }
            }
            catch (Exception e) {
                Debug.LogError("此 xml 无法转成 类: " + path + "，" + e);
            }
            return obj;
        }

        /// <summary>
        /// 类 转 二进制
        /// </summary>
        /// <param name="path"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool BinarySerilize(string path, System.Object obj) {
            try {
                using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite)) {
                    BinaryFormatter bf = new BinaryFormatter();
                    bf.Serialize(fs, obj);
                }
                return true;
            }
            catch (Exception e) {
                Debug.LogError("此类无法转换成二进制 " + obj.GetType() + "," + e);
            }
            return false;
        }

        /// <summary>
        /// 读取二进制（二进制 转 类）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        public static T BinaryDeserilize<T>(string path) where T : class {
            T t = default(T);
            ////TextAsset textAsset = ResourceManager.Instance.LoadResource<TextAsset>(path);
            //TextAsset textAsset = UnityEditor.AssetDatabase.LoadAssetAtPath<TextAsset>(path);
            //if (textAsset == null) {
            //    UnityEngine.Debug.LogError("cant load TextAsset: " + path);
            //    return null;
            //}
            //try {
            //    using (MemoryStream stream = new MemoryStream(textAsset.bytes)) {
            //        BinaryFormatter bf = new BinaryFormatter();
            //        t = (T)bf.Deserialize(stream);
            //    }
            //    //ResourceManager.Instance.ReleaseResouce(path, true);
            //}
            //catch (Exception e) {
            //    Debug.LogError("load TextAsset exception: " + path + "," + e);
            //}
            return t;
        }
    }
}