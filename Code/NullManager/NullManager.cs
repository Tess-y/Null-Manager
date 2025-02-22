using CardChoiceSpawnUniqueCardPatch.CustomCategories;
using Photon.Pun;
using RarityLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using UnboundLib;
using UnboundLib.Utils;
using UnityEngine;
namespace Nullmanager {
    public class NullManager: MonoBehaviour {
        public static NullManager instance;
        public GameObject AntiCardBase;
        public GameObject NullCard;
        private Dictionary<int, Dictionary<string, NullCardInfo>> nullDic = new Dictionary<int, Dictionary<string, NullCardInfo>>();
        private Dictionary<int, Dictionary<string, CardInfoStat[]>> NullStats = new Dictionary<int, Dictionary<string, CardInfoStat[]>>();
        private Dictionary<int, Dictionary<CardInfo.Rarity, Dictionary<string, CardInfoStat[]>>> NullRarityStats = new Dictionary<int, Dictionary<CardInfo.Rarity, Dictionary<string, CardInfoStat[]>>>();
        internal List<Action<NullCardInfo, Player>> callbacks = new List<Action<NullCardInfo, Player>>();


        internal void SetUp() {
            RarityLib.Utils.RarityUtils.Rarities.Values.ToList().ForEach(rarity => {
                NullCardInfo info = gameObject.AddComponent<NullCardInfo>();
                info.rarity=rarity.value;
                info.name="[]";
                info.cardDestription="Adds a random nulled card of this rarity to a player";
                info.randomCard=true;
                info.categories=new CardCategory[] { CustomCardCategories.instance.CardCategory("nullCard") };
                info.blacklistedCategories=new CardCategory[0];
                ModdingUtils.Utils.Cards.instance.AddHiddenCard(info);
            });
        }


        public int GetNullValue(CardInfo.Rarity rarity) {
            return (int)(1f/RarityUtils.GetRarityData(rarity).relativeRarity);
        }
        public NullCardInfo GetNullCardInfo(string card, int player) {
            if(!nullDic.ContainsKey(player))
                nullDic[player]=new Dictionary<string, NullCardInfo>();
            var infoDic = nullDic[player];
            if(!infoDic.ContainsKey(card)) {
                var info = gameObject.AddComponent<NullCardInfo>();
                info.NulledSorce=((DefaultPool)PhotonNetwork.PrefabPool).ResourceCache[card].GetComponent<CardInfo>();
                info.PlayerId=player;
                info.cardName="[]"+info.NulledSorce.cardName;
                info.rarity=info.NulledSorce.rarity;
                info.blacklistedCategories=new CardCategory[0];
                info.GetNullData().isAntiCard=false;
                info.sourceCard=info;
                info.categories=new CardCategory[] { CustomCardCategories.instance.CardCategory("nullCard") };
                info.cardBase = info.NulledSorce.cardBase;
                info.colorTheme = info.NulledSorce.colorTheme;
                info.cardArt = info.NulledSorce.cardArt;
                infoDic[card]=info;
            }
            int amount = GetNullValue(infoDic[card].NulledSorce.rarity);
            List<CardInfoStat> list = new List<CardInfoStat>();
            list.Add(new CardInfoStat { positive=true, stat=$"null{(amount==1 ? "" : "s")}", amount=$"- <b>{amount}</b> " });
            if(NullStats.ContainsKey(player))
                NullStats[player].Values.ToList().ForEach(stats => stats.ToList().ForEach(stat => list.Add(stat)));
            if(NullRarityStats.ContainsKey(player))
                if(NullRarityStats[player].ContainsKey(infoDic[card].NulledSorce.rarity))
                    NullRarityStats[player][infoDic[card].NulledSorce.rarity].Values.ToList().ForEach(stats => stats.ToList().ForEach(stat => list.Add(stat)));
            infoDic[card].cardStats=list.ToArray();
            return infoDic[card];

        }
        public NullCardInfo GetNullCardInfo(string card, Player player) {
            return GetNullCardInfo(card, player.playerID);
        }

        public NullCardInfo GetRandomNullWithRarity(Player player, CardInfo.Rarity rarity) {
            var cards = CardManager.cards.Values.ToArray().Where(cardData => cardData.enabled&&cardData.cardInfo.rarity==rarity
            &&ModdingUtils.Utils.Cards.instance.PlayerIsAllowedCard(player, cardData.cardInfo)).Select(card => card.cardInfo.name).ToList();
            if(cards.Count==0)
                cards.Add("__NULL__  ");
            cards.Shuffle();
            return GetNullCardInfo(cards[0], player);
        }

        public void RegesterOnAddCallback(Action<NullCardInfo, Player> action) {
            callbacks.Add(action);
        }

        public void SetAdditionalNullStats(Player player, string key, CardInfoStat[] stats) {
            if(!NullStats.ContainsKey(player.playerID))
                NullStats[player.playerID]=new Dictionary<string, CardInfoStat[]>();
            NullStats[player.playerID][key]=stats;
        }

        public void SetRarityNullStats(Player player, CardInfo.Rarity rarity, string key, CardInfoStat[] stats) {
            if(!NullRarityStats.ContainsKey(player.playerID))
                NullRarityStats[player.playerID]=new Dictionary<CardInfo.Rarity, Dictionary<string, CardInfoStat[]>>();
            if(!NullRarityStats[player.playerID].ContainsKey(rarity))
                NullRarityStats[player.playerID][rarity]=new Dictionary<string, CardInfoStat[]>();
            NullRarityStats[player.playerID][rarity][key]=stats;
        }
    }
}