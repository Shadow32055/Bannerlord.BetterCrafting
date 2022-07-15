using HarmonyLib;
using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.ViewModelCollection.WeaponCrafting;
using TaleWorlds.CampaignSystem.ViewModelCollection.WeaponCrafting.Smelting;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace BetterCrafting.Patches
{
    [HarmonyPatch(typeof(SmeltingVM), "SmeltSelectedItems", new Type[] { typeof(Hero) })]
    public class SmeltingVMPatch
    {
        private static bool shouldRun = true;
        private static bool Prefix(SmeltingVM __instance, Hero currentCraftingHero)
        {
            if (!SmeltingVMPatch.shouldRun)
            {
                return true;
            }

            SmeltingItemVM smeltingItemVM = __instance.CurrentSelectedItem;

            if (smeltingItemVM != null)
            {

                ItemObject item = smeltingItemVM.EquipmentElement.Item;
                CraftingResourceItemVM? craftingResourceItemVM = null;

                foreach (CraftingResourceItemVM craftingResourceItemVM2 in smeltingItemVM.InputMaterials)
                {
                    if (craftingResourceItemVM2.ResourceItem.ToString().Equals("charcoal"))
                    {
                        craftingResourceItemVM = craftingResourceItemVM2;
                    }
                }

                if (craftingResourceItemVM == null)
                {
                    return false;
                }

                if (smeltingItemVM != null)
                {

                    int smithingRepeats = SubModule.datasource.GetMultiplier();



                    ICraftingCampaignBehavior campaignBehavior = Campaign.Current.GetCampaignBehavior<ICraftingCampaignBehavior>();
                    int heroCraftingStamina = campaignBehavior.GetHeroCraftingStamina(currentCraftingHero);

                    ItemRoster itemRoster = MobileParty.MainParty.ItemRoster;

                    //Get number of charcoal in the inventory
                    int charcoalAmount = itemRoster.GetItemNumber(craftingResourceItemVM.ResourceItem);

                    //Get number of items to smelt in the inventory
                    int itemAmount = itemRoster.GetItemNumber(item);

                    int val = Math.Min(Math.Min(itemAmount * craftingResourceItemVM.ResourceAmount, charcoalAmount), smithingRepeats);


                    int energyCostForRefining = Campaign.Current.Models.SmithingModel.GetEnergyCostForSmelting(item, currentCraftingHero);

                    int totalEnergyCost = energyCostForRefining * smithingRepeats;

                    if (totalEnergyCost > heroCraftingStamina)
                    {
                        //Adjust repeats to what hero has stamina for
                        smithingRepeats = heroCraftingStamina / energyCostForRefining;
                        totalEnergyCost = smithingRepeats * energyCostForRefining;
                    }

                    //SmeltingVMPatch.WriteLog(string.Format("Smelt amount: {0}", num4));

                    SmeltingVMPatch.shouldRun = false;
                    if (smeltingItemVM.InputMaterials != null && smeltingItemVM.InputMaterials.Count > 0)
                    {
                        while (__instance.CurrentSelectedItem != null && smeltingItemVM != null && smithingRepeats > 0)
                        {
                            __instance.TrySmeltingSelectedItems(currentCraftingHero);
                            smithingRepeats--;

                            bool flag5;

                            if (smeltingItemVM == __instance.CurrentSelectedItem)
                            {
                                MBBindingList<CraftingResourceItemVM> inputMaterials = smeltingItemVM.InputMaterials;
                                flag5 = (inputMaterials == null || inputMaterials.Count <= 0);
                            }
                            else
                            {
                                flag5 = false;
                            }

                            if (flag5)
                            {
                                break;
                            }
                        }
                    }
                    SmeltingVMPatch.shouldRun = true;
                }
            }
            return false;
        }
    }
}
