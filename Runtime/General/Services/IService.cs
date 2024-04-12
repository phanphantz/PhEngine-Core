using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PhEngine.Core
{
    public interface IService
    {
        ServiceGroup GetGroup()
        {
            return GetGroupOfType<ServiceGroup>();
        }

        static T GetGroupOfType<T>() where T : ServiceGroup
        {
            var type = typeof(T);
            if (Services.GroupDictionary.TryGetValue(type, out var existingLibrary))
                return existingLibrary as T;
            
            var newObj = Object.FindObjectOfType<T>();
            if (!newObj)
            {
                var newGameObj = new GameObject { name = nameof(T) };
                newObj = newGameObj.AddComponent<T>();
                Services.GroupDictionary[type] = newObj;
            }

            return newObj;
        }
    }
    
    public interface IService<T> : IService where T : ServiceGroup
    {
        ServiceGroup IService.GetGroup()
        {
            return GetGroupOfType<T>();
        }
    }
}