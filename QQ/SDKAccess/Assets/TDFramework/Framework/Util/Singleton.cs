/****************************************************
    文件：Singleton.cs
	作者：TravelerTD
    日期：2019/8/7 17:56:57
	功能：单例模板
*****************************************************/

using System;
using System.Reflection;

namespace TDFramework {
    public abstract class Singleton<T> where T : Singleton<T> {

        protected static T mInstance = null;
        protected Singleton() { }

        public static T Instance {
            get {
                if (mInstance == null) {
                    // 先获取所有非 public 的构造⽅法
                    var ctors = typeof(T).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic);
                    // 从 ctors 中获取⽆参的构造⽅法
                    var ctor = Array.Find(ctors, c => c.GetParameters().Length == 0);
                    if (ctor == null) {
                        throw new Exception("Non-public ctor() not found!");
                    }
                    // 调⽤构造⽅法
                    mInstance = ctor.Invoke(null) as T;
                }
                return mInstance;
            }
        }
    }
}