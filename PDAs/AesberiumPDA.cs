using Nautilus.Handlers;
using PrecursorLibrary;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace ArchiTech
{
    public static class AesberiumPDA
    {
        private static Texture2D LoadTextureFromFile(string fileName)
        {
            try
            {
                string pluginPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                string assetsPath = Path.Combine(pluginPath, "Assets", "PDA");
                string filePath = Path.Combine(assetsPath, fileName);

                Plugin.Logger.LogInfo($"[AesberiumPDA] Looking for texture in subfolder: {filePath}");

                if (!File.Exists(filePath))
                {
                    Plugin.Logger.LogWarning($"[AesberiumPDA] Texture file not found in Assets/PDA: {filePath}");
                    return null;
                }

                byte[] fileData = File.ReadAllBytes(filePath);

                Texture2D tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
                tex.filterMode = FilterMode.Bilinear;
                tex.wrapMode = TextureWrapMode.Clamp;

                if (tex.LoadImage(fileData))
                {
                    tex.Apply();
                    Plugin.Logger.LogInfo($"[AesberiumPDA] Successfully loaded texture: {fileName} ({tex.width}x{tex.height})");
                    return tex;
                }

                UnityEngine.Object.DestroyImmediate(tex);
                return null;
            }
            catch (System.Exception ex)
            {
                Plugin.Logger.LogError($"[AesberiumPDA] Exception loading texture: {ex}");
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

        public static void Register(TechType aesberiumDrillableTechType)
        {
            Plugin.Logger.LogInfo("[AesberiumPDA] Registering Aesberium PDA entry...");

            Texture2D aesberiumTexture = LoadTextureFromFile("AesbEntry.png");
            Sprite aesberiumPopup = CreateSpriteFromTexture(aesberiumTexture);

            if (aesberiumTexture == null)
            {
                Plugin.Logger.LogWarning("[AesberiumPDA] Custom texture failed to load, entry will have no image");
            }

            PDAHandler.AddEncyclopediaEntry(
                key: "Aesberium",
                path: "PlanetaryGeology",
                title: "Aesberium",
                desc: "A highly durable metallic mineral displaying unprecedented structural properties, discovered in mountainous regions. Advanced analysis reveals an extraordinary molecular composition consistent with theoretical construction materials.\n\n" +
                      "MOLECULAR COMPOSITION:\n" +
                      "Aesberium exhibits complex lattice structures with interlocking metallic bonds that demonstrate theoretical construction capabilities. The material's atomic arrangement suggests advanced engineering principles, possibly indicating non-terrestrial origins or extremely rare geological processes.\n\n" +
                      "STRUCTURAL PROPERTIES:\n" +
                      "• Durability: Unprecedented - exceeds all known terrestrial materials\n" +
                      "• Structural integrity: Maintains cohesion under extreme stress\n" +
                      "• Thermal resistance: Stable across temperature extremes\n" +
                      "• Unknown structure: Molecular arrangement defies conventional classification\n\n" +
                      "ALTERRA ASSESSMENT:\n" +
                      "This material represents a significant breakthrough in construction science. Preliminary analysis suggests potential applications in theoretical construction projects requiring maximum durability and structural integrity. The unknown molecular structure indicates advanced material science beyond current understanding.\n\n" +
                      "RESEARCH PRIORITY: HIGH\n" +
                      "Further study recommended to unlock the full potential of this remarkable material. Initial findings suggest possible connections to advanced architectural applications requiring unprecedented durability standards.\n\n" +
                      "// MATERIAL PROPERTIES LOGGED FOR ADVANCED RESEARCH //",
                image: aesberiumTexture,
                popupImage: aesberiumPopup,
                unlockSound: PDAHandler.UnlockBasic
            );

            PDAHandler.AddCustomScannerEntry(
                new PDAScanner.EntryData()
                {
                    key = aesberiumDrillableTechType,
                    blueprint = TechType.None,
                    destroyAfterScan = false,
                    encyclopedia = "Aesberium",
                    isFragment = false,
                    locked = false,
                    scanTime = 4f,
                    totalFragments = 1
                }
            );

            Plugin.Logger.LogInfo("[AesberiumPDA] Aesberium PDA entry registered successfully.");
        }
    }
}