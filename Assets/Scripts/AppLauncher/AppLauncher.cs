/**
 * App启动项 
 */

using System;
using UnityEngine;

public class AppLauncher : MonoBehaviour
{
    /// <summary>
    /// 初始化
    /// </summary>
    private void Awake()
    {
        if (AppConfig.AssetBundleEncryptkey.Length > 0)
        {
            AssetBundle.SetAssetBundleDecryptKey(AppConfig.AssetBundleEncryptkey);
        }
    }

    /// <summary>
    /// 热更新
    /// </summary>
    private void Start()
    {
        var prefab = Resources.Load<GameObject>("HotfixResourceManager");
        var hotfixResourceManager = GameObject.Instantiate(prefab).GetComponent<HotfixResourceManager>();
        hotfixResourceManager.OnEnterGame = EnterGame;
    }

    /// <summary>
    /// 进入启动游戏
    /// </summary>
    private void EnterGame(AssetBundle dllAssetBundle)
    {
        Debug.Log("app launcher ==> enter game");
        var gameLogicsDll = dllAssetBundle.LoadAsset<TextAsset>("GameLogics.dll.bytes");
        System.Reflection.Assembly.Load(gameLogicsDll.bytes);
        var gameGraphicsDll = dllAssetBundle.LoadAsset<TextAsset>("GameGraphics.dll.bytes");
        System.Reflection.Assembly.Load(gameGraphicsDll.bytes);
        var gameEntranceDll = dllAssetBundle.LoadAsset<TextAsset>("GameEntrance.dll.bytes");
        var gameEntranceAssembly = System.Reflection.Assembly.Load(gameEntranceDll.bytes);
        var assemblyCSharpDll = dllAssetBundle.LoadAsset<TextAsset>("Assembly-CSharp.dll.bytes");
        System.Reflection.Assembly.Load(assemblyCSharpDll.bytes);
#if !UNITY_EDITOR
        LoadMetadataForAOTAssembly(dllAssetBundle);
#endif
        if (gameEntranceAssembly != null)
        {
            var gameEntranceType = gameEntranceAssembly.GetType("GameEntrance");
            var gameEntrance = new GameObject("GameEntrance");
            gameEntrance.AddComponent(gameEntranceType);
        }
    }

    /// <summary>
    /// 为aot assembly加载原始metadata， 这个代码放aot或者热更新都行。
    /// 一旦加载后，如果AOT泛型函数对应native实现不存在，则自动替换为解释模式执行
    /// </summary>
    public static unsafe void LoadMetadataForAOTAssembly(AssetBundle dllAssetBundle)
    {
        // 可以加载任意aot assembly的对应的dll。但要求dll必须与unity build过程中生成的裁剪后的dll一致，而不能直接使用原始dll。
        // 我们在BuildProcessor_xxx里添加了处理代码，这些裁剪后的dll在打包时自动被复制到 {项目目录}/HybridCLRData/AssembliesPostIl2CppStrip/{Target} 目录。

        /// 注意，补充元数据是给AOT dll补充元数据，而不是给热更新dll补充元数据。
        /// 热更新dll不缺元数据，不需要补充，如果调用LoadMetadataForAOTAssembly会返回错误
        /// 
        string[] aotDllList = Resources.Load<HotUpdateAssemblyManifest>("HotUpdateAssemblyManifest").AOTMetadataDlls;

        AssetBundle dllAB = dllAssetBundle;
        foreach (var aotDllName in aotDllList)
        {
            Debug.Log($"{aotDllName}");
            byte[] dllBytes = dllAB.LoadAsset<TextAsset>(aotDllName).bytes;
            fixed (byte* ptr = dllBytes)
            {
                // 加载assembly对应的dll，会自动为它hook。一旦aot泛型函数的native函数不存在，用解释器版本代码
                HybridCLR.LoadImageErrorCode err = (HybridCLR.LoadImageErrorCode)HybridCLR.RuntimeApi.LoadMetadataForAOTAssembly((IntPtr)ptr, dllBytes.Length);
                Debug.Log($"LoadMetadataForAOTAssembly:{aotDllName}. ret:{err}");
            }
        }
    }
}
