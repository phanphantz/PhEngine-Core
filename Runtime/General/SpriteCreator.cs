using UnityEngine;

namespace PhEngine.Core
{
    public static class SpriteCreator
    {
        public static Sprite CreateFromTexture(Texture texture)
        {
            return CreateFromTexture(texture as Texture2D, new Rect(0, 0, texture.width, texture.height),
                new Vector2(0.5f, 0.5f));
        }

        public static Sprite CreateFromTexture(Texture texture, Rect rect, Vector2 pivot)
        {
            return Sprite.Create(texture as Texture2D, rect, pivot);
        }
    }
}