using System;
using System.Linq;
using System.Runtime.CompilerServices;
using HarmonyLib;
using UnityEngine;
namespace Nullmanager{
[Serializable]
public class CharacterStatModifiersnullData
{
    public int nulls;
    public CharacterStatModifiersnullData()
    {
        nulls = 0;
    }
    public void Reset(){

    }
}
public static class CharacterStatModifiersExtension
{
    public static readonly ConditionalWeakTable<CharacterStatModifiers, CharacterStatModifiersnullData> data =
        new ConditionalWeakTable<CharacterStatModifiers, CharacterStatModifiersnullData>();


    internal static CharacterStatModifiersnullData GetNullData(this CharacterStatModifiers characterstats)
    {
        return data.GetOrCreateValue(characterstats);
    }

    public static void AjustNulls(this CharacterStatModifiers characterstats, int value)
    {
        characterstats.GetNullData().nulls = Mathf.Clamp(characterstats.GetNullData().nulls+value,0,1000000);
    }
    public static int GetNulls(this CharacterStatModifiers characterstats)
    {
        return characterstats.GetNullData().nulls;
    }

    internal static void AddData(this CharacterStatModifiers characterstats, CharacterStatModifiersnullData value)
    {
        try
        {
            data.Add(characterstats, value);
        }
        catch (Exception) { }
    }

}
// reset additional CharacterStatModifiers when ResetStats is called
[HarmonyPatch(typeof(CharacterStatModifiers), "ResetStats")]
class CharacterStatModifiersPatchResetStats
{
    private static void Prefix(CharacterStatModifiers __instance)
    {
        __instance.GetNullData().Reset();
    }
}


[Serializable]
public class CardInfoAdditionalData
{
    public bool isAntiCard;
    public bool nullAble;
    public bool needsNull;

    public CardInfoAdditionalData()
    {
        isAntiCard = false;
        nullAble = true;
        needsNull = false;
    }
}
public static class CardInfoExtension
{
    public static readonly ConditionalWeakTable<CardInfo, CardInfoAdditionalData> data =
        new ConditionalWeakTable<CardInfo, CardInfoAdditionalData>();

    internal static CardInfoAdditionalData GetNullData(this CardInfo cardInfo)
    {
        return data.GetOrCreateValue(cardInfo);
    }

    public static CardInfo SetAntiCard(this CardInfo cardInfo){
        cardInfo.GetNullData().isAntiCard = true;
        return cardInfo;
    }

    public static CardInfo MarkUnNullable(this CardInfo cardInfo){
        cardInfo.GetNullData().nullAble = false;
        return cardInfo;
    }

    public static CardInfo NeedsNull(this CardInfo cardInfo){
        cardInfo.GetNullData().needsNull = true;
        return cardInfo;
    }

    public static void AddData(this CardInfo cardInfo, CardInfoAdditionalData value)
    {
        try
        {
            data.Add(cardInfo, value);
        }
        catch (Exception) { }
    }
}


public static class PlayerExtensions
{
    public static int GetNullCount(this Player player, CardInfo.Rarity rarity = (CardInfo.Rarity)(-1)){
        if(((int)rarity) == -1){
            return player.data.currentCards.Where(c=> c is NullCardInfo).Count();
        }
        return player.data.currentCards.Where(c=> c is NullCardInfo nullcard && nullcard.NulledSorce.rarity == rarity).Count();
    }

    public static int GetNullValue(this Player player, CardInfo.Rarity rarity = (CardInfo.Rarity)(-1)){
        if(((int)rarity) == -1){
            return player.data.currentCards.Where(c=> c is NullCardInfo).Select(c=>NullManager.instance.GetNullValue(c.rarity)).Sum();
        }
        return player.data.currentCards.Where(c=> c is NullCardInfo nullcard && nullcard.NulledSorce.rarity == rarity).Select(c=>NullManager.instance.GetNullValue(c.rarity)).Sum();
    }
}
}