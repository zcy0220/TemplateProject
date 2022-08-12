/**
 * 游戏启动项
 */

using UnityEngine;
using GameUnityFramework;
using GameBaseFramework.Base;
using GameBaseFramework.Event;
using GameUnityFramework.Resource;

namespace GameMain
{
    public class GameLauncher : MonoBehaviour
    {
        /// <summary>
        /// 初始化
        /// </summary>
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
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
            G.UnityObjectManager.AsyncGameObjectInstantiate("UI/Prefabs/UIMainRoot.prefab", (obj) => { });
        }
    }
}