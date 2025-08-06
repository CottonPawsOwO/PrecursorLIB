using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Crafting;
using Nautilus.Utility;
using PrecursorLibrary;
using UnityEngine;
using UWE;

namespace Items.PrecursorMaterials
{
    public static class AesberiumItem
    {
        public static PrefabInfo Info { get; } = PrefabInfo.WithTechType("Aesberium", "Aesberium", "A rare metallic mineral with unprecedented durability found in mountain regions near the QEP. Essential for precursor theoric construction applications.")
            .WithIcon(PrecursorHandler.LoadAtlasSprite("Aesberium.png"));

        public static void Register()
        {
            var prefab = new CustomPrefab(Info);

            var cloneTemplate = new CloneTemplate(Info, TechType.Lead)
            {
                ModifyPrefab = ModifyAesberiumPrefab
            };

            prefab.SetGameObject(cloneTemplate);

            CraftData.pickupSoundList.Add(Info.TechType, "event:/loot/pickup_titanium");

            prefab.Register();
        }

        private static void ModifyAesberiumPrefab(GameObject prefab)
        {
            PrefabUtils.AddResourceTracker(prefab, Info.TechType);

            Renderer componentInChildren = prefab.GetComponentInChildren<Renderer>();
            if (componentInChildren != null)
            {
                componentInChildren.material.SetColor("_Color", new Color(0.4f, 0.5f, 0.6f, 0.95f));
                componentInChildren.material.SetColor("_SpecColor", new Color(0.7f, 0.8f, 0.9f, 1.0f));
                componentInChildren.material.SetColor("_GlowColor", new Color(0.3f, 0.4f, 0.5f, 1f));
                componentInChildren.material.SetFloat("_Fresnel", 0.55f);
                componentInChildren.material.SetFloat("_SpecInt", 1.3f);
                componentInChildren.material.SetFloat("_Shininess", 18f);
                componentInChildren.material.SetFloat("_GlowStrength", 0.3f);
                componentInChildren.material.SetFloat("_GlowStrengthNight", 0.5f);

                componentInChildren.transform.localScale = Vector3.one * 0.8f;
            }

            prefab.EnsureComponent<VFXSurface>().surfaceType = VFXSurfaceTypes.metal;
            var techTag = prefab.GetComponent<TechTag>() ?? prefab.AddComponent<TechTag>();
            techTag.type = Info.TechType;
        }
    }
}