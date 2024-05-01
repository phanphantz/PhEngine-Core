using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PhEngine.Core
{
    public static class Services
    {
        static readonly Dictionary<Type, Component> ServiceDictionary = new Dictionary<Type, Component>();
        public static readonly Dictionary<Type, ServiceGroup> GroupDictionary = new Dictionary<Type, ServiceGroup>();

        public static event Action<IService> OnRegister; 
        public static event Action<IService> OnUnregister; 
        
        public static T Of<T>() where T : Component, IService
        {
            var type = typeof(T);
            if (ServiceDictionary.TryGetValue(type, out var existingObj) && existingObj)
                return existingObj as T;
            
            var newObj = Object.FindObjectOfType<T>();
            if (!newObj)
            {
                var newGameObj = new GameObject { name = type.Name };
                newObj = newGameObj.AddComponent<T>();
                var parent = newObj.GetGroup();
                if (parent)
                    newObj.transform.SetParent(parent.transform);
            }
            
            Register(newObj);
            return newObj;
        }

        public static void Register<T>(T obj) where T : Component, IService
        {
            ServiceDictionary[typeof(T)] = obj;
            OnRegister?.Invoke(obj);
        }

        public static void Unregister<T>(T obj) where T : Component, IService
        {
            ServiceDictionary.Remove(typeof(T));
            OnUnregister?.Invoke(obj);
        }
    }
}