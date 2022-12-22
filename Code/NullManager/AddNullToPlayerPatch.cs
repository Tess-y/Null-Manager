using System;
using System.Linq;
using HarmonyLib;
using ModdingUtils.Utils;
using Photon.Pun;
using UnboundLib;
using UnboundLib.Utils;

namespace Nullmanager{
[HarmonyPatch]
internal class AddNullToPlayerPatch
{
    [HarmonyPatch(typeof(Cards),"AddCardToPlayer",new Type[]{typeof(Player), typeof(CardInfo), typeof(bool), typeof(string), typeof(float), typeof(float), typeof(bool)})]
    [HarmonyPrefix]
    public static bool Add(Player player, CardInfo card, bool reassign, string twoLetterCode, float forceDisplay, float forceDisplayDelay, bool addToCardBar){
        if (card is NullCardInfo nullCard){
            if (nullCard.randomCard) nullCard = NullManager.instance.GetRandomNullWithRarity(player,nullCard.rarity);
            if (PhotonNetwork.OfflineMode)
            {
                card = NullManager.instance.GetNullCardInfo(nullCard.NulledSorce.name,player);
                player.data.currentCards.Add(card);
                    
                NullManager.instance.callbacks.ForEach(c => {
                    try{
                        c.Invoke((NullCardInfo)card,player);
                    }catch{}
                });

                if (addToCardBar)
                {
                    Cards.SilentAddToCardBar(player.playerID, card, twoLetterCode, forceDisplay, forceDisplayDelay);
                }
            }
            else if(PhotonNetwork.IsMasterClient){
                if (addToCardBar)
                {
                    NetworkingManager.RPC(typeof(Cards), "RPCA_AssignCard", new object[] { $"___NULL___{nullCard.NulledSorce.name}", player.playerID, reassign, twoLetterCode, forceDisplay, forceDisplayDelay });
                }
                else
                {
                    NetworkingManager.RPC(typeof(Cards), "RPCA_AssignCardWithoutCardBar", new object[] { $"___NULL___{nullCard.NulledSorce.name}", player.playerID, reassign, twoLetterCode, forceDisplay, forceDisplayDelay });
                }
            }
            return false;
        }
        return true;
    }

    [HarmonyPatch(typeof(Cards),"RPCA_AssignCard",new Type[]{typeof(string), typeof(int), typeof(bool), typeof(string), typeof(float), typeof(float), typeof(bool)})]
    [HarmonyPrefix]
    public static bool RPC(string cardObjectName, int playerID, bool reassign, string twoLetterCode, float forceDisplay, float forceDisplayDelay, bool addToCardBar){
        if(cardObjectName.StartsWith("___NULL___")){
            Player playerToUpgrade;
            playerToUpgrade = (Player)PlayerManager.instance.InvokeMethod("GetPlayerWithID", playerID);
            NullCardInfo card = NullManager.instance.GetNullCardInfo(cardObjectName.Substring(10), playerToUpgrade);
            playerToUpgrade.data.currentCards.Add(card);
            
            NullManager.instance.callbacks.ForEach(c => {
                try{
                    c.Invoke(card,playerToUpgrade);
                }catch{}
            });
            if (addToCardBar)
            {
                Cards.SilentAddToCardBar(playerToUpgrade.playerID, card, twoLetterCode, forceDisplay, forceDisplayDelay);
            }
            return false;
        }
        return true;
    }

    [HarmonyPatch(typeof(CardManager),nameof(CardManager.GetCardInfoWithName))]
    [HarmonyPostfix]
    public static void GetCardInfoWithName(string cardName, ref CardInfo __result){
        if(__result == null && cardName.StartsWith("___NULL___")){
            __result =  NullManager.instance.GetNullCardInfo(cardName.Substring(10),-1);
        }
    }

}
}