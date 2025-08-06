using Nautilus.Handlers;
using PrecursorLibrary;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace ArchiTech
{
    public static class EmeraldPDA
    {
        private static Texture2D LoadTextureFromFile(string fileName)
        {
            try
            {
                string pluginPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                string assetsPath = Path.Combine(pluginPath, "Assets", "PDA");
                string filePath = Path.Combine(assetsPath, fileName);

                Plugin.Logger.LogInfo($"[EmeraldPDA] Looking for texture: {filePath}");

                if (!File.Exists(filePath))
                {
                    Plugin.Logger.LogWarning($"[EmeraldPDA] Texture file not found: {filePath}");
                    return null;
                }

                byte[] fileData = File.ReadAllBytes(filePath);

                Texture2D tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
                tex.filterMode = FilterMode.Bilinear;
                tex.wrapMode = TextureWrapMode.Clamp;

                if (tex.LoadImage(fileData))
                {
                    tex.Apply();
                    Plugin.Logger.LogInfo($"[EmeraldPDA] Successfully loaded texture: {fileName} ({tex.width}x{tex.height})");
                    return tex;
                }

                UnityEngine.Object.DestroyImmediate(tex);
                return null;
            }
            catch (System.Exception ex)
            {
                Plugin.Logger.LogError($"[EmeraldPDA] Exception loading texture: {ex}");
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

        public static void Register(TechType emeraldTechType)
        {
            Plugin.Logger.LogInfo("[EmeraldPDA] Registering Emerald PDA entry...");

            Texture2D emeraldTexture = LoadTextureFromFile("EmeraldEntry.png");
            Sprite emeraldPopup = CreateSpriteFromTexture(emeraldTexture);

            if (emeraldTexture == null)
            {
                Plugin.Logger.LogWarning("[EmeraldPDA] Custom texture failed to load, entry will have no image");
            }

            PDAHandler.AddEncyclopediaEntry(
    key: "Emerald",
    path: "PlanetaryGeology",
    title: "Emerald",
    desc: "A beautiful green crystalline mineral discovered in the deeper cavern systems of 4546B. Its structure appears unusually perfect compared to naturally occurring gems.\n\n" +
          "1. Crystal Formation:\n" +
          "These emeralds exhibit a flawless hexagonal crystal structure that defies conventional geological formation theories. The precision of their atomic arrangement suggests formation under unknown conditions or processes.\n\n" +
          "2. Optical Properties:\n" +
          "The crystals emit a distinctive blue-green luminescence when exposed to certain wavelengths of light. This phenomenon appears to be caused by an unidentified trace element within the crystal matrix.\n\n" +
          "3. Electromagnetic Anomalies:\n" +
          "Sensitive instruments detect subtle electromagnetic fluctuations emanating from larger specimens. The source of this activity remains unexplained, though it may be related to the mineral's unique atomic structure.\n\n" +
          "4. Distribution Patterns:\n" +
          "Emerald deposits are consistently found in proximity to ancient geological formations and structures of unknown origin. This correlation suggests a possible connection to prehistoric geological events on 4546B.\n\n" +
          "The crystals' exceptional clarity and mysterious properties make them valuable for both research and advanced technological applications. Their presence often indicates areas of significant geological interest.\n\n" +
          "Assessment: Rare crystalline mineral with unusual properties. Recommended for collection and further analysis.",
    image: emeraldTexture,
    popupImage: emeraldPopup,
    unlockSound: PDAHandler.UnlockBasic
);


            PDAHandler.AddCustomScannerEntry(
                key: emeraldTechType,
                scanTime: 2f,
                destroyAfterScan: false,
                encyclopediaKey: "Emerald"
            );

            Plugin.Logger.LogInfo("[EmeraldPDA] Emerald PDA entry registered successfully.");
        }
    }
}