using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;


namespace Pool
{
    public class ObjectPoolManager
    {
        private static ObjectPoolManager _instance;
        public static ObjectPoolManager Instance
        {
            get
            {
                if (_instance == null) _instance = new ObjectPoolManager();
                return _instance;
            }
        }

        private readonly Dictionary<string, AbstractObjectPool> _pools;

        /// <summary>
        /// Creates the Object Pool Manager
        /// </summary>
        public ObjectPoolManager()
        {
            _pools = new Dictionary<string, AbstractObjectPool>();
        }

        /// <summary>
        /// Creates an object pool
        /// </summary>
        /// <typeparam name="T">The type of the pool</typeparam>
        /// <param name="factoryMethod">Factory method to create objects</param>
        /// <param name="turnOnCallback">Callback to turn on the object</param>
        /// <param name="turnOffCallback">Callback to turn off the object</param>
        /// <param name="initialStock">The initial stock that will be created</param>
        /// <param name="isDynamic">If the pool is dynamic</param>
        public void AddObjectPool<T>(Func<T> factoryMethod, Action<T> turnOnCallback, Action<T> turnOffCallback, int initialStock = 0, bool isDynamic = true)
        {
            if(!_pools.ContainsKey(typeof(T)+"ByType"))
                _pools.Add(typeof(T) + "ByType", new ObjectPool<T>(factoryMethod, turnOnCallback, turnOffCallback, initialStock, isDynamic));
        }

        /// <summary>
        /// Creates an object pool
        /// </summary>
        /// <typeparam name="T">The type of the pool</typeparam>
        /// <param name="factoryMethod">Factory method to create objects</param>
        /// <param name="turnOnCallback">Callback to turn on the object</param>
        /// <param name="turnOffCallback">Callback to turn off the object</param>
        /// <param name="poolName">The pool name</param>
        /// <param name="initialStock">The initial stock that will be created</param>
        /// <param name="isDynamic">If the pool is dynamic</param>
        public void AddObjectPool<T>(Func<T> factoryMethod, Action<T> turnOnCallback, Action<T> turnOffCallback, string poolName, int initialStock = 0, bool isDynamic = true)
        {
            if (!_pools.ContainsKey(poolName))
                _pools.Add(poolName, new ObjectPool<T>(factoryMethod, turnOnCallback, turnOffCallback, initialStock, isDynamic));
        }

        /// <summary>
        /// Creates an object pool
        /// </summary>
        /// <typeparam name="T">The type of the pool</typeparam>
        /// <param name="factoryMethod">Factory method to create objects</param>
        /// <param name="turnOnCallback">Callback to turn on the object</param>
        /// <param name="turnOffCallback">Callback to turn off the object</param>
        /// <param name="initialStock">The initial stock of objects</param>
        /// <param name="isDynamic">If the pool is dynamic</param>
        public void AddObjectPool<T>(Func<T> factoryMethod, Action<T> turnOnCallback, Action<T> turnOffCallback, List<T> initialStock, bool isDynamic = true) where T : AbstractObjectPool, new()
        {
            if (!_pools.ContainsKey(typeof(T) + "ByType"))
                _pools.Add(typeof(T) + "ByType", new ObjectPool<T>(factoryMethod, turnOnCallback, turnOffCallback, initialStock, isDynamic));
        }

        /// <summary>
        /// Creates an object pool
        /// </summary>
        /// <typeparam name="T">The type of the pool</typeparam>
        /// <param name="factoryMethod">Factory method to create objects</param>
        /// <param name="turnOnCallback">Callback to turn on the object</param>
        /// <param name="turnOffCallback">Callback to turn off the object</param>
        /// <param name="initialStock">The initial stock of the objects</param>
        /// <param name="poolName">The pool name</param>
        /// <param name="isDynamic">If the pool is dynamic</param>
        public void AddObjectPool<T>(Func<T> factoryMethod, Action<T> turnOnCallback, Action<T> turnOffCallback, List<T> initialStock, string poolName, bool isDynamic = true) where T : AbstractObjectPool, new()
        {
            if (!_pools.ContainsKey(poolName))
                _pools.Add(poolName, new ObjectPool<T>(factoryMethod, turnOnCallback, turnOffCallback, initialStock, isDynamic));
        }

