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
    public static class IonicShardsItem
    {
        public static PrefabInfo Info { get; } = PrefabInfo.WithTechType("IonicShards", "Ionic Shards", "Incredible green glowing crystalline shards with intense ionic energy. These luminescent fragments seem related to the precursor Tecnology.")
            .WithIcon(PrecursorHandler.LoadAtlasSprite("IonicShards.png"));

        public static void Register()
        {
            var prefab = new CustomPrefab(Info);

            var cloneTemplate = new CloneTemplate(Info, TechType.Quartz)
            {
                ModifyPrefab = ModifyIonicShardsPrefab
            };

            prefab.SetGameObject(cloneTemplate);

            CraftData.pickupSoundList.Add(Info.TechType, "event:/loot/pickup_precursorioncrystal");

            prefab.SetSpawns(new WorldEntityInfo
            {
                cellLevel = LargeWorldEntity.CellLevel.Far,
                classId = Info.ClassID,
                localScale = Vector3.one,
                slotType = EntitySlot.Type.Medium,
                techType = Info.TechType
            }, new[]
            {
                new LootDistributionData.BiomeData { biome = BiomeType.DeepGrandReef_Ground, count = 3, probability = 0.4f },
                new LootDistributionData.BiomeData { biome = BiomeType.GrandReef_Wall, count = 2, probability = 0.2f },
                new LootDistributionData.BiomeData { biome = BiomeType.GrandReef_CaveFloor, count = 4, probability = 0.2f },
                new LootDistributionData.BiomeData { biome = BiomeType.GrandReef_CaveCeiling, count = 3, probability = 0.2f },
                new LootDistributionData.BiomeData { biome = BiomeType.GrandReef_Ground, count = 2, probability = 0.2f },
            });

            prefab.Register();
        }

        private static void ModifyIonicShardsPrefab(GameObject prefab)
        {
            PrefabUtils.AddResourceTracker(prefab, Info.TechType);

            Renderer componentInChildren = prefab.GetComponentInChildren<Renderer>();
            if (componentInChildren != null)
            {
                componentInChildren.material.SetColor("_Color", new Color(0.0f, 1.0f, 0.2f, 1.0f));
                componentInChildren.material.SetColor("_SpecColor", new Color(0.5f, 1.0f, 0.6f, 1.0f));
                componentInChildren.material.SetColor("_GlowColor", new Color(0.0f, 1.0f, 0.3f, 1.0f));
                componentInChildren.material.SetFloat("_Fresnel", 0.7f);
                componentInChildren.material.SetFloat("_SpecInt", 2.5f);
                componentInChildren.material.SetFloat("_Shininess", 30f);
                componentInChildren.material.SetFloat("_GlowStrength", 2.0f);
                componentInChildren.material.SetFloat("_GlowStrengthNight", 3.0f);

                componentInChildren.transform.localScale = Vector3.one * 0.8f;
            }

            Light glowLight = prefab.GetComponent<Light>();
            if (glowLight == null)
            {
                glowLight = prefab.AddComponent<Light>();
            }
            glowLight.type = LightType.Point;
            glowLight.color = new Color(0.0f, 1.0f, 0.3f, 1.0f);
            glowLight.intensity = 1.5f;
            glowLight.range = 10f;
            glowLight.shadows = LightShadows.None;

            prefab.EnsureComponent<VFXSurface>().surfaceType = VFXSurfaceTypes.glass;
            var techTag = prefab.GetComponent<TechTag>() ?? prefab.AddComponent<TechTag>();
            techTag.type = Info.TechType;
        }
    }
}