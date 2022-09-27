﻿/**
 * 游戏逻辑主入口
 */
using UnityEngine;
using GameUnityFramework.Resource;

namespace GameMain
{
    public class MainEntrance : MonoBehaviour
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
            ResourcePathHelper.ResourcePathPrefix = "Assets/ArtPack/Pack";
            ResourcePathHelper.AssetBundlesFolder = "assetbundles";
            ResourcePathHelper.PackConfigPath = "Assets/ArtPack/Pack/PackConfig.txt";
        }

        /// <summary>
        /// 初始化
        /// </summary>
        private void Start()
        {
            Debug.Log("main entrance ==> start game");
            G.UnityObjectManager.SyncGameObjectInstantiate("Prefabs/Base/MainCamera.prefab");
            G.UnityObjectManager.SyncGameObjectInstantiate("Tests/Test4.prefab");
        }

        /// <summary>
        /// Update总入口
        /// </summary>
        private void Update()
        {
            G.UIManager.Update();
            G.UnityObjectManager.Update();
        }
    }
}