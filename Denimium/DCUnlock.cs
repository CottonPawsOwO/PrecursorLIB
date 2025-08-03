using Nautilus.Handlers;
using PrecursorLibrary;
using UnityEngine;

namespace ArchiTech
{
    public static class ReinforcementUnlock
    {
        public static void ModifyReinforcementUnlock()
        {
            Plugin.Logger.LogInfo("[ReinforcementUnlock] Modifying Reinforcement blueprint unlock condition...");

            // Remove the default unlock for Reinforcement (if it's unlocked by default)
            // Make it locked initially
            KnownTechHandler.RemoveDefaultUnlock(TechType.BaseReinforcement);

            // Set up the unlock to trigger when analyzing Denimium Crystal
            // This will show the "New blueprint acquired" notification! OwO
            KnownTechHandler.SetAnalysisTechEntry(Items.PrecursorMaterials.DenimiumCrystalItem.Info.TechType, new TechType[] { TechType.BaseReinforcement });

            Plugin.Logger.LogInfo("[ReinforcementUnlock] Reinforcement blueprint will now be unlocked by analyzing Denimium Crystal with proper notification! OwO");
        }
    }
}
