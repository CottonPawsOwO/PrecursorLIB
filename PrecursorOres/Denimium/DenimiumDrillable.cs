using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Utility;
using PrecursorLibrary;
using System;
using UnityEngine;
using UWE;

namespace Items.PrecursorMaterials
{
    internal class DenimiumDrillable : BindablePrefab
    {
        public DenimiumDrillable()
        {
            base.Info = PrefabInfo.WithTechType("DrillableDenimium", "Denimium Deposit", "A large deposit of denimium crystals. These dark purple gems can be extracted using a drill for deep-sea construction applications.");
        }

        protected override void Configure(ICustomPrefab customPrefab)
        {
            // Set the TechType in the handler
            PrecursorHandler.DrillableDenimiumTechType = base.Info.TechType;

            base.SetGameObject(customPrefab, new CloneTemplate(base.Info, "4f441e53-7a9a-44dc-83a4-b1791dc88ffd")
            {
                ModifyPrefab = new Action<GameObject>(this.ApplyChangesToPrefab)
            });

            customPrefab.SetSpawns(new WorldEntityInfo
            {
                cellLevel = LargeWorldEntity.CellLevel.VeryFar,
                classId = base.Info.ClassID,
                localScale = Vector3.one,
                slotType = EntitySlot.Type.Medium,
                techType = base.Info.TechType
            }, this.BiomesToSpawnIn);
        }

        private LootDistributionData.BiomeData[] BiomesToSpawnIn
        {
            get
            {
                return new LootDistributionData.BiomeData[]
                {
                    // Deep sea spawns - perfect for denimium's extreme depth theme
                    new LootDistributionData.BiomeData
                    {
                        biome = BiomeType.Dunes_CaveWall,
                        count = 1,
                        probability = 0.4f
                    },
                    new LootDistributionData.BiomeData
                    {
                        biome = BiomeType.Dunes_CaveFloor,
                        count = 1,
                        probability = 0.4f
                    },
                    new LootDistributionData.BiomeData
                    {
                        biome = BiomeType.Dunes_CaveCeiling,
                        count = 1,
                        probability = 0.3f
                    },
                    new LootDistributionData.BiomeData
                    {
                        biome = BiomeType.DeepGrandReef_Ground,
                        count = 1,
                        probability = 0.25f
                    },
                    new LootDistributionData.BiomeData
                    {
                        biome = BiomeType.LostRiver_BonesField_Lake,
                        count = 1,
                        probability = 0.2f
                    }
                };
            }
        }

