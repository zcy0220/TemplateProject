/**
 * 游戏逻辑主入口
 */

using UnityEngine;
using GameBaseFramework.Base;
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
        Debuger.EnableLog = true;
        Debuger.Init(new UnityDebugConsole());
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
        GameGraphics.MainGraphic.Init();
        //逻辑层的输出即为渲染层的输入
        //渲染层的UI、键盘摇杆等输出，调用逻辑层的输入
        var output = GameGraphics.MainGraphic.InputManager;
        GameLogics.MainLogic.Bind(output);
        GameLogics.MainLogic.StartGame();
        var command = new GameLogics.LoginCommand();
        command.UserId = SystemInfo.deviceUniqueIdentifier;
        GameLogics.MainLogic.Input(command);
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
