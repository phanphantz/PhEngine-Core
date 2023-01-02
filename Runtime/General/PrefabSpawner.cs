using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PhEngine.Core
{
    public abstract class PrefabSpawner<T> : MonoBehaviour where T : Object
    {
        protected abstract bool IsInstantiateInWorldSpace { get; }
        
        [SerializeField] T prefab;
        public T Prefab => prefab;
        public void SetPrefab(T targetPrefab) => prefab = targetPrefab;

        [SerializeField] Transform parentTransform;
        public Transform ParentTransform => parentTransform;
        public void SetParentTransform(Transform parent) => parentTransform = parent;

        [SerializeField] bool isHidePrefab = true;
        public bool IsHidePrefab => isHidePrefab;
        public void SetIsHidePrefab(bool value) => isHidePrefab = value;

        public IReadOnlyCollection<T> ElementList => elementList.AsReadOnly();
        [SerializeField] List<T> elementList = new List<T>();
        
        public void PrepareElements(int count) 
        {
            CleanUp();
            if (count <= 0)
                return;
            
            var currentUICount = elementList.Count;
            if (currentUICount < count)
                CreateNewElements(count - currentUICount);
        }

        void CleanUp()
        {
            RemoveNullElements();
            TryHidePrefab();
        }
        
        public void RemoveNullElements()
        {
            elementList.RemoveAll(element => element == null);
        }

        void TryHidePrefab()
        {
            if (!isHidePrefab)
                return;

            var prefabObject = GetGameObjectFrom(prefab);
            if (prefabObject)
                prefabObject.gameObject.SetActive(false);
            
            elementList.Remove(prefab);
        }

        public void CreateNewElements(int count)
        {
            for (var i = 0; i < count; i++)
                CreateNewElements();
        }
        
        public T CreateNewElements()
        {
            var newObject = Spawn(prefab, parentTransform, IsInstantiateInWorldSpace);
            elementList.Add(newObject);
            return newObject;
        }

        public void ForEachElements(Action<T> action)
        {
            elementList.ForEach(action.Invoke);
        }
        
        public static List<T> SpawnListOf(T prefab , int amount, Transform parent = null , bool instantiateInWorldSpace = false)
        {
            var resultList = new List<T>();
            for(var i = 1 ; i <= amount ; i++)
            {
                var newT = Spawn(prefab, parent, instantiateInWorldSpace);
                resultList.Add(newT);
            }

            return resultList;
        }

        public static T Spawn(T prefab, Transform parent = null, bool instantiateInWorldSpace = false)
        {
            var tryGetGameObject = GetGameObjectFrom(prefab);
            var newObject = CreateGameObject();
            var newT = newObject.GetComponent<T>();

            GameObject CreateGameObject()
            {
                return Instantiate(tryGetGameObject, parent, instantiateInWorldSpace);
            }

            return newT;
        }

        static GameObject GetGameObjectFrom(Object obj)
        {
            var tryGetGameObject = obj as GameObject;
            if (tryGetGameObject)
                return tryGetGameObject;

            var tryGetMonoBehaviour = obj as MonoBehaviour;
            return tryGetMonoBehaviour ? tryGetMonoBehaviour.gameObject : null;
        }

        public static GameObject GetGameObjectFrom(T t)
        {
            return GetGameObjectFrom(t as Object);
        }
    }
}