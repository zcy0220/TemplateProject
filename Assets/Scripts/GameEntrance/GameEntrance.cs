﻿/**
 * 游戏逻辑主入口
 */

using UnityEngine;
using GameUnityFramework.Log;
using GameUnityFramework.Resource;

public class GameEntrance : MonoBehaviour
{
    /// <summary>
    /// 初始化
    /// </summary>
    private void Awake()
    {
        ConfigGameLogics();
        ConfigGameGraphics();
        GameObject.DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// 配置逻辑相关
    /// </summary>
    private void ConfigGameLogics()
    {
        GameLogics.Debuger.EnableLog = true;
        GameLogics.Debuger.LogCallback =  Debuger.Log;
    }

    /// <summary>
    /// 配置渲染相关
    /// </summary>
    private void ConfigGameGraphics()
    {
        ResourcePathHelper.ResourcePathPrefix = "Assets/ArtPack/Pack";
        ResourcePathHelper.AssetBundlesFolder = "assetbundles";
        ResourcePathHelper.PackConfigPath = "Assets/ArtPack/Pack/PackConfig.txt";
    }

    /// <summary>
    /// 开始游戏
    /// </summary>
    private void Start()
    {
        Debuger.Log("game entrance ==> start");
        //逻辑层的输出即为渲染层的输入
        //渲染层的UI、键盘摇杆等输出，调用逻辑层的输入
        GameLogics.MainLogic.BindGraphics(GameGraphics.MainGraphic.InputManager);
        GameLogics.MainLogic.StartGame();
    }

    /// <summary>
    /// 渲染驱动
    /// </summary>
    private void Update()
    {
        GameGraphics.MainGraphic.Update();
    }

    /// <summary>
    /// 逻辑驱动
    /// </summary>
    private void FixedUpdate()
    {
        GameLogics.MainLogic.Update();
    }
}
