using UnityEditor;
using UnityEngine;

namespace PhEngine.Core.AssetBox.Editor
{
    public static class ObjectBoxVariantUpdater
    {
        public static void Update(IObjectBox origin)
        {
            var variants = origin.Variants;
            if (variants == null)
                return;

            UpdateAllVariants(origin, variants);
        }

        static void UpdateAllVariants(IObjectBox origin, AssetBox[] variants)
        {
            foreach (var variant in variants)
            {
                if (variant == null)
                    continue;

                UpdateVariant(origin as AssetBox, variant);
            }

            Debug.Log($"Updated {variants.Length} asset box variants.");
        }

        static void UpdateVariant(AssetBox from, AssetBox to)
        {
            var assetRefs = from.GetBaseAssetReferences();
            to.Pack(assetRefs);
            Debug.Log($"Updated '{to.name}' asset box variant by '{from.name}'.");
        }
    }
}