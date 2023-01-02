using System;
using UnityEngine;

namespace PhEngine.Core.AssetBox
{
    [Serializable]
    public partial class PathRef : AssetRef
    {
        public string loadPath;
        [HideInInspector][SerializeField] string fullPath;
        
        public PathRef(string id, string loadPath, string fullPath = null) : base(id)
        {
            this.loadPath = loadPath;
            this.fullPath = fullPath;
        }
    }

}