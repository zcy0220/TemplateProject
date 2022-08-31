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
        /// AssetBundle导出目录
        /// </summary>
        public static readonly string AssetBundleExportPath = Path.Combine(Application.streamingAssetsPath, "assetbundles");
        /// <summary>
        /// 资源依赖数据缓存路径
        /// </summary>
        public static readonly string DependenciesCachePath = Path.Combine(Application.dataPath, "Editor/AssetBundlePacker/DependenciesCache.txt");
    }
}
