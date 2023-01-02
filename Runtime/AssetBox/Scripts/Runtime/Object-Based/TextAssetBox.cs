using UnityEngine;

namespace PhEngine.Core.AssetBox
{
    [CreateAssetMenu(menuName = AssetBoxCreateAssetMenuPath.OBJECT_BOX_PREFIX + nameof(TextAssetBox), fileName = nameof(TextAssetBox))]
    public class TextAssetBox : ObjectBox<TextAsset>
    {
        
    }
}