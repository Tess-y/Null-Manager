using System.Linq;
using HarmonyLib;

namespace Nullmanager {
    [HarmonyPatch(typeof(ApplyCardStats), "ApplyStats")]
    internal class ApplyCardStatsPatch {
        private static void Postfix(ApplyCardStats __instance, Player ___playerToUpgrade) {
            if(__instance.GetComponent<CardInfo>() is NullCardInfo nullCard) {
                ___playerToUpgrade.data.stats.AjustNulls(-NullManager.instance.GetNullValue(nullCard.rarity));
                NullManager.instance.callbacks.ForEach(c => {
                    try {
                        c.Invoke(nullCard, ___playerToUpgrade);
                    } catch { }
                });
            }
            if(__instance.GetComponent<CardInfo>().sourceCard is NullCardInfo nullCard2) {
                ___playerToUpgrade.data.stats.AjustNulls(-NullManager.instance.GetNullValue(nullCard2.rarity));
                NullManager.instance.callbacks.ForEach(c => {
                    try {
                        c.Invoke(nullCard2, ___playerToUpgrade);
                    } catch { }
                });
            }
        }
    }
}