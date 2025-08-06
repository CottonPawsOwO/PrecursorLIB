using ArchiTech;
using BepInEx;
using BepInEx.Logging;
using Items.PrecursorMaterials;
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

            EmeraldItem.Register();
            PrecursorHandler.EmeraldTechType = EmeraldItem.Info.TechType;

            IonicShardsItem.Register();
            PrecursorHandler.IonicShardsTechType = IonicShardsItem.Info.TechType;

            var emeraldDrillable = new EmeraldDrillable();
            ((IBindablePrefab)emeraldDrillable).Register();

            DenimiumCrystalItem.Register();
            PrecursorHandler.DenimiumTechType = DenimiumCrystalItem.Info.TechType;

            var denimiumDrillable = new DenimiumDrillable();
            ((IBindablePrefab)denimiumDrillable).Register();

            AesberiumItem.Register();
            PrecursorHandler.AesberiumTechType = AesberiumItem.Info.TechType;

            var aesberiumDrillable = new AesberiumDrillable();
            ((IBindablePrefab)aesberiumDrillable).Register();

            Logger.LogInfo("Precursor materials registered successfully.");

            EmeraldPDA.Register(PrecursorHandler.EmeraldTechType);
            Logger.LogInfo("Emerald PDA registered successfully.");

            DenimiumCrystalPDA.Register(PrecursorHandler.DenimiumTechType);
            Logger.LogInfo("Denimium PDA registered successfully.");

            AesberiumPDA.Register(PrecursorHandler.DrillableAesberiumTechType);
            Logger.LogInfo("Aesberium PDA registered successfully.");

            IonicShardsPDA.Register(PrecursorHandler.IonicShardsTechType);
            Logger.LogInfo("Ionic shards PDA registered successfully.");
        }

        private static void SetupCrafting()
        {
            Logger.LogInfo("Setting up crafting system...");
            Logger.LogInfo("Crafting system setup completed.");
        }

        private static void RegisterConsoleCommands()
        {
            ConsoleCommandsHandler.RegisterConsoleCommand("givepitem", (Action<string>)GiveItem);
            ConsoleCommandsHandler.RegisterConsoleCommand("giveallprecursoritems", (Action)GiveAllPrecursorItems);
            Logger.LogInfo("Registered console commands 'givepitem' and 'giveallprecursoritems'.");
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
                case "drillableaesberium":
                case "aesberiumdeposit":
                    techTypeToGive = PrecursorHandler.DrillableAesberiumTechType;
                    break;
                case "aesberium":
                    techTypeToGive = PrecursorHandler.AesberiumTechType;
                    break;
                case "Ionremanant":
                    techTypeToGive = PrecursorHandler.IonicShardsTechType;
                    break;
                default:
                    ErrorMessage.AddMessage($"Unknown item '{itemName}'.");
                    return;
            }

            if (techTypeToGive != TechType.None)
            {
                CraftData.AddToInventory(techTypeToGive, 1);
                ErrorMessage.AddMessage($"Added {techTypeToGive} to inventory.");
            }
        }

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
                CraftData.AddToInventory(PrecursorHandler.AesberiumTechType, 1);
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

    /// Handler for managing PrecursorLibrary TechTypes and utilities
    public static class PrecursorHandler
    {
        public static TechType EmeraldTechType { get; internal set; }

        public static TechType DrillableEmeraldTechType { get; internal set; }
        public static TechType DenimiumTechType { get; internal set; }

        public static TechType DrillableDenimiumTechType { get; internal set; }

        public static TechType AesberiumTechType { get; internal set; }

        public static TechType DrillableAesberiumTechType { get; internal set; }

        public static TechType IonicShardsTechType { get; internal set; }

        internal static string AssetsFolder => Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets");

        /// Makes the object given scannable from the Scanner Room.
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
        
        /// Loads a UnityEngine.Sprite from the assets folder with the given filename
        /// <param name="filename">The filename of the sprite</param>
        /// <returns>The loaded UnityEngine.Sprite</returns>
        public static Sprite LoadSprite(string filename)
        {
            string fullPath = Path.Combine(AssetsFolder, "Minerals", filename);
            Atlas.Sprite atlasSprite = ImageUtils.LoadSpriteFromFile(fullPath);
            Texture2D texture = atlasSprite.texture;
            return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        }

        /// Loads an Atlas.Sprite from the assets folder with the given filename.
        /// <param name="filename">The filename of the sprite</param>
        /// <returns>The loaded Atlas.Sprite</returns>
        public static Atlas.Sprite LoadAtlasSprite(string filename)
        {
            string fullPath = Path.Combine(AssetsFolder, "Minerals", filename);
            return ImageUtils.LoadSpriteFromFile(fullPath);
        }

        /// Loads a texture from the assets folder with the given filename.
        /// <param name="filename">The filename of the texture</param>
        /// <returns>The loaded texture</returns>
        public static Texture2D LoadTexture(string filename)
        {
            string fullPath = Path.Combine(AssetsFolder, "Minerals", filename);
            return ImageUtils.LoadTextureFromFile(fullPath);
        }

        /// Applies the beautiful translucency effects to a renderer for crystal-like materials.
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
