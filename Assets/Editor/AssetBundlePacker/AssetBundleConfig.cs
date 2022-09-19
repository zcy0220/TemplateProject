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
        /// 版本配置信息导出路径
        /// </summary>
        public static readonly string VersionConfigExportPath = ProjectPath + "Builds/AssetBundles/StreamingAssets/VersionConfig.json";
        /// <summary>
        /// AssetBundle信息列表导出路径
        /// </summary>
        public static readonly string AssetBundleInfoListExportPath = ProjectPath + "Builds/AssetBundles/StreamingAssets/AssetBundleInfoList.json";
        /// <summary>
        /// AssetBundleFolder
        /// </summary>
        public static readonly string AssetBundleFolder = "assetbundles";
        /// <summary>
        /// AssetBundle导出目录
        /// </summary>
        public static readonly string AssetBundleExportPath = ProjectPath + "Builds/AssetBundles/StreamingAssets/" + AssetBundleFolder;
    }
}
