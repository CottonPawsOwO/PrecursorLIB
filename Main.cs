using Items.PrecursorMaterials;
using BepInEx;
using BepInEx.Logging;
using Nautilus.Handlers;
using Nautilus.Utility;
using System;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace PrecursorLibrary
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInDependency("com.snmodding.nautilus")]
    public class Plugin : BaseUnityPlugin
    {
        public new static ManualLogSource Logger { get; private set; }

        private void Awake()
        {
            Logger = base.Logger;
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");

            try
            {
                RegisterItems();
                SetupCrafting();
                RegisterConsoleCommands();
                Logger.LogInfo("PrecursorLibrary mod loaded successfully!");
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error loading the mod: {ex}");
            }
        }

        private static void RegisterItems()
        {
            Logger.LogInfo("Registering precursor materials...");

            // Register Emerald
            EmeraldItem.Register();
            PrecursorHandler.EmeraldTechType = EmeraldItem.Info.TechType;

            // Register Drillable Emerald using BindablePrefab
            var emeraldDrillable = new EmeraldDrillable();
            ((IBindablePrefab)emeraldDrillable).Register();

            // Register Denimium Crystal
            DenimiumCrystalItem.Register();
            PrecursorHandler.DenimiumTechType = DenimiumCrystalItem.Info.TechType;

            // Register Drillable Denimium using BindablePrefab
            var denimiumDrillable = new DenimiumDrillable();
            ((IBindablePrefab)denimiumDrillable).Register();

            Logger.LogInfo("Precursor materials registered successfully.");
        }

        private static void SetupCrafting()
        {
            Logger.LogInfo("Setting up crafting system...");
            // TODO: Implement crafting system when needed
            Logger.LogInfo("Crafting system setup completed.");
        }

        private static void RegisterConsoleCommands()
        {
            // Explicitly cast the methods to the correct delegate type (Action)
            // This resolves the compiler ambiguity.
            ConsoleCommandsHandler.RegisterConsoleCommand("giveitem", (Action<string>)GiveItem);
            ConsoleCommandsHandler.RegisterConsoleCommand("giveallprecursoritems", (Action)GiveAllPrecursorItems);
            Logger.LogInfo("Registered console commands 'giveitem' and 'giveallarchitechitems'.");
        }

        // Usage in console: giveitem emerald
        private static void GiveItem(string itemName)
        {
            if (string.IsNullOrEmpty(itemName))
            {
                ErrorMessage.AddMessage("Usage: giveitem <itemName>");
                ErrorMessage.AddMessage("Available items: emerald, drillableemerald, denimium, drillabledenimium");
                return;
            }

            TechType techTypeToGive = TechType.None;

            switch (itemName.ToLower())
            {
                case "emerald":
                    techTypeToGive = PrecursorHandler.EmeraldTechType;
                    break;
                case "drillableemerald":
                case "emeralddeposit":
                    techTypeToGive = PrecursorHandler.DrillableEmeraldTechType;
                    break;
                case "denimium":
                case "denimiumcrystal":
                    techTypeToGive = PrecursorHandler.DenimiumTechType;
                    break;
                case "drillabledenimium":
                case "denimiumdeposit":
                    techTypeToGive = PrecursorHandler.DrillableDenimiumTechType;
                    break;
                default:
                    ErrorMessage.AddMessage($"Unknown item '{itemName}'.");
                    ErrorMessage.AddMessage("Available items: emerald, drillableemerald, denimium, drillabledenimium");
                    return;
            }

            if (techTypeToGive != TechType.None)
            {
                CraftData.AddToInventory(techTypeToGive, 1);
                ErrorMessage.AddMessage($"Added {techTypeToGive} to inventory.");
            }
        }

        // Usage in console: giveallprecursor
        private static void GiveAllPrecursorItems()
        {
            CraftData.AddToInventory(PrecursorHandler.EmeraldTechType, 1);
            if (PrecursorHandler.DrillableEmeraldTechType != TechType.None)
            {
                CraftData.AddToInventory(PrecursorHandler.DrillableEmeraldTechType, 1);
            }
            CraftData.AddToInventory(PrecursorHandler.DenimiumTechType, 1);
            if (PrecursorHandler.DrillableDenimiumTechType != TechType.None)
            {
                CraftData.AddToInventory(PrecursorHandler.DrillableDenimiumTechType, 1);
            }
            ErrorMessage.AddMessage("Added all precursor materials to inventory.");
        }
    }

    public static class PluginInfo
    {
        public const string PLUGIN_GUID = "com.precursorlibrary.mod";
        public const string PLUGIN_NAME = "PrecursorLibrary";
        public const string PLUGIN_VERSION = "1.0.0";
    }

    /// <summary>
    /// Handler for managing PrecursorLibrary TechTypes and utilities
    /// </summary>
    public static class PrecursorHandler
    {
        /// <summary>
        /// Gets the Emerald TechType so you can reference it in your mod.
        /// </summary>
        public static TechType EmeraldTechType { get; internal set; }

        /// <summary>
        /// Gets the Drillable Emerald TechType so you can reference it in your mod.
        /// </summary>
        public static TechType DrillableEmeraldTechType { get; internal set; }

        /// <summary>
        /// Gets the Denimium TechType so you can reference it in your mod.
        /// </summary>
        public static TechType DenimiumTechType { get; internal set; }

        /// <summary>
        /// Gets the Drillable Denimium TechType so you can reference it in your mod.
        /// </summary>
        public static TechType DrillableDenimiumTechType { get; internal set; }

        /// <summary>
        /// Gets the Aesberium TechType so you can reference it in your mod.
        /// </summary>
        public static TechType AesberiumTechType { get; internal set; }

        /// <summary>
        /// Gets the Drillable Aesberium TechType so you can reference it in your mod.
        /// </summary>
        public static TechType DrillableAesberiumTechType { get; internal set; }

        /// <summary>
        /// Gets the Ionized Capsules TechType so you can reference it in your mod.
        /// </summary>
        public static TechType IonizedCapsulesTechType { get; internal set; }

        /// <summary>
        /// Gets the assets folder path for loading sprites and textures.
        /// </summary>
        internal static string AssetsFolder => Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets");

        /// <summary>
        /// Makes the object given scannable from the Scanner Room.
        /// </summary>
        /// <param name="gameObject">The GameObject to make scannable</param>
        /// <param name="categoryTechType">Category in the Scanner Room</param>
        public static void SetObjectScannable(GameObject gameObject, TechType categoryTechType = TechType.GenericEgg)
        {
            if (CraftData.GetTechType(gameObject) == TechType.None)
            {
                Plugin.Logger.LogError("TechType to get from SetObjectScannable() is null");
                return;
            }
            PrefabUtils.AddResourceTracker(gameObject, categoryTechType);
        }

        /// <summary>
        /// Loads a UnityEngine.Sprite from the assets folder with the given filename.
        /// </summary>
        /// <param name="filename">The filename of the sprite</param>
        /// <returns>The loaded UnityEngine.Sprite</returns>
        public static Sprite LoadSprite(string filename)
        {
            string fullPath = Path.Combine(AssetsFolder, "Minerals", filename);
            Atlas.Sprite atlasSprite = ImageUtils.LoadSpriteFromFile(fullPath);
            Texture2D texture = atlasSprite.texture;
            return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        }

        /// <summary>
        /// Loads an Atlas.Sprite from the assets folder with the given filename.
        /// </summary>
        /// <param name="filename">The filename of the sprite</param>
        /// <returns>The loaded Atlas.Sprite</returns>
        public static Atlas.Sprite LoadAtlasSprite(string filename)
        {
            string fullPath = Path.Combine(AssetsFolder, "Minerals", filename);
            return ImageUtils.LoadSpriteFromFile(fullPath);
        }

        /// <summary>
        /// Loads a texture from the assets folder with the given filename.
        /// </summary>
        /// <param name="filename">The filename of the texture</param>
        /// <returns>The loaded texture</returns>
        public static Texture2D LoadTexture(string filename)
        {
            string fullPath = Path.Combine(AssetsFolder, "Minerals", filename);
            return ImageUtils.LoadTextureFromFile(fullPath);
        }

        /// <summary>
        /// Applies the beautiful translucency effects to a renderer for crystal-like materials.
        /// </summary>
        /// <param name="renderer">The renderer to apply translucency to</param>
        public static void ApplyTranslucency(Renderer renderer)
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
