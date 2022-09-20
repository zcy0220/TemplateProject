/**
 * AssetBundle打包工具的配置文件
 */

using System.IO;
using UnityEngine;

namespace Editor.AssetBundlePacker
{
    public class AssetBundleConfig
    {
        /// <summary>
        /// 需要打包的资源根目录
        /// </summary>
        public static readonly string PackRootPath = "Assets/ArtPack/Pack";
        /// <summary>
        /// 资源路径和AssetBundle映射表
        /// </summary>
        public static readonly string PackConfigPath = PackRootPath + "/PackConfig.txt";
        /// <summary>
        /// 项目路径
        /// </summary>
        public static readonly string ProjectPath = Application.dataPath.Substring(0, Application.dataPath.Length - 6);
        /// <summary>
        /// 导出的AssetBundles等信息的路径
        /// 打APK时再考到Unity的StreamingAssets路径下
        /// </summary>
        public static readonly string AssetBundlesStreamingAssetsPath = ProjectPath + "Builds/AssetBundles/StreamingAssets";
        /// <summary>
        /// 版本配置信息导出路径
        /// </summary>
        public static readonly string VersionConfigExportPath = AssetBundlesStreamingAssetsPath + "/VersionConfig.json";
        /// <summary>
        /// AssetBundleFolder
        /// </summary>
        public static readonly string AssetBundleFolder = "assetbundles";
        /// <summary>
        /// AssetBundle导出目录
        /// </summary>
        public static readonly string AssetBundleExportPath = AssetBundlesStreamingAssetsPath + "/" + AssetBundleFolder;
    }
}
