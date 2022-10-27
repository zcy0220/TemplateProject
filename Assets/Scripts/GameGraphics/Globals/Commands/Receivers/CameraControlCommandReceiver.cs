/**
 * 打开UI指令接收器
 */

using System;
using UnityEngine;
using GameUnityFramework.Log;
using GameBaseFramework.Patterns;
using System.Collections.Generic;

namespace GameGraphics
{
    internal class CameraControlCommandReceiver : CommandReceiver
    {
        /// <summary>
        /// 相机根节点
        /// </summary>
        private Transform _mainCameraRoot;
        /// <summary>
        /// 主相机组件
        /// </summary>
        private Camera _mainCamera;

        /// <summary>
        /// 构造初始化
        /// </summary>
        public CameraControlCommandReceiver()
        {
            var _mainCameraRoot = GameObject.Find("MainCameraRoot");
            if (_mainCameraRoot == null)
            {
                _mainCameraRoot = MainGraphic.UnityObjectManager.SyncGameObjectInstantiate("Prefabs/Cameras/MainCameraRoot.prefab");
                GameObject.DontDestroyOnLoad(_mainCameraRoot);
            }
            _mainCamera = _mainCameraRoot.GetComponent<Camera>();
        }

        /// <summary>
        /// 更新
        /// </summary>
        public void Update()
        {
        }
    }
}
