using BetterCrafting.Utils;
using HarmonyLib;
using System;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace BetterCrafting.Patches {
    [HarmonyPatch(typeof(Crafting))]
    class CraftingPatch {
        static string[] prefixes = { "[Broken] ", "[Rusty] ", "[Crude] ", "", "[Fine] ", "[Masterwork] ", "[Legendary] "};
        static string startSearchString = "[";

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Crafting), nameof(Crafting.CraftedWeaponName), MethodType.Getter)]
        public static void postfix(ref TextObject __result, ref ItemObject __instance) {
            if (__result.Contains(startSearchString)) {
                foreach (string s in prefixes) {
                    if (__result.Contains(s)) {
                        return;
                    }
                }
            }

            //__instance2.CraftedWeaponName = new TextObject(__instance1.Tier.ToString(), null);

            __result = new TextObject(GetWeaponTierPrefix(((int)__instance.Tier)) + __result.ToString());
        }

        public static string GetWeaponTierPrefix(int weaponTier) {
            return prefixes[weaponTier + 3];
        }   
    }
}
