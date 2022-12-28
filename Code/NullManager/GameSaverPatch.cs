
using GameSaver.Util;
using HarmonyLib;
using System;
using System.Collections.Generic;

namespace Nullmanager {
    public class GameSaverPatch {
        public static void Patch() {
            Main.harmony.Patch(typeof(SaveManager.PlayerData).GetConstructor(new Type[] { typeof(string), typeof(int), typeof(List<CardInfo>), typeof(int), typeof(int), typeof(bool), typeof(ulong) }),
               postfix: new HarmonyMethod(typeof(GameSaverPatch).GetMethod(nameof(Constructor))));
        }

        public static void Constructor(List<CardInfo> cards, SaveManager.PlayerData __instance) {
            for(int i = 0; i<cards.Count; i++) {
                if(cards[i] is NullCardInfo nullCard) {
                    __instance.serializedCards[i]=$"___NULL___{nullCard.NulledSorce.name}";
                }
            }
        }
    }

}