        /// <summary>
        /// Adds an existing Object Pool if it doesn't have already one of that type
        /// </summary>
        /// <param name="pool">Pool to be added</param>
        public void AddObjectPool(AbstractObjectPool pool)
        {
            if (!_pools.ContainsKey(pool.GetType() + "ByType"))
                _pools.Add(pool.GetType() + "ByType", pool);
        }

        /// <summary>
        /// Adds an existing Object Pool if it doesn't have already one with that index
        /// </summary>
        /// <param name="pool">Pool to be added</param>
        /// <param name="poolName">Pool name</param>
        public void AddObjectPool(AbstractObjectPool pool, string poolName)
        {
            if (_pools.ContainsKey(poolName))
                _pools.Add(poolName, pool);
        }

        /// <summary>
        /// Gets an Object Pool
        /// </summary>
        /// <typeparam name="T">Type of the object pool</typeparam>
        /// <returns>The object pool that contains T type</returns>
        public ObjectPool<T> GetObjectPool<T>()
        {
            return (ObjectPool<T>)_pools[typeof(T) + "ByType"];
        }

        /// <summary>
        /// Gets an Object Pool
        /// </summary>
        /// <typeparam name="T">Type of the object pool</typeparam>
        /// <param name="poolName">The name of the pool</param>
        /// <returns>The object pool that contains T type</returns>
        public ObjectPool<T> GetObjectPool<T>(string poolName)
        {
            return (ObjectPool<T>)_pools[poolName];
        }

        /// <summary>
        /// Gets an object from a pool
        /// </summary>
        /// <typeparam name="T">Type of the object</typeparam>
        /// <returns>The object of T type</returns>
        public T GetObject<T>()
        {
            return ((ObjectPool<T>)_pools[typeof(T) + "ByType"]).GetObject();
        }

        /// <summary>
        /// Gets an object from a pool
        /// </summary>
        /// <typeparam name="T">Type of the object</typeparam>
        /// <param name="poolName">The name of the pool</param>
        /// <returns>The object of T type</returns>
        public T GetObject<T>(string poolName)
        {
            return ((ObjectPool<T>)_pools[poolName]).GetObject();
        }
        
        /// <summary>
        /// Returns object(type) from pool
        /// </summary>
        /// <param name="poolName"></param>
        /// <param name="item"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetObjectAuto<T>(string poolName, T item) where T : MonoBehaviour
        {
            if (_pools.ContainsKey(poolName) == false)
            {
                ObjectPoolManager.Instance.AddObjectPool(
                    ()=> BaseFactor(item),
                    (x)=> x.gameObject.SetActive(true),
                    (x)=> x.gameObject.SetActive(false),
                    poolName,
                    0, true);
            }

            return ((ObjectPool<T>)_pools[poolName]).GetObject();
        }
        
        public T GetObjectAuto<T>(string poolName, T item, Action<T> turnOnCallback, Action<T> turnOffCallback) where T : MonoBehaviour
        {
            if (!_pools.ContainsKey(poolName))
            {
                ObjectPoolManager.Instance.AddObjectPool(
                    ()=> BaseFactor(item),
                    turnOnCallback,
                    turnOffCallback,
                    poolName,
                    0, true);
            }

            return ((ObjectPool<T>)_pools[poolName]).GetObject();
        }
        
        T BaseFactor<T>(T item) where T : MonoBehaviour
        {
            T x = GameObject.Instantiate(item);
            x.gameObject.name = item.name;
            return x;
        }
        
        public T GetObjectAuto<T>(T prefab, Action<T> turnOnCallback, Action<T> turnOffCallback) where T : MonoBehaviour
        {
            if (!_pools.ContainsKey(prefab.name))
            {
                ObjectPoolManager.Instance.AddObjectPool(
                    ()=> BaseFactor(prefab),
                    turnOnCallback,
                    turnOffCallback,
                    prefab.name,
                    0, true);
            }
            
            T BaseFactor<T>(T item) where T : MonoBehaviour
            {
                T x = GameObject.Instantiate(item);
                x.gameObject.name = item.name;
                return x;
            }

            return ((ObjectPool<T>)_pools[prefab.name]).GetObject();
        }

        private MonoBehaviour tempMono;
        private Object tempObj;


        
        public T GetObjectAuto<T>(string poolName, GameObject item) where T : Object
        {
            if (!_pools.ContainsKey(poolName))
            {
                ObjectPoolManager.Instance.AddObjectPool(
                    ()=> GameObject.Instantiate(item),
                    (x)=> x.gameObject.SetActive(true),
                    (x)=> x.gameObject.SetActive(false),
                    poolName,
                    5, true);
            }

            return ((ObjectPool<T>)_pools[poolName]).GetObject();
        }


