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
}
