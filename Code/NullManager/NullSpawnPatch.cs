using System.Collections.Generic;
using HarmonyLib;
using Photon.Pun;
using UnboundLib;
using UnityEngine;
using ModdingUtils.Utils;
 namespace Nullmanager{   
[HarmonyPatch(typeof(CardChoice),"SpawnUniqueCard")]
class SpawnNulls
{
    [HarmonyPriority(Priority.Last)]
    private static void Postfix(List<GameObject> ___spawnedCards, int ___pickrID, ref GameObject __result){
        
        var player = GetPlayerWithID(___pickrID);
        if(player != null && !__result.GetComponent<CardInfo>().sourceCard.GetNullData().isAntiCard 
         && __result.GetComponent<CardInfo>().sourceCard.GetNullData().nullAble && nullTotal(___spawnedCards) < player.data.stats.GetNullData().nulls){
            GameObject old = __result;
            Main.instance.ExecuteAfterFrames(3, ()=>PhotonNetwork.Destroy(old));
            __result = PhotonNetwork.Instantiate(NullManager.instance.NullCard.name, __result.transform.position, __result.transform.rotation, 0, new object[]{__result.transform.localScale,__result.GetComponent<CardInfo>().sourceCard.name,___pickrID});
            __result.name = old.name;
        }
    }

    private static int nullTotal(List<GameObject> spawnedCards){
        int count = 0;
        foreach(var Card in spawnedCards){
            var source = Card.GetComponent<CardInfo>().sourceCard;
            if(source is NullCardInfo info)
                count += NullManager.instance.GetNullValue(info.NulledSorce.rarity);
        }
        return count;
    }

    
    internal static Player GetPlayerWithID(int playerID)
    {
        for (int i = 0; i < PlayerManager.instance.players.Count; i++)
        {
            if (PlayerManager.instance.players[i].playerID == playerID)
            {
                return PlayerManager.instance.players[i];
            }
        }
        return null;
    }

    [HarmonyPatch(typeof(Cards),"PlayerIsAllowedCard")]
    [HarmonyPostfix]
    private static void PatchAllowed(Player player, CardInfo card, ref bool __result){
        if(__result && card.GetNullData().isAntiCard){
            if(CardChoice.instance.IsPicking && player.playerID == CardChoice.instance.pickrID){
                if(nullTotal((List<GameObject>)CardChoice.instance.GetFieldValue("spawnedCards")) < player.data.stats.GetNullData().nulls){
                    __result = true;
                    return;
                }
            }
            __result = false;
        }
        if(__result && card.GetNullData().needsNull){
            if(player.data.stats.GetNullData().nulls > 0 || player.GetNullCount() >0){
                __result = true;
                return;
            }
            __result = false;
        }
    }
}
}