using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PhEngine.Core.AssetBox
{
    public class AssetLoadValidator<T> where T: Object
    {
        Object Target { get; }
        AssetLoadRequest Request { get; }

        public AssetLoadValidator(Object target, AssetLoadRequest request)
        {
            Target = target;
            Request = request;
        }

        public T GetValidObject()
        {
            var assetAsType = Target as T;
            return assetAsType != null ? 
                LoadDirectly(assetAsType) : 
                LoadIndirectly();
        }

        T LoadDirectly(T assetAsType)
        {
            return !IsInstantiatable(assetAsType) ? 
                assetAsType : 
                LoadByRequestMode(assetAsType);
        }

        T LoadByRequestMode(T assetAsType)
        {
            return Request.Mode switch
            {
                AssetLoadMode.Asset => assetAsType,
                AssetLoadMode.SceneObject => Instantiate(assetAsType, true),
                AssetLoadMode.UI => Instantiate(assetAsType, false),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        static bool IsInstantiatable(T assetAsType)
        {
            return (assetAsType is GameObject || assetAsType is Component);
        }

        T Instantiate(T assetAsType, bool isInstantiateInWorldSpace)
        {
            return Object.Instantiate(assetAsType, Request.ParentTransform, isInstantiateInWorldSpace);
        }

        T LoadIndirectly()
        {
            var gameObject = Target as GameObject;
            return gameObject != null ?
                LoadFromGameObject(gameObject) : 
                LoadFromComponent(Target);
        }

        T LoadFromGameObject(GameObject assetAsGameObject)
        {
            var component = assetAsGameObject.GetComponent<T>();
            if (component != null)
                return LoadDirectly(component);

            PhDebug.LogError<AssetLoadValidator<T>>(AssetLoaderError.GetAssetTypeMismatchErrorMessage<T>(Request.Id));
            return default;
        }

        T LoadFromComponent(Object assetObject)
        {
            var plainComponent = assetObject as Component;
            if (plainComponent == null)
            {
                PhDebug.LogError<AssetLoadValidator<T>>(AssetLoaderError.GetAssetTypeMismatchErrorMessage<T>(Request.Id));
                return default;
            }

            var targetComponent = plainComponent.GetComponent<T>();
            if (targetComponent != null)
                return LoadDirectly(targetComponent);

            PhDebug.LogError<AssetLoadValidator<T>>(AssetLoaderError.GetAssetTypeMismatchErrorMessage<T>(Request.Id));
            return default;
        }
    }
}