using UnityEngine;

namespace PhEngine.Core.AssetBox
{
    [CreateAssetMenu(menuName = AssetBoxCreateAssetMenuPath.OBJECT_BOX_PREFIX + nameof(MaterialBox), fileName = nameof(MaterialBox))]
    public class MaterialBox : ObjectBox<Material>
    {
        
    }
}