using System.Linq;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using UnboundLib;
using TMPro;
namespace Nullmanager {
    [HarmonyPatch(typeof(CardChoice), "AddCardVisual")]
    class NullCardBar {
        private static bool Prefix(CardInfo cardToSpawn, Vector3 pos, ref GameObject __result) {
            if(cardToSpawn is NullCardInfo nullCard) {
                GameObject temp = new GameObject();
                nullCard=NullManager.instance.GetNullCardInfo(nullCard.NulledSorce.name, nullCard.PlayerId);
                temp.SetActive(false);
                __result=GameObject.Instantiate(nullCard.NulledSorce.gameObject, pos, Quaternion.identity, temp.transform);
                CardInfo card = __result.GetComponent<CardInfo>();
                card.cardDestription="";
                card.cardStats=nullCard.cardStats;
                Main.instance.ExecuteAfterFrames(2, () => {
                    if(card!=null)
                        card.GetComponentsInChildren<Image>().ToList().ForEach(image => {
                            image.sprite=Main.Assets.LoadAsset<Sprite>("missing_texture");
                            image.type=Image.Type.Tiled;
                        });
                    card.GetComponentsInChildren<TextMeshProUGUI>().ToList().ForEach(text => {
                        if(text.text==card.cardName.ToUpper())
                            text.font=Main.Assets.LoadAsset<TMP_FontAsset>("Nightmare");
                    });
                });
                __result.transform.SetParent(null, true);
                GameObject.Destroy(temp);
                __result.GetComponentInChildren<CardVisuals>().firstValueToSet=true;
                return false;
            }
            return true;
        }
    }
}