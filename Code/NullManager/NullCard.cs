using System.Linq;
using Photon.Pun;
using UnboundLib;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
namespace Nullmanager {
    public class NullCard: MonoBehaviour, IPunInstantiateMagicCallback {
        public void OnPhotonInstantiate(PhotonMessageInfo info) //Scale,Card,playerID
        {
            var data = info.photonView.InstantiationData;
            transform.localScale=(Vector3)data[0];
            GetComponent<CardInfo>().sourceCard=NullManager.instance.GetNullCardInfo((string)data[1], PlayerManager.instance.players.Find(p => p.playerID==(int)data[2]));
            GetComponent<CardInfo>().cardBase=GetComponent<CardInfo>().sourceCard.cardBase;
            GetComponent<CardInfo>().rarity=GetComponent<CardInfo>().sourceCard.rarity;
            Main.instance.ExecuteAfterFrames(2, () => {
                UnityEngine.Object.DestroyImmediate(gameObject.transform.GetChild(0).gameObject);
                gameObject.SetActive(false);
                var temp = GameObject.Instantiate(((NullCardInfo)GetComponent<CardInfo>().sourceCard).NulledSorce.gameObject, transform);
                temp.GetComponent<CardInfo>().cardDestription="";
                temp.GetComponent<CardInfo>().cardStats=GetComponent<CardInfo>().sourceCard.cardStats;
                gameObject.SetActive(true);
                Main.instance.ExecuteAfterFrames(4, () => {
                    for(int i = 0; i<temp.transform.childCount; i++)
                        temp.transform.GetChild(i).SetParent(transform, false);
                    UnityEngine.Object.DestroyImmediate(temp);
                    GetComponentsInChildren<Image>().ToList().ForEach(image => {
                        image.sprite=Main.Assets.LoadAsset<Sprite>("missing_texture");
                        image.type=Image.Type.Tiled;
                    });
                    GetComponentsInChildren<TextMeshProUGUI>().ToList().ForEach(text => {
                        if(text.text==((NullCardInfo)GetComponent<CardInfo>().sourceCard).NulledSorce.cardName.ToUpper())
                            text.font=Main.Assets.LoadAsset<TMP_FontAsset>("Nightmare");
                    });
                });
            });
        }
    }
}