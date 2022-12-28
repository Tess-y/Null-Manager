using System;
using HarmonyLib;
using TabInfo.Utils;
using UnboundLib;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System.Linq;

namespace Nullmanager {
    public class TabinfoInterface {
        public static void Setup() {
            var cat = TabInfoManager.RegisterCategory("null", 7);
            TabInfoManager.RegisterStat(cat, "Nulls", (p) => p.data.stats.GetNullData().nulls>0, (p) => $"{p.data.stats.GetNullData().nulls}");

            Main.harmony.Patch(typeof(PlayerCardButton).GetMethod("OnPointerEnter"), new HarmonyMethod(typeof(TabinfoInterface).GetMethod("Prefix")));
        }

        public static bool Prefix(PlayerCardButton __instance, PointerEventData eventData) {
            if(__instance.card is NullCardInfo nullCard) {
                GameObject displayedCard = (GameObject)__instance.GetFieldValue("displayedCard");
                if(displayedCard!=null) {
                    UnityEngine.GameObject.Destroy(displayedCard);
                }
                displayedCard=GameObject.Instantiate((GameObject)typeof(TabInfoManager).GetField("cardHolderTemplate", System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Static).GetValue(null),
                    ((GameObject)typeof(TabInfoManager).GetField("canvas", System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Static).GetValue(null)).transform);
                displayedCard.transform.position=__instance.gameObject.transform.position;
                displayedCard.SetActive(false);
                var cardObj = GameObject.Instantiate(nullCard.NulledSorce.gameObject, displayedCard.transform);
                CardInfo card = cardObj.GetComponent<CardInfo>();
                card.GetComponent<CardInfo>().cardDestription="";
                card.GetComponent<CardInfo>().cardStats=nullCard.cardStats;
                displayedCard.SetActive(true);
                var cardVis = cardObj.GetComponentInChildren<CardVisuals>();
                cardVis.firstValueToSet=true;
                cardObj.transform.localPosition=Vector3.zero;
                Collider2D[] componentsInChildren = displayedCard.GetComponentsInChildren<Collider2D>();
                for(int i = 0; i<componentsInChildren.Length; i++) {
                    componentsInChildren[i].enabled=false;
                }
                cardObj.GetComponentInChildren<Canvas>().sortingLayerName="MostFront";
                cardObj.GetComponentInChildren<GraphicRaycaster>().enabled=false;
                cardObj.GetComponentInChildren<SetScaleToZero>().enabled=false;
                cardObj.GetComponentInChildren<SetScaleToZero>().transform.localScale=Vector3.one*1.15f;
                __instance.ExecuteAfterFrames(1, () => {
                    cardObj.transform.localScale=Vector3.one*25f;
                });
                __instance.ExecuteAfterFrames(2, () => {
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
                __instance.SetFieldValue("displayedCard", displayedCard);
                return false;
            }
            return true;
        }

        private static GameObject Instantiate(GameObject cardHolderTemplate, Transform transform) {
            throw new NotImplementedException();
        }
    }
}