using Nautilus.Handlers;
using PrecursorLibrary;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace ArchiTech
{
    public static class DenimiumCrystalPDA
    {
        private static Texture2D LoadTextureFromFile(string fileName)
        {
            try
            {
                string pluginPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                string assetsPath = Path.Combine(pluginPath, "Assets", "PDA");
                string filePath = Path.Combine(assetsPath, fileName);

                Plugin.Logger.LogInfo($"[DenimiumCrystalPDA] Looking for texture in subfolder: {filePath}");

                if (!File.Exists(filePath))
                {
                    Plugin.Logger.LogWarning($"[DenimiumCrystalPDA] Texture file not found in Assets/PDA: {filePath}");
                    return null;
                }

                byte[] fileData = File.ReadAllBytes(filePath);

                Texture2D tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
                tex.filterMode = FilterMode.Bilinear;
                tex.wrapMode = TextureWrapMode.Clamp;

                if (tex.LoadImage(fileData))
                {
                    tex.Apply();
                    Plugin.Logger.LogInfo($"[DenimiumCrystalPDA] Successfully loaded texture: {fileName} ({tex.width}x{tex.height})");
                    return tex;
                }

                UnityEngine.Object.DestroyImmediate(tex);
                return null;
            }
            catch (System.Exception ex)
            {
                Plugin.Logger.LogError($"[DenimiumCrystalPDA] Exception loading texture: {ex}");
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

        public static void Register(TechType denimiumTechType)
        {
            Plugin.Logger.LogInfo("[DenimiumCrystalPDA] Registering Denimium Crystal PDA entry...");

            Texture2D denimiumTexture = LoadTextureFromFile("DeniEntry.png");
            Sprite denimiumPopup = CreateSpriteFromTexture(denimiumTexture);

            if (denimiumTexture == null)
            {
                Plugin.Logger.LogWarning("[DenimiumCrystalPDA] Custom texture failed to load, entry will have no image");
            }

            PDAHandler.AddEncyclopediaEntry(
                key: "DenimiumCrystal",
                path: "PlanetaryGeology",
                title: "Denimium Crystal",
                desc: "A rare crystalline mineral comprised of dark semi-purplish formations, discovered in the deepest oceanic trenches. Scientific analysis reveals a unique molecular lattice structure with extraordinary pressure deformation resistance.\n\n" +
                      "MOLECULAR COMPOSITION:\n" +
                      "Denimium crystals contain complex arrangements of compressed carbon matrices interwoven with trace metallic elements. Subjected to extreme pressures over geological timescales, these formations develop interlocking crystalline structures that grow stronger under stress.\n\n" +
                      "STRUCTURAL PROPERTIES:\n" +
                      "• Compressive strength: 15,000 PSI (exceeds titanium by 300%)\n" +
                      "• Pressure resistance: Increases exponentially with depth\n" +
                      "• Thermal stability: Maintains integrity up to 2,000°C\n" +
                      "• Chemical inertness: Highly resistant to corrosion\n\n" +
                      "ALTERRA ASSESSMENT:\n" +
                      "This material represents a breakthrough in deep-sea construction technology. Preliminary testing suggests Denimium crystals could be processed into Reinforcement Alloy - a superior replacement for lithium-based compounds in pressure-resistant applications. Blueprint integration is recommended.\n\n" +
                      "The discovery has immediate implications for habitat construction beyond the 1000-meter depth threshold, where conventional materials fail. Priority acquisition is advised for deep-sea installation projects.\n\n" +
                      "// BLUEPRINT ADDED TO DATABANK //",
                image: denimiumTexture,
                popupImage: denimiumPopup,
                unlockSound: PDAHandler.UnlockBasic
            );

            PDAHandler.AddCustomScannerEntry(
                new PDAScanner.EntryData()
                {
                    key = denimiumTechType,
                    blueprint = TechType.None, // We'll handle unlocking custom reinforcements
                    destroyAfterScan = false,
                    encyclopedia = "DenimiumCrystal",
                    isFragment = false,
                    locked = false,
                    scanTime = 3f,
                    totalFragments = 1
                }
            );

            Plugin.Logger.LogInfo("[DenimiumCrystalPDA] Denimium Crystal PDA entry registered successfully.");
        }
    }
}