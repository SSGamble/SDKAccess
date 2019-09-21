/****************************************************
    文件：SimpleObjectPool.cs
	作者：TravelerTD
    日期：2019/8/7 16:54:9
	功能：池管理器
*****************************************************/

using System;
using System.Collections.Generic;

namespace TDFramework {

    /// <summary>
    /// 池的接⼝
    /// </summary>
    public interface IPool<T> {
        T Allocate();
        bool Recycle(T obj);
    }

    /// <summary>
    /// 对象⼯⼚
    /// 对象池的⼀个重要功能就是缓存，要想实现缓存就要求对象可以在对象池内部进⾏创建，所以我们要抽象出⼀个对象的⼯⼚
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IObjectFactory<T> {
        T Create();
    }

    public abstract class Pool<T> : IPool<T> {
        #region ICountObserverable
        /// <summary>
        /// Gets the current count.
        /// </summary>
        /// <value>The current count.</value>
        public int CurCount {
            get { return mCacheStack.Count; }
        }
        #endregion

        // 对象工厂
        protected IObjectFactory<T> mFactory;
        // 池容器
        protected Stack<T> mCacheStack = new Stack<T>();

        /// <summary>
        /// 池里默认 5 个对象
        /// </summary>
        protected int mMaxCount = 5;

        /// <summary>
        /// 申请对象
        /// </summary>
        /// <returns></returns>
        public virtual T Allocate() {
            return mCacheStack.Count > 0 ? mCacheStack.Pop() : mFactory.Create();
        }
        /// <summary>
        /// 回收对象
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public abstract bool Recycle(T obj);
    }

    /// <summary>
    /// 对象的创建器
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CustomObjectFactory<T> : IObjectFactory<T> {
        public CustomObjectFactory(Func<T> factoryMethod) {
            mFactoryMethod = factoryMethod;
        }

        protected Func<T> mFactoryMethod;

        public T Create() {
            return mFactoryMethod();
        }
    }

    /// <summary>
    /// 对象池实现
    /// unsafe but fast
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SimpleObjectPool<T> : Pool<T> {
        readonly Action<T> mResetMethod; // 重置对象

        public SimpleObjectPool(Func<T> factoryMethod, Action<T> resetMethod =  null, int initCount = 0) {
            mFactory = new CustomObjectFactory<T>(factoryMethod);
            mResetMethod = resetMethod;
            for (var i = 0; i < initCount; i++) {
                mCacheStack.Push(mFactory.Create());
            }
        }

        public override bool Recycle(T obj) {
            if (mResetMethod != null) {
                mResetMethod(obj);
            }
            mCacheStack.Push(obj);
            return true;
        }
    }
}