        private void ApplyChangesToPrefab(GameObject prefab)
        {
            Plugin.Logger.LogInfo($"[DenimiumDrillable] Starting prefab modification for: {prefab.name}");
            Plugin.Logger.LogInfo($"[DenimiumDrillable] Prefab has {prefab.transform.childCount} children");

            // Disable light if it exists
            Light light = prefab.GetComponentInChildren<Light>();
            if (light != null)
            {
                light.enabled = false;
                Plugin.Logger.LogInfo("[DenimiumDrillable] Disabled light component");
            }

            // Add resource tracker
            PrefabUtils.AddResourceTracker(prefab, PrecursorHandler.DenimiumTechType);
            Plugin.Logger.LogInfo($"[DenimiumDrillable] Added resource tracker for: {PrecursorHandler.DenimiumTechType}");

            // Configure drillable component
            Drillable component = prefab.GetComponent<Drillable>();
            if (component != null)
            {
                Plugin.Logger.LogInfo($"[DenimiumDrillable] Found Drillable component with {component.resources.Length} resources");
                component.resources = new Drillable.ResourceType[]
                {
                    new Drillable.ResourceType
                    {
                        chance = 1f,
                        techType = PrecursorHandler.DenimiumTechType
                    }
                };
                component.maxResourcesToSpawn = 4; // Slightly less than emerald due to rarity
                component.health = new float[] { 40f }; // Harder to drill than emerald
                Plugin.Logger.LogInfo("[DenimiumDrillable] Configured drillable component");
            }
            else
            {
                Plugin.Logger.LogError("[DenimiumDrillable] No Drillable component found!");
            }

            // Apply denimium colors to all renderers
            Renderer[] componentsInChildren = prefab.GetComponentsInChildren<Renderer>();
            Plugin.Logger.LogInfo($"[DenimiumDrillable] Found {componentsInChildren.Length} renderers");

            if (componentsInChildren.Length == 0)
            {
                Plugin.Logger.LogError("[DenimiumDrillable] No renderers found! This is why it's invisible!");
                // Try to find renderers in a different way
                Component[] allComponents = prefab.GetComponentsInChildren<Component>();
                Plugin.Logger.LogInfo($"[DenimiumDrillable] All components found: {allComponents.Length}");
                foreach (Component comp in allComponents)
                {
                    Plugin.Logger.LogInfo($"[DenimiumDrillable] Component: {comp.GetType().Name} on {comp.name}");
                }
            }

            foreach (Renderer renderer in componentsInChildren)
            {
                Plugin.Logger.LogInfo($"[DenimiumDrillable] Processing renderer: {renderer.name}");

                // Apply the same denimium colors as the regular crystal
                renderer.material.SetColor("_Color", new Color(0.25f, 0.15f, 0.35f, 0.9f)); // Dark purple with slight transparency
                renderer.material.SetColor("_SpecColor", new Color(0.8f, 0.6f, 0.9f)); // Purple-white specular
                renderer.material.SetColor("_GlowColor", new Color(0.4f, 0.2f, 0.6f, 1f)); // Purple glow

                // Crystal properties for that perfect denimium look
                renderer.material.SetFloat("_Fresnel", 0.65f);
                renderer.material.SetFloat("_SpecInt", 1.2f);
                renderer.material.SetFloat("_Shininess", 15f);
                renderer.material.SetFloat("_GlowStrength", 0.4f);
                renderer.material.SetFloat("_GlowStrengthNight", 0.6f);

                // Make sure renderer is enabled
                renderer.enabled = true;

                // Apply translucency
                this.ApplyTranslucency(renderer);
                Plugin.Logger.LogInfo($"[DenimiumDrillable] Applied colors and effects to renderer: {renderer.name}");
            }

            // Scale up for drillable deposits - make it impressive!
            if (prefab.transform.childCount > 0)
            {
                prefab.transform.GetChild(0).localScale = new Vector3(1.8f, 1.8f, 1.8f);
                Plugin.Logger.LogInfo("[DenimiumDrillable] Scaled up first child");
            }
            else
            {
                Plugin.Logger.LogWarning("[DenimiumDrillable] No children to scale!");
                // Try scaling the prefab itself
                prefab.transform.localScale = new Vector3(1.8f, 1.8f, 1.8f);
                Plugin.Logger.LogInfo("[DenimiumDrillable] Scaled the prefab itself instead");
            }

            // Glass surface type for proper crystal interaction sounds
            prefab.EnsureComponent<VFXSurface>().surfaceType = VFXSurfaceTypes.glass;

            // Ensure the prefab is active
            prefab.SetActive(true);

            Plugin.Logger.LogInfo("[DenimiumDrillable] Prefab modification completed with beautiful dark purple crystal effects!");
        }

        private void ApplyTranslucency(Renderer renderer)
        {
            renderer.material.EnableKeyword("_ZWRITE_ON");
            renderer.material.EnableKeyword("WBOIT");
            renderer.material.SetInt("_ZWrite", 0);
            renderer.material.SetInt("_Cutoff", 0);
            renderer.material.SetFloat("_SrcBlend", 1f);
            renderer.material.SetFloat("_DstBlend", 1f);
            renderer.material.SetFloat("_SrcBlend2", 0f);
            renderer.material.SetFloat("_DstBlend2", 10f);
            renderer.material.SetFloat("_AddSrcBlend", 1f);
            renderer.material.SetFloat("_AddDstBlend", 1f);
            renderer.material.SetFloat("_AddSrcBlend2", 0f);
            renderer.material.SetFloat("_AddDstBlend2", 10f);
            renderer.material.globalIlluminationFlags = (MaterialGlobalIlluminationFlags.RealtimeEmissive | MaterialGlobalIlluminationFlags.EmissiveIsBlack);
            renderer.material.renderQueue = 3101;
            renderer.material.enableInstancing = true;
        }
    }
}
