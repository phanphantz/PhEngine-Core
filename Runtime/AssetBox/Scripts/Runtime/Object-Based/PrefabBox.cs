using UnityEngine;

namespace PhEngine.Core.AssetBox
{
    [CreateAssetMenu(menuName = AssetBoxCreateAssetMenuPath.OBJECT_BOX_PREFIX + nameof(PrefabBox), fileName = nameof(PrefabBox))]
    public class PrefabBox : ObjectBox<GameObject>
    {
        
    }
}