        /// <summary>
        /// Returns an object to the pool
        /// </summary>
        /// <typeparam name="T">Type of the object</typeparam>
        /// <param name="o">The object of T type</param>
        public void ReturnObject<T>(T o)
        {
            ((ObjectPool<T>)_pools[typeof(T) + "ByType"]).ReturnObject(o);
        }

        /// <summary>
        /// Returns an object to the pool
        /// </summary>
        /// <typeparam name="T">Type of the object</typeparam>
        /// <param name="o">The object of T type</param>
        /// <param name="poolName">Pool name</param>
        public void ReturnObject<T>(string poolName, T o)
        {
            ((ObjectPool<T>)_pools[poolName]).ReturnObject(o);
        }

        /// <summary>
        /// Removes a pool
        /// </summary>
        /// <typeparam name="T">Type of the pool</typeparam>
        public void RemovePool<T>()
        {
            _pools[typeof(T) + "ByType"] = null;
        }

        /// <summary>
        /// Removes a pool
        /// </summary>
        /// <param name="poolName">Pool name</param>
        public void RemovePool(string poolName)
        {
            _pools[poolName] = null;
        }
    }
}
/////////////////////////
///
/// /////


namespace Pool
{

    public class ObjectPool<T> : AbstractObjectPool
    {
        private readonly List<T> _currentStock;
        private readonly Func<T> _factoryMethod;
        private readonly bool _isDynamic;
        private readonly Action<T> _turnOnCallback;
        private readonly Action<T> _turnOffCallback;

        /// <summary>
        /// Creates an object pool
        /// </summary>
        /// <param name="factoryMethod">Factory method to create objects</param>
        /// <param name="turnOnCallback">Callback to turn on the object</param>
        /// <param name="turnOffCallback">Callback to turn off the object</param>
        /// <param name="initialStock">The initial stock that will be created</param>
        /// <param name="isDynamic">If the pool is dynamic</param>
        public ObjectPool(Func<T> factoryMethod, Action<T> turnOnCallback, Action<T> turnOffCallback, int initialStock = 0, bool isDynamic = true)
        {
            _factoryMethod = factoryMethod;
            _isDynamic = isDynamic;

            _turnOffCallback = turnOffCallback;
            _turnOnCallback = turnOnCallback;

            _currentStock = new List<T>();

            for (var i = 0; i < initialStock; i++)
            {
                var o = _factoryMethod();
                _turnOffCallback(o);
                _currentStock.Add(o);
            }
        }

        /// <summary>
        /// Creates an object pool with a given initial stock
        /// </summary>
        /// <param name="factoryMethod">Factory method to create objects</param>
        /// <param name="turnOnCallback">Callback to turn on the object</param>
        /// <param name="turnOffCallback">Callback to turn off the object</param>
        /// <param name="initialStock">The initial stock of objects</param>
        /// <param name="isDynamic">If the pool is dynamic</param>
        public ObjectPool(Func<T> factoryMethod, Action<T> turnOnCallback, Action<T> turnOffCallback, List<T> initialStock, bool isDynamic = true)
        {
            _factoryMethod = factoryMethod;
            _isDynamic = isDynamic;

            _turnOffCallback = turnOffCallback;
            _turnOnCallback = turnOnCallback;

            _currentStock = initialStock;
        }

        /// <summary>
        /// Gets an object from the pool. If there aren't any, and the pool is dynamic, it will create a new one. If there aren't any, and the pool isn't dynamic, it will return the default of T
        /// </summary>
        /// <returns>Object from the pool</returns>
        public T GetObject()
        {
            var result = default(T);
            if (_currentStock.Count > 0)
            {
                result = _currentStock[0];
                _currentStock.RemoveAt(0);
            }
            else if (_isDynamic)
                result = _factoryMethod();
            _turnOnCallback(result);
            return result;
        }

        /// <summary>
        /// Method to return objects
        /// </summary>
        /// <param name="o">The object you want to return</param>
        public void ReturnObject(T o)
        {
            _turnOffCallback(o);
            _currentStock.Add(o);
        }
    }
}
/////////////////////////////////////////////////////////////
///
///

namespace Pool
{
    //This class works as an empty interface for the Object Pool Manager

    public abstract class AbstractObjectPool
    {
        
    }
}