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
    public static class DenimiumCrystalItem
    {
        public static PrefabInfo Info { get; } = PrefabInfo.WithTechType("DenimiumCrystal", "Denimium Crystal", "Dark semi-purplish crystals that when pressed together form Reinforcement Alloy - the perfect natural replacement for lithium in deep-sea construction at extreme depths.")
            .WithIcon(PrecursorHandler.LoadAtlasSprite("Denimium.png"));

        public static void Register()
        {
            var prefab = new CustomPrefab(Info);

            // Use Kyanite as the base model - perfect crystalline structure!
            var cloneTemplate = new CloneTemplate(Info, TechType.Kyanite)
            {
                ModifyPrefab = ModifyDenimiumCrystalPrefab
            };

            prefab.SetGameObject(cloneTemplate);

            // Crystal pickup sound (like kyanite)
            CraftData.pickupSoundList.Add(Info.TechType, "event:/loot/pickup_kyanite");

            prefab.Register();
        }

        private static void ModifyDenimiumCrystalPrefab(GameObject prefab)
        {
            // Add resource tracker
            PrefabUtils.AddResourceTracker(prefab, Info.TechType);

            // Get the renderer to apply dark purplish crystal effects
            Renderer componentInChildren = prefab.GetComponentInChildren<Renderer>();
            if (componentInChildren != null)
            {
                // Dark semi-purplish crystal appearance
                componentInChildren.material.SetColor("_Color", new Color(0.25f, 0.15f, 0.35f, 0.9f)); // Dark purple with slight transparency

                // Specular with purplish highlight
                componentInChildren.material.SetColor("_SpecColor", new Color(0.8f, 0.6f, 0.9f)); // Purple-white specular

                // Subtle purple glow
                componentInChildren.material.SetColor("_GlowColor", new Color(0.4f, 0.2f, 0.6f, 1f)); // Purple glow

                // Crystal properties for that perfect denimium look
                componentInChildren.material.SetFloat("_Fresnel", 0.65f);
                componentInChildren.material.SetFloat("_SpecInt", 1.2f);
                componentInChildren.material.SetFloat("_Shininess", 15f);
                componentInChildren.material.SetFloat("_GlowStrength", 0.4f);
                componentInChildren.material.SetFloat("_GlowStrengthNight", 0.6f); // More glow at night for that mystical feel

                // Perfect denimium crystal size
                componentInChildren.transform.localScale = Vector3.one * 0.7f;

                // Apply the incredible translucency effects! ✨
                PrecursorHandler.ApplyTranslucency(componentInChildren);
            }

            // Glass surface type for proper crystal interaction sounds
            prefab.EnsureComponent<VFXSurface>().surfaceType = VFXSurfaceTypes.glass;

            // Ensure correct tech type
            var techTag = prefab.GetComponent<TechTag>() ?? prefab.AddComponent<TechTag>();
            techTag.type = Info.TechType;

            Plugin.Logger.LogInfo("Denimium Crystal prefab modified with beautiful translucent dark purple crystal effects! OwO");
        }
    }
}
