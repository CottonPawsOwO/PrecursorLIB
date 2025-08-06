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

            var cloneTemplate = new CloneTemplate(Info, TechType.Kyanite)
            {
                ModifyPrefab = ModifyDenimiumCrystalPrefab
            };

            prefab.SetGameObject(cloneTemplate);

            CraftData.pickupSoundList.Add(Info.TechType, "event:/loot/pickup_quartz");

            prefab.SetSpawns(new WorldEntityInfo
            {
                cellLevel = LargeWorldEntity.CellLevel.Far,
                classId = Info.ClassID,
                localScale = Vector3.one,
                slotType = EntitySlot.Type.Medium,
                techType = Info.TechType
            }, new[]
            {
        new LootDistributionData.BiomeData { biome = BiomeType.Dunes_SandDune, count = 2, probability = 0.1f },
        new LootDistributionData.BiomeData { biome = BiomeType.Dunes_SandPlateau, count = 2, probability = 0.1f },
        new LootDistributionData.BiomeData { biome = BiomeType.Dunes_CaveFloor, count = 2, probability = 0.4f },
        new LootDistributionData.BiomeData { biome = BiomeType.Dunes_CaveCeiling, count = 2, probability = 0.4f },
    });

            prefab.Register();
        }

        private static void ModifyDenimiumCrystalPrefab(GameObject prefab)
        {
            PrefabUtils.AddResourceTracker(prefab, Info.TechType);

            Renderer componentInChildren = prefab.GetComponentInChildren<Renderer>();
            if (componentInChildren != null)
            {
                componentInChildren.material.SetColor("_Color", new Color(0.25f, 0.15f, 0.35f, 0.9f));

                componentInChildren.material.SetColor("_SpecColor", new Color(0.8f, 0.6f, 0.9f));

                componentInChildren.material.SetColor("_GlowColor", new Color(0.4f, 0.2f, 0.6f, 1f));

                componentInChildren.material.SetFloat("_Fresnel", 0.65f);
                componentInChildren.material.SetFloat("_SpecInt", 1.2f);
                componentInChildren.material.SetFloat("_Shininess", 15f);
                componentInChildren.material.SetFloat("_GlowStrength", 0.4f);
                componentInChildren.material.SetFloat("_GlowStrengthNight", 0.6f); 

                componentInChildren.transform.localScale = Vector3.one * 0.7f;

                PrecursorHandler.ApplyTranslucency(componentInChildren);
            }

            prefab.EnsureComponent<VFXSurface>().surfaceType = VFXSurfaceTypes.glass;
            var techTag = prefab.GetComponent<TechTag>() ?? prefab.AddComponent<TechTag>();
            techTag.type = Info.TechType;

            Plugin.Logger.LogInfo("Denimium Crystal prefab modified with beautiful translucent dark purple crystal effects! OwO");
        }
    }
}