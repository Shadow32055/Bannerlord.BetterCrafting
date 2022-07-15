using HarmonyLib;
using System;
using System.Collections;
using System.Reflection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CampaignBehaviors;

namespace BetterCrafting.Patches {

    [HarmonyPatch(typeof(CraftingCampaignBehavior))]
    class CraftingCampaignBehaviorPatch {
        private static FieldInfo? recordsInfo;

        [HarmonyPrefix]
        [HarmonyPatch(typeof(CraftingCampaignBehavior), "HourlyTick")]
        static bool Prefix(ref CraftingCampaignBehavior __instance) {
            if (recordsInfo == null)
                recordsInfo = typeof(CraftingCampaignBehavior).GetField("_heroCraftingRecords", BindingFlags.Instance | BindingFlags.NonPublic);

            if (recordsInfo == null || __instance == null) throw new ArgumentNullException(nameof(__instance), $"Tried to run postfix for {nameof(CraftingCampaignBehavior)}.HourlyTickPatch but the recordsInfo or __instance was null.");

            IDictionary? records = (IDictionary)recordsInfo.GetValue(__instance);

            foreach (Hero hero in records.Keys) {
                int curCraftingStamina = __instance.GetHeroCraftingStamina(hero);

                if (curCraftingStamina < __instance.GetMaxHeroCraftingStamina(hero)) {
                    int staminaGainAmount = 5; //Native is 5?
                    float CraftingStaminaGainOutsideSettlementMultiplier = .05f;
                    if (CraftingStaminaGainOutsideSettlementMultiplier < 1 && hero.PartyBelongedTo?.CurrentSettlement == null)
                        staminaGainAmount = (int)Math.Ceiling(staminaGainAmount * CraftingStaminaGainOutsideSettlementMultiplier);

                    int diff = __instance.GetMaxHeroCraftingStamina(hero) - curCraftingStamina;
                    if (diff < staminaGainAmount)
                        staminaGainAmount = diff;

                    __instance.SetHeroCraftingStamina(hero, Math.Min(__instance.GetMaxHeroCraftingStamina(hero), curCraftingStamina + staminaGainAmount));
                }
            }
            return false;
        }

        [HarmonyPostfix]
		[HarmonyPatch(typeof(CraftingCampaignBehavior), nameof(CraftingCampaignBehavior.GetHeroCraftingStamina))]
		public static void GetHeroCraftingStamina(ref int __result, ref CraftingCampaignBehavior __instance, Hero hero) {
			__result = __instance.GetMaxHeroCraftingStamina(hero);
        }

		[HarmonyPostfix]
		[HarmonyPatch(typeof(CraftingCampaignBehavior), nameof(CraftingCampaignBehavior.GetMaxHeroCraftingStamina))]
		public static void GetMaxHeroCraftingStamina(ref int __result, ref CraftingCampaignBehavior __instance, Hero hero) {
			if (hero == Hero.MainHero)
				__result = 1000;
		}
	}
}
