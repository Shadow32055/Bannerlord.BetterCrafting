using BetterCrafting.Utils;
using HarmonyLib;
using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.ViewModelCollection.WeaponCrafting.Refinement;
using static TaleWorlds.Core.Crafting;

namespace BetterCrafting.Patches
{
    [HarmonyPatch(typeof(RefinementVM), "ExecuteSelectedRefinement", new Type[] { typeof(Hero) })]
    public class RefinementVMPatch
    {

        private static bool refinmentPatchActive = true;
        private static bool Prefix(RefinementVM __instance, Hero currentCraftingHero)
        {
            if (!RefinementVMPatch.refinmentPatchActive)
            {
                return true;
            }

            if (__instance.CurrentSelectedAction != null)
            {

                int refinmentRepeats = SubModule.datasource.GetMultiplier();

                ICraftingCampaignBehavior campaignBehavior = Campaign.Current.GetCampaignBehavior<ICraftingCampaignBehavior>();
                int heroCraftingStamina = campaignBehavior.GetHeroCraftingStamina(currentCraftingHero);

                RefiningFormula refineFormula = __instance.CurrentSelectedAction.RefineFormula;
                int energyCostForRefining = Campaign.Current.Models.SmithingModel.GetEnergyCostForRefining(ref refineFormula, currentCraftingHero);

                int totalEnergyCost = energyCostForRefining * refinmentRepeats;

                if (totalEnergyCost > heroCraftingStamina)
                {
                    //Adjust repeats to what hero has stamina for
                    refinmentRepeats = heroCraftingStamina / energyCostForRefining;
                    totalEnergyCost = refinmentRepeats * energyCostForRefining;
                }

                //Disable prefix patch to pervent stackoverflow issue
                RefinementVMPatch.refinmentPatchActive = false;

                Helper.DisplayFriendlyMsg(String.Format("Spent {0} stamina to refine {1} product(s).", totalEnergyCost, refinmentRepeats));
                for (int i = 0; i <= refinmentRepeats; i++)
                {
                    __instance.ExecuteSelectedRefinement(currentCraftingHero);
                }

                //finished multi crafting re-enable the patch for next run
                RefinementVMPatch.refinmentPatchActive = true;
            }
            return false;
        }
    }
}
