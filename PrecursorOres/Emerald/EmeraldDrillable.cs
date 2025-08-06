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
    internal class EmeraldDrillable : BindablePrefab
    {
        public EmeraldDrillable()
        {
            base.Info = PrefabInfo.WithTechType("DrillableEmerald", "Emerald Deposit", "A large deposit of emerald crystals. This precious green gem can be extracted using a drill.");
        }

        protected override void Configure(ICustomPrefab customPrefab)
        {
            PrecursorHandler.DrillableEmeraldTechType = base.Info.TechType;

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
                    new LootDistributionData.BiomeData
                    {
                        biome = BiomeType.KooshZone_RockWall,
                        count = 1,
                        probability = 0.06f
                    },
                    new LootDistributionData.BiomeData
                    {
                        biome = BiomeType.KooshZone_KooshReefs,
                        count = 1,
                        probability = 0.06f
                    }
                };
            }
        }

        private void ApplyChangesToPrefab(GameObject prefab)
        {
            Plugin.Logger.LogInfo($"[EmeraldDrillable] Starting prefab modification for: {prefab.name}");
            Plugin.Logger.LogInfo($"[EmeraldDrillable] Prefab has {prefab.transform.childCount} children");

            Light light = prefab.GetComponentInChildren<Light>();
            if (light != null)
            {
                light.enabled = false;
                Plugin.Logger.LogInfo("[EmeraldDrillable] Disabled light component");
            }

            PrefabUtils.AddResourceTracker(prefab, PrecursorHandler.EmeraldTechType);
            Plugin.Logger.LogInfo($"[EmeraldDrillable] Added resource tracker for: {PrecursorHandler.EmeraldTechType}");

            Drillable component = prefab.GetComponent<Drillable>();
            if (component != null)
            {
                Plugin.Logger.LogInfo($"[EmeraldDrillable] Found Drillable component with {component.resources.Length} resources");
                component.resources = new Drillable.ResourceType[]
                {
                    new Drillable.ResourceType
                    {
                        chance = 1f,
                        techType = PrecursorHandler.EmeraldTechType
                    }
                };
                component.maxResourcesToSpawn = 5;
                component.health = new float[] { 30f };
                Plugin.Logger.LogInfo("[EmeraldDrillable] Configured drillable component");
            }
            else
            {
                Plugin.Logger.LogError("[EmeraldDrillable] No Drillable component found!");
            }


            Renderer[] componentsInChildren = prefab.GetComponentsInChildren<Renderer>();
            Plugin.Logger.LogInfo($"[EmeraldDrillable] Found {componentsInChildren.Length} renderers");

            if (componentsInChildren.Length == 0)
            {
                Plugin.Logger.LogError("[EmeraldDrillable] No renderers found! This is why it's invisible!");
                Component[] allComponents = prefab.GetComponentsInChildren<Component>();
                Plugin.Logger.LogInfo($"[EmeraldDrillable] All components found: {allComponents.Length}");
                foreach (Component comp in allComponents)
                {
                    Plugin.Logger.LogInfo($"[EmeraldDrillable] Component: {comp.GetType().Name} on {comp.name}");
                }
            }

            foreach (Renderer renderer in componentsInChildren)
            {
                Plugin.Logger.LogInfo($"[EmeraldDrillable] Processing renderer: {renderer.name}");

                renderer.material.SetColor("_Color", new Color(0.2f, 0.9f, 0.8f, 0.9f));
                renderer.material.SetColor("_SpecColor", new Color(0.5f, 1.8f, 1f));
                renderer.material.SetColor("_GlowColor", new Color(0.3f, 1.1f, 0.7f, 1f));
                renderer.material.SetFloat("_Fresnel", 0.69f);
                renderer.material.SetFloat("_SpecInt", 1f);
                renderer.material.SetFloat("_Shininess", 12f);
                renderer.material.SetFloat("_GlowStrength", 0.8f);
                renderer.material.SetFloat("_GlowStrengthNight", 0.8f);
                renderer.enabled = true;
                this.ApplyTranslucency(renderer);
                Plugin.Logger.LogInfo($"[EmeraldDrillable] Applied colors and effects to renderer: {renderer.name}");
            }

            if (prefab.transform.childCount > 0)
            {
                prefab.transform.GetChild(0).localScale = new Vector3(1.5f, 1.5f, 1.5f);
                Plugin.Logger.LogInfo("[EmeraldDrillable] Scaled up first child");
            }
            else
            {
                Plugin.Logger.LogWarning("[EmeraldDrillable] No children to scale!");
                prefab.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
                Plugin.Logger.LogInfo("[EmeraldDrillable] Scaled the prefab itself instead");
            }

            prefab.EnsureComponent<VFXSurface>().surfaceType = VFXSurfaceTypes.glass;
            prefab.SetActive(true);

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