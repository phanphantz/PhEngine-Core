using UnityEngine;

namespace PhEngine.Core.AssetBox
{
    [CreateAssetMenu(menuName = AssetBoxCreateAssetMenuPath.OBJECT_BOX_PREFIX + nameof(AudioClipBox), fileName = nameof(AudioClipBox))]
    public class AudioClipBox : ObjectBox<AudioClip>
    {
        
    }
}