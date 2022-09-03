﻿/**
 * 游戏启动项
 */

using UnityEngine;
using GameUnityFramework.Resource;
using GameMain.Views.UI;

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
            HotfixResourceManager.Init(GameConfig.HotfixResourceAddress, (EHotfixResourceStatus status) =>
            {
                switch(status)
                {
                    case EHotfixResourceStatus.StartHotfix:
                        Debug.LogError("开始热更新");
                        break;
                    case EHotfixResourceStatus.HotfixSuccess:
                        Debug.LogError("热更新成功");
                        break;
                }
            });
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