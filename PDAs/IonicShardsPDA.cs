using Nautilus.Handlers;
using PrecursorLibrary;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace ArchiTech
{
    public static class IonicShardsPDA
    {
        private static Texture2D LoadTextureFromFile(string fileName)
        {
            try
            {
                string pluginPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                string assetsPath = Path.Combine(pluginPath, "Assets", "PDA");
                string filePath = Path.Combine(assetsPath, fileName);

                Plugin.Logger.LogInfo($"[IonicShardsPDA] Looking for texture in subfolder: {filePath}");

                if (!File.Exists(filePath))
                {
                    Plugin.Logger.LogWarning($"[IonicShardsPDA] Texture file not found in Assets/PDA: {filePath}");
                    return null;
                }

                byte[] fileData = File.ReadAllBytes(filePath);

                Texture2D tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
                tex.filterMode = FilterMode.Bilinear;
                tex.wrapMode = TextureWrapMode.Clamp;

                if (tex.LoadImage(fileData))
                {
                    tex.Apply();
                    Plugin.Logger.LogInfo($"[IonicShardsPDA] Successfully loaded texture: {fileName} ({tex.width}x{tex.height})");
                    return tex;
                }

                UnityEngine.Object.DestroyImmediate(tex);
                return null;
            }
            catch (System.Exception ex)
            {
                Plugin.Logger.LogError($"[IonicShardsPDA] Exception loading texture: {ex}");
                return null;
            }
        }

        private static Sprite CreateSpriteFromTexture(Texture2D tex)
        {
            if (tex == null) return null;

            Sprite sprite = Sprite.Create(
                tex,
                new Rect(0, 0, tex.width, tex.height),
                new Vector2(0.5f, 0.5f),
                100.0f
            );

            return sprite;
        }

        public static void Register(TechType ionicShardsTechType)
        {
            Plugin.Logger.LogInfo("[IonicShardsPDA] Registering Ionic Shards PDA entry...");

            Texture2D ionicShardsTexture = LoadTextureFromFile("IonicShardsEntry.png");
            Sprite ionicShardsPopup = CreateSpriteFromTexture(ionicShardsTexture);

            if (ionicShardsTexture == null)
            {
                Plugin.Logger.LogWarning("[IonPDA] Custom texture failed to load, entry will have no image");
            }

            PDAHandler.AddEncyclopediaEntry(
                key: "IonicShards",
                path: "PlanetaryGeology",
                title: "Ionic Shards",
                desc: "Incredible ionized energy found within these luminescent crystalline fragments. These bright green glowing shards emit intense ionic radiation and display energy patterns consistent with advanced technology.\n\n" +
                      "ENERGY COMPOSITION:\n" +
                      "CAUTION: High levels of ionized energy detected. The shards contain concentrated ionic charges that generate continuous luminescence. Energy readings indicate sustained power output without apparent energy source, suggesting advanced energy manipulation principles.\\n\\n" +
                      "STRUCTURAL ANALYSIS:\n" +
                      "• Energy output: Continuous ionic emission with no decay\n" +
                      "• Luminescence: Intense green glow with 24/7 visibility\n" +
                      "• Radiation levels: Elevated but within safe handling parameters\n" +
                      "• Crystal matrix: Self-sustaining energy lattice structure\n\n" +
                      "THEORETICAL CONNECTIONS:\n" +
                      "Analysis reveals energy signatures theoretically related to precursor technology. The ionic patterns match theoretical models of advanced energy manipulation systems. Material demonstrates principles beyond current scientific understanding.\n\n" +
                      "ALTERRA ASSESSMENT:\n" +
                      "RESEARCH PRIORITY: MAXIMUM\n" +
                      "These shards represent a breakthrough in energy research. The sustained ionic emission without external power source indicates revolutionary energy storage or generation technology.\n\n" +
                      "CAUTION: Extended exposure monitoring recommended.\n" +
                      "Further study recommended to understand the underlying energy mechanisms and potential applications in advanced technology systems.\n\n" +
                      "// ENERGY ANOMALY LOGGED - PRECURSOR TECHNOLOGY RESEARCH PRIORITY //",
                image: ionicShardsTexture,
                popupImage: ionicShardsPopup,
                unlockSound: PDAHandler.UnlockBasic
            );

            PDAHandler.AddCustomScannerEntry(
                new PDAScanner.EntryData()
                {
                    key = ionicShardsTechType,
                    blueprint = TechType.None,
                    destroyAfterScan = false,
                    encyclopedia = "IonicShards",
                    isFragment = false,
                    locked = false,
                    scanTime = 8.0f,
                    totalFragments = 1
                }
            );

            Plugin.Logger.LogInfo("[IonicShardsPDA] Ionic Shards PDA entry registered successfully.");
        }
    }
}