using UnityEngine;

namespace PhEngine.Core.AssetBox
{
    [CreateAssetMenu(menuName = AssetBoxCreateAssetMenuPath.OBJECT_BOX_PREFIX + nameof(Texture2DBox), fileName = nameof(Texture2DBox))]
    public class Texture2DBox : ObjectBox<Texture2D>
    {
        
    }
}