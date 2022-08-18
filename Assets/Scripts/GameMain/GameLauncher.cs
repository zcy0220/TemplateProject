/**
 * 游戏启动项
 */

using UnityEngine;
using GameUnityFramework;
using GameBaseFramework.Base;
using GameBaseFramework.Event;
using GameUnityFramework.Utils;

namespace GameMain
{
    public class GameLauncher : MonoBehaviour
    {
        /// <summary>
        /// 固定帧的间隔时间（毫秒）
        /// </summary>
        private int _fixedDeltaTime;

        /// <summary>
        /// 初始化
        /// </summary>
        private void Awake()
        {
            _fixedDeltaTime = (int)(Time.fixedDeltaTime * 1000);
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
            //G.UnityObjectManager.AsyncGameObjectInstantiate("UI/Prefabs/UIMainRoot.prefab", (obj) => { });
        }

        /// <summary>
        /// Update总入口
        /// </summary>
        private void Update()
        {
            MonoBehaviourUtils.Instance.Update(Time.deltaTime);
        }

        /// <summary>
        /// FixedUpdate总入口
        /// </summary>
        private void FixedUpdate()
        {
            MonoBehaviourUtils.Instance.FixedUpdate(_fixedDeltaTime);
        }
    }
}