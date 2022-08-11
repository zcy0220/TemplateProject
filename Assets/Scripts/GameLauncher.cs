/**
 * 游戏启动项
 */

using UnityEngine;
using GameUnityFramework;
using GameUnityFramework.Resource;

public class GameLauncher : MonoBehaviour
{
    /// <summary>
    /// 初始化配置模块
    /// </summary>
    private void Awake()
    {
        GameUnityFrameworkConfig.ResourcePathPrefix = "Assets/ArtPack/Pack";
        GameUnityFrameworkConfig.Init();
    }

    /// <summary>
    /// 开始游戏
    /// </summary>
    private void Start()
    {
        //var playerData = new PlayerData();
        //playerData.Id = 1;
        //playerData.TeamId = 1;
        //playerData.UserId = 1001;
        //playerData.Name = "测试名字A";
        //var param = new BattleParam();
        //param.Players.Add(playerData);

        //var battle = new Battle();
        //battle.Start(param);
        //MonoUpdaterManager.Instance.AddFixedUpdateListener(battle.UpdateFrame);
        UnityObjectManager.Instance.SyncGameObjectInstantiate("Assets/ArtPack/Pack/UI/Prefabs/UIMainRoot.prefab");
    }
}
