using System;
using Object = UnityEngine.Object;

namespace PhEngine.Core.AssetBox
{
    [Serializable]
    public partial class ObjectRef<T> : AssetRef where T : Object
    {
        public T targetObject;
        public ObjectRef(string id, T targetObject) : base(id)
        {
            this.targetObject = targetObject;
        }
    }

}