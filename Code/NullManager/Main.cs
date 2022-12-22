using BepInEx;
using HarmonyLib;
using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;
using System.Reflection;
using UnboundLib.GameModes;
namespace Nullmanager{
[BepInDependency("com.willis.rounds.unbound", BepInDependency.DependencyFlags.HardDependency)]
[BepInDependency("pykess.rounds.plugins.moddingutils", BepInDependency.DependencyFlags.HardDependency)]
[BepInDependency("pykess.rounds.plugins.cardchoicespawnuniquecardpatch", BepInDependency.DependencyFlags.HardDependency)]
[BepInDependency("com.willuwontu.rounds.tabinfo", BepInDependency.DependencyFlags.SoftDependency)]
[BepInDependency("ot.dan.rounds.gamesaver", BepInDependency.DependencyFlags.SoftDependency)]
[BepInPlugin(ModId, ModName, Version)]
[BepInProcess("Rounds.exe")]
public class Main : BaseUnityPlugin
{
    private const string ModId = "com.Root.Null";
    private const string ModName = "NullManager";
    public const string Version = "1.0.1";  
    internal static AssetBundle Assets;
    internal static Harmony harmony;
    public static Main instance { get; private set; }

    //    <EmbeddedResource Include="Assets\AssetBundles\nullassets" />
    void Awake()
    { 
        harmony = new Harmony(ModId);
        harmony.PatchAll();
        instance = this;
        Assets = Jotunn.Utils.AssetUtils.LoadAssetBundleFromResources("nullassets", typeof(Main).Assembly);
        NullManager.instance = Assets.LoadAsset<GameObject>("NullManger").GetComponent<NullManager>();
        PhotonNetwork.PrefabPool.RegisterPrefab(NullManager.instance.NullCard.name,NullManager.instance.NullCard);

    }

    void Start(){
        var plugins = (List<BaseUnityPlugin>)typeof(BepInEx.Bootstrap.Chainloader).GetField("_plugins", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);
        if(plugins.Exists(plugin => plugin.Info.Metadata.GUID == "com.willuwontu.rounds.tabinfo")){
            TabinfoInterface.Setup();
        }
        if(plugins.Exists(plugin => plugin.Info.Metadata.GUID == "ot.dan.rounds.gamesaver")){
            GameSaverPatch.Patch();
        }
        NullManager.instance.SetUp();
    }
}
}