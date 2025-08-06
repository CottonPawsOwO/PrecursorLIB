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
    internal class AesberiumDrillable : BindablePrefab
    {
        public AesberiumDrillable()
        {
            base.Info = PrefabInfo.WithTechType("DrillableAesberium", "Aesberium Deposit", "A large deposit of aesberium ore found in mountain regions, near the QEP. This rare metallic mineral can be extracted using a drill for ultimate advanced construction applications.");
        }

        protected override void Configure(ICustomPrefab customPrefab)
        {
            PrecursorHandler.DrillableAesberiumTechType = base.Info.TechType;

            base.SetGameObject(customPrefab, new CloneTemplate(base.Info, TechType.DrillableLithium)
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
                    new LootDistributionData.BiomeData
                    {
                        biome = BiomeType.Mountains_CaveFloor,
                        count = 1,
                        probability = 0.02f
                    },
                    new LootDistributionData.BiomeData
                    {
                        biome = BiomeType.Mountains_CaveCeiling,
                        count = 1,
                        probability = 0.02f
                    },
                    new LootDistributionData.BiomeData
                    {
                        biome = BiomeType.Mountains_Sand,
                        count = 1,
                        probability = 0.01f
                    },
                    new LootDistributionData.BiomeData
                    {
                        biome = BiomeType.Mountains_Rock,
                        count = 1,
                        probability = 0.01f
                    }
                };
            }
        }

        private void ApplyChangesToPrefab(GameObject prefab)
        {
            Plugin.Logger.LogInfo($"[AesberiumDrillable] Starting prefab modification for: {prefab.name}");
            Plugin.Logger.LogInfo($"[AesberiumDrillable] Prefab has {prefab.transform.childCount} children");

            Light light = prefab.GetComponentInChildren<Light>();
            if (light != null)
            {
                light.enabled = false;
                Plugin.Logger.LogInfo("[AesberiumDrillable] Disabled light component");
            }

            PrefabUtils.AddResourceTracker(prefab, PrecursorHandler.AesberiumTechType);
            Plugin.Logger.LogInfo($"[AesberiumDrillable] Added resource tracker for: {PrecursorHandler.AesberiumTechType}");

            Drillable component = prefab.GetComponent<Drillable>();
            if (component != null)
            {
                Plugin.Logger.LogInfo($"[AesberiumDrillable] Found Drillable component with {component.resources.Length} resources");
                component.resources = new Drillable.ResourceType[]
                {
                    new Drillable.ResourceType
                    {
                        chance = 1f,
                        techType = PrecursorHandler.AesberiumTechType
                    }
                };
                component.maxResourcesToSpawn = 3;
                component.health = new float[] { 60f };
                Plugin.Logger.LogInfo("[AesberiumDrillable] Configured drillable component");
            }
            else
            {
                Plugin.Logger.LogError("[AesberiumDrillable] No Drillable component found!");
            }

            Renderer[] componentsInChildren = prefab.GetComponentsInChildren<Renderer>();
            Plugin.Logger.LogInfo($"[AesberiumDrillable] Found {componentsInChildren.Length} renderers");

            if (componentsInChildren.Length == 0)
            {
                Plugin.Logger.LogError("[AesberiumDrillable] No renderers found! This is why it's invisible!");
                Component[] allComponents = prefab.GetComponentsInChildren<Component>();
                Plugin.Logger.LogInfo($"[AesberiumDrillable] All components found: {allComponents.Length}");
                foreach (Component comp in allComponents)
                {
                    Plugin.Logger.LogInfo($"[AesberiumDrillable] Component: {comp.GetType().Name} on {comp.name}");
                }
            }

            foreach (Renderer renderer in componentsInChildren)
            {
                Plugin.Logger.LogInfo($"[AesberiumDrillable] Processing renderer: {renderer.name}");

                renderer.material.SetColor("_Color", new Color(0.4f, 0.5f, 0.6f, 0.92f));
                renderer.material.SetColor("_SpecColor", new Color(0.7f, 0.8f, 0.9f, 1.0f));
                renderer.material.SetColor("_GlowColor", new Color(0.3f, 0.4f, 0.5f, 1f));
                renderer.material.SetFloat("_Fresnel", 0.6f);
                renderer.material.SetFloat("_SpecInt", 1.4f);
                renderer.material.SetFloat("_Shininess", 20f);
                renderer.material.SetFloat("_GlowStrength", 0.4f);
                renderer.material.SetFloat("_GlowStrengthNight", 0.6f);
                renderer.enabled = true;
                Plugin.Logger.LogInfo($"[AesberiumDrillable] Applied colors and effects to renderer: {renderer.name}");
            }

            if (prefab.transform.childCount > 0)
            {
                prefab.transform.GetChild(0).localScale = new Vector3(1.6f, 1.6f, 1.6f);
                Plugin.Logger.LogInfo("[AesberiumDrillable] Scaled up first child");
            }
            else
            {
                Plugin.Logger.LogWarning("[AesberiumDrillable] No children to scale!");
                prefab.transform.localScale = new Vector3(1.6f, 1.6f, 1.6f);
                Plugin.Logger.LogInfo("[AesberiumDrillable] Scaled the prefab itself instead");
            }

            prefab.EnsureComponent<VFXSurface>().surfaceType = VFXSurfaceTypes.metal;
            prefab.SetActive(true);
        }

    }
}