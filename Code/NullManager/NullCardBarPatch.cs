using HarmonyLib;
using System.Linq;
using TMPro;
using UnboundLib;
using UnityEngine;
using UnityEngine.UI;
namespace Nullmanager {
    [HarmonyPatch(typeof(CardChoice), "AddCardVisual")]
    class NullCardBar {
        private static bool Prefix(CardInfo cardToSpawn, Vector3 pos, ref GameObject __result) {
            if(cardToSpawn is NullCardInfo nullCard) {
                GameObject temp = new GameObject();
                nullCard=NullManager.instance.GetNullCardInfo(nullCard.NulledSorce.name, nullCard.PlayerId);
                temp.SetActive(false);
                UnityEngine.Debug.Log(".Instantiate");
                __result =GameObject.Instantiate(
                    ((CardInfo)typeof(CardChoiceSpawnUniqueCardPatch.CardChoiceSpawnUniqueCardPatch).GetField("NullCard", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static).GetValue(null)).gameObject,
                    pos, Quaternion.identity, temp.transform);
                CardInfo card = __result.GetComponent<CardInfo>();
                GameObject.DestroyImmediate(card);
                card = CopyComponent<CardInfo>(nullCard, __result); 
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
                    card.GetComponentInChildren<CardVisuals>().nameText.text = card.GetComponentInChildren<CardVisuals>().nameText.text.Substring(2);
                });
                UnityEngine.Debug.Log("sanity check");
                __result.transform.SetParent(null, true);
                GameObject.Destroy(temp);
                __result.GetComponentInChildren<CardVisuals>().firstValueToSet=true;
                return false;
            }
            return true;
        }
        private static T CopyComponent<T>(T original, GameObject destination) where T : Component {
            System.Type type = original.GetType();
            Component copy = destination.AddComponent(type);
            System.Reflection.FieldInfo[] fields = type.GetFields();
            foreach(System.Reflection.FieldInfo field in fields) {
                field.SetValue(copy, field.GetValue(original));
            }
            return copy as T;
        }
    }
}