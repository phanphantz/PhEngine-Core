using UnityEngine;

namespace PhEngine.Core.AssetBox
{
    [CreateAssetMenu(menuName = AssetBoxCreateAssetMenuPath.OBJECT_BOX_PREFIX + nameof(SpriteBox), fileName = nameof(SpriteBox))]
    public class SpriteBox : ObjectBox<Sprite>
    {
        
    }
}