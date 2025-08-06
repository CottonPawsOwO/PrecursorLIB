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
    public static class EmeraldItem
    {
        public static PrefabInfo Info { get; } = PrefabInfo.WithTechType("Emerald", "Emerald", "A beautiful green crystalline mineral. Its structure seems unusually perfect...")
            .WithIcon(ImageUtils.LoadSpriteFromFile("F:\\STEAM\\steamapps\\common\\Subnautica\\BepInEx\\plugins\\PrecursorLIB\\Assets\\Minerals\\Emerald.png"));

        public static void Register()
        {
            var prefab = new CustomPrefab(Info);

            var cloneTemplate = new CloneTemplate(Info, TechType.Kyanite)
            {
                ModifyPrefab = ModifyEmeraldPrefab
            };

            prefab.SetGameObject(cloneTemplate);

            CraftData.pickupSoundList.Add(Info.TechType, "event:/loot/pickup_precursorioncrystal");

            prefab.Register();

            Plugin.Logger.LogInfo("Emerald item registered successfully.");
        }

        private static void ModifyEmeraldPrefab(GameObject prefab)
        {
            PrefabUtils.AddResourceTracker(prefab, Info.TechType);

            Renderer componentInChildren = prefab.GetComponentInChildren<Renderer>();
            if (componentInChildren != null)
            {
                componentInChildren.material.SetColor("_Color", new Color(0.2f, 0.9f, 0.8f, 0.9f));

                componentInChildren.material.SetColor("_SpecColor", new Color(0.5f, 1.8f, 1f));

                componentInChildren.material.SetColor("_GlowColor", new Color(0.3f, 1.1f, 0.7f, 1f));

                componentInChildren.material.SetFloat("_Fresnel", 0.69f);
                componentInChildren.material.SetFloat("_SpecInt", 1f);
                componentInChildren.material.SetFloat("_Shininess", 12f);
                componentInChildren.material.SetFloat("_GlowStrength", 0.8f);
                componentInChildren.material.SetFloat("_GlowStrengthNight", 0.8f); 


                componentInChildren.transform.localScale = Vector3.one * 0.66f;


                ApplyTranslucency(componentInChildren);
            }


            prefab.EnsureComponent<VFXSurface>().surfaceType = VFXSurfaceTypes.glass;


            var techTag = prefab.GetComponent<TechTag>() ?? prefab.AddComponent<TechTag>();
            techTag.type = Info.TechType;

        }

        private static void ApplyTranslucency(Renderer renderer)
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