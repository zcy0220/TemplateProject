/**
 * 游戏启动项
 */

using UnityEngine;
using HybridCLR;
using System;
using GameUnityFramework.Resource;

public class GameLauncher
{
    /// <summary>
    /// 开始游戏
    /// </summary>
    public static void Start()
    {
#if !UNITY_EDITOR
        LoadMetadataForAOTAssembly();
#endif
        var dllBytes = AppLauncher.DllAssetBundle.LoadAsset<TextAsset>("GameMain.dll.bytes");
        var gameMainAssembly = System.Reflection.Assembly.Load(dllBytes.bytes);
        if (gameMainAssembly != null)
        {
            var mainEntranceType = gameMainAssembly.GetType("GameMain.MainEntrance");
            var mainEntrance = new GameObject("MainEntrance");
            mainEntrance.AddComponent(mainEntranceType);
            GameObject.DontDestroyOnLoad(mainEntrance);
        }
    }

    /// <summary>
    /// 为aot assembly加载原始metadata， 这个代码放aot或者热更新都行。
    /// 一旦加载后，如果AOT泛型函数对应native实现不存在，则自动替换为解释模式执行
    /// </summary>
    public static unsafe void LoadMetadataForAOTAssembly()
    {
        // 可以加载任意aot assembly的对应的dll。但要求dll必须与unity build过程中生成的裁剪后的dll一致，而不能直接使用原始dll。
        // 我们在BuildProcessor_xxx里添加了处理代码，这些裁剪后的dll在打包时自动被复制到 {项目目录}/HybridCLRData/AssembliesPostIl2CppStrip/{Target} 目录。

        /// 注意，补充元数据是给AOT dll补充元数据，而不是给热更新dll补充元数据。
        /// 热更新dll不缺元数据，不需要补充，如果调用LoadMetadataForAOTAssembly会返回错误
        /// 
        string[] aotDllList = Resources.Load<HotUpdateAssemblyManifest>("HotUpdateAssemblyManifest").AOTMetadataDlls;

        AssetBundle dllAB = AppLauncher.DllAssetBundle;
        foreach (var aotDllName in aotDllList)
        {
            Debug.Log($"{aotDllName}");
            byte[] dllBytes = dllAB.LoadAsset<TextAsset>(aotDllName).bytes;
            fixed (byte* ptr = dllBytes)
            {
                // 加载assembly对应的dll，会自动为它hook。一旦aot泛型函数的native函数不存在，用解释器版本代码
                LoadImageErrorCode err = (LoadImageErrorCode)RuntimeApi.LoadMetadataForAOTAssembly((IntPtr)ptr, dllBytes.Length);
                Debug.Log($"LoadMetadataForAOTAssembly:{aotDllName}. ret:{err}");
            }
        }
    }

    ///// <summary>
    ///// 固定帧的间隔时间（毫秒）
    ///// </summary>
    //private int _fixedDeltaTime;
    ///// <summary>
    ///// 游戏控制
    ///// </summary>
    //private bool _isRunning = false;

    ///// <summary>
    ///// 初始化
    ///// </summary>
    //private void Awake()
    //{
    //    _fixedDeltaTime = (int)(Time.fixedDeltaTime * 1000);
    //    ResourcePathHelper.ResourcePathPrefix = "Assets/ArtPack/Pack";
    //    ResourcePathHelper.AssetBundlesFolder = "assetbundles";
    //    ResourcePathHelper.PackConfigPath = "Assets/ArtPack/Pack/PackConfig.txt";
    //    DontDestroyOnLoad(gameObject);
    //}

    ///// <summary>
    ///// 开始游戏
    ///// </summary>
    //private void Start()
    //{
    //    //if (GameConfig.AssetBundleEncryptkey.Length > 0)
    //    //{
    //    //    AssetBundle.SetAssetBundleDecryptKey(GameConfig.AssetBundleEncryptkey);
    //    //}
    //    //if (GameConfig.IsOpenHotfixResource)
    //    //{
    //    //    HotfixResource();
    //    //}
    //    //else
    //    //{
    //    //    EnterGame();
    //    //}
    //}

    ///// <summary>
    ///// 热更新
    ///// </summary>
    //private void HotfixResource()
    //{
    //    //var obj = Resources.Load<GameObject>("HotfixResourceManager");
    //    //var hotfixResourceManager = GameObject.Instantiate(obj).GetComponent<HotfixResourceManager>();
    //    //hotfixResourceManager.OnStatusCallback = (status) =>
    //    //{
    //    //    switch (status)
    //    //    {
    //    //        case EHotfixResourceStatus.StartHotfix:
    //    //            break;
    //    //        case EHotfixResourceStatus.EnterGame:
    //    //            EnterGame();
    //    //            break;
    //    //        case EHotfixResourceStatus.InitServerVersionError:
    //    //            EnterGame();
    //    //            break;
    //    //    }
    //    //};
    //    //StartCoroutine(hotfixResourceManager.Startup(GameConfig.HotfixResourceAddress));
    //}

    ///// <summary>
    ///// 正式进入游戏
    ///// </summary>
    //private void EnterGame()
    //{
    //    _isRunning = true;
    //    Debuger.Log("进入游戏", "cyan");
    //    G.UnityObjectManager.SyncGameObjectInstantiate("Tests/Test4.prefab");
    //}

    ///// <summary>
    ///// Update总入口
    ///// </summary>
    //private void Update()
    //{
    //    if (_isRunning)
    //    {
    //        G.UIManager.Update();
    //        G.UnityObjectManager.Update();
    //    }
    //}

    ///// <summary>
    ///// FixedUpdate总入口
    ///// </summary>
    //private void FixedUpdate()
    //{
    //}
}
