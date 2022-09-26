/**
 * 打包配置文件
 */

using System.IO;
using UnityEngine;

namespace Editor.BuildProjectPacker
{
    public class BuildProjectConfig
    {
        /// <summary>
        /// 项目路径
        /// </summary>
        public static readonly string ProjectPath = Directory.GetParent(Application.dataPath).ToString();
        /// <summary>
        /// 编译导出路径
        /// </summary>
        public static readonly string ProjectBuildPath = Path.Combine(ProjectPath, "Builds");
        /// <summary>
        /// Builds/StreamingAssets
        /// PlatformConfig.txt、Assetbundles
        /// </summary>
        public static readonly string ProjectBuildStreamingAssetsPath = Path.Combine(ProjectPath, "Builds/StreamingAssets");
        /// <summary>
        /// HybridCLRBuildCache
        /// </summary>
        public static readonly string HybridCLRBuildCachePath = Path.Combine(ProjectBuildPath, "AssetBundleCaches/HybridCLRBuildCache");
        /// <summary>
        /// 脚本AssetBundle临时目录
        /// </summary>
        public static readonly string ScriptAssetBundleTempPath = $"{Application.dataPath}/DllPack";
        /// <summary>
        /// AssetBundleFolder
        /// </summary>
        public static readonly string AssetBundleFolder = "assetbundles";
        /// <summary>
        /// AssetBundle导出目录
        /// </summary>
        public static readonly string AssetBundleOutputPath = $"{ProjectBuildStreamingAssetsPath}/{AssetBundleFolder}";
        /// <summary>
        /// AOT补充元数据dll列表
        /// </summary>
        public static readonly HotUpdateAssemblyManifest HotUpdateAssemblyManifest = Resources.Load<HotUpdateAssemblyManifest>("HotUpdateAssemblyManifest");
        /// <summary>
        /// 版本配置信息导出路径
        /// </summary>
        public static readonly string AssetBundleVersionConfigFile = "AssetBundleVersionConfig.json";
        /// <summary>
        /// 需要打包的资源根目录
        /// </summary>
        public static readonly string PackRootPath = "Assets/ArtPack/Pack";
        /// <summary>
        /// 资源路径和AssetBundle映射表
        /// </summary>
        public static readonly string PackConfigPath = PackRootPath + "/PackConfig.txt";
        /// <summary>
        /// 需要强制打成一个包的配置文件
        /// </summary>
        public static string ForcePackAssetConfig = Path.Combine(ProjectBuildPath, "AssetBundleCaches/ForcePackAssetConfig.json");
    }
}
