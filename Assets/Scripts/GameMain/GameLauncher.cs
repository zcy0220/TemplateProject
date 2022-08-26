/**
 * 游戏启动项
 */

using UnityEngine;
using GameUnityFramework.Resource;

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
            var hotfixResourceManager = new HotfixResourceManager();
            hotfixResourceManager.ServerAddress = GameConfig.HotfixResourceAddress;
            hotfixResourceManager.FinishedCallback = OnHotfixResourceFinished;
            StartCoroutine(hotfixResourceManager.Start());
        }

        /// <summary>
        /// 热更完成回调
        /// </summary>
        /// <param name="result"></param>
        private void OnHotfixResourceFinished(EHotfixResourceResult result)
        {
            if (result == EHotfixResourceResult.Success)
            {
                Debug.LogError("热更新成功");
            }
        }

        /// <summary>
        /// Update总入口
        /// </summary>
        private void Update()
        {
            G.UIManager.Update();
            G.UnityObjectManager.Update();
        }

        /// <summary>
        /// FixedUpdate总入口
        /// </summary>
        private void FixedUpdate()
        {
        }
    }
}