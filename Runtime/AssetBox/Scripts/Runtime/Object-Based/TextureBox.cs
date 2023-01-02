using UnityEngine;

namespace PhEngine.Core.AssetBox
{
    [CreateAssetMenu(menuName = AssetBoxCreateAssetMenuPath.OBJECT_BOX_PREFIX + nameof(TextureBox), fileName = nameof(TextureBox))]
    public class TextureBox : ObjectBox<Texture>
    {
        
    }
}