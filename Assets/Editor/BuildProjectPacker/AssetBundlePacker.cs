/**
 * AssetBundle打包工具
 */

using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

namespace Editor.BuildProjectPacker
{
    public class AssetBundlePacker
    {
        /// <summary>
        /// 构建脚本相关AssetBundle
        /// </summary>
        public static void BuildScriptAssetBundles(BuildTarget target)
        {
            var stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
            var builder = new AssetBundleBuilder();
            builder.BuildScriptAssetBundles(target);
            stopwatch.Stop();
            var timespan = stopwatch.Elapsed;
            Debug.Log($"<color=cyan>构建依赖耗时：{timespan.TotalMilliseconds}ms</color>");
        }

        /// <summary>
        /// 需要打包的资源列表
        /// </summary>
        /// <returns></returns>
        private static List<string> GetPackAssetList()
        {
            var packAssetList = new List<string>();
            var fileList = Directory.GetFiles(BuildProjectConfig.PackRootPath, "*.*", SearchOption.AllDirectories);
            for (int i = 0; i < fileList.Length; i++)
            {
                if (AssetBundleHelper.IsValidAssetPath(fileList[i]))
                {
                    packAssetList.Add(fileList[i].Replace("\\", "/"));
                }
            }
            return packAssetList;
        }

        /// <summary>
        /// 强制合包列表
        /// </summary>
        /// <returns></returns>
        private static List<string> GetAllForcePacks()
        {
            var allForcePacks = new List<string>()
            {
                //"Assets/ArtPack/Pack"
            };
            return allForcePacks;
        }

        /// <summary>
        /// 构建资源相关AssetBundle
        /// </summary>
        /// <param name="target"></param>
        public static void BuildResourceAssetBundles(BuildTarget target)
        {
            var stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
            var dependenciesbuilder = new DependenciesBuilder();
            var allAssetsDependencies = dependenciesbuilder.GetAllAssetsDependencies(BuildProjectConfig.PackRootPath);
            var assetBundleBuilder = new AssetBundleBuilder();
            assetBundleBuilder.AllAssetPaths = GetPackAssetList().ToArray();
            assetBundleBuilder.AllForcePacks = GetAllForcePacks().ToArray();
            assetBundleBuilder.AllAssetsDependencies = allAssetsDependencies;
            assetBundleBuilder.BuildResourceAssetBundles(target);
            stopwatch.Stop();
            var timespan = stopwatch.Elapsed;
            Debug.Log($"<color=cyan>构建耗时：{timespan.TotalMilliseconds}ms</color>");
        }

        /// <summary>
        /// 构建所有AssetBundles
        /// </summary>
        /// <param name="target"></param>
        public static void BuildAllAssetBundles(BuildTarget target)
        {
            var stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
            var dependenciesbuilder = new DependenciesBuilder();
            var allAssetsDependencies = dependenciesbuilder.GetAllAssetsDependencies(BuildProjectConfig.PackRootPath);
            var assetBundleBuilder = new AssetBundleBuilder();
            assetBundleBuilder.AllAssetPaths = GetPackAssetList().ToArray();
            assetBundleBuilder.AllForcePacks = GetAllForcePacks().ToArray();
            assetBundleBuilder.AllAssetsDependencies = allAssetsDependencies;
            assetBundleBuilder.BuildAllAssetBundles(target);
            stopwatch.Stop();
            var timespan = stopwatch.Elapsed;
            Debug.Log($"<color=cyan>构建耗时：{timespan.TotalMilliseconds}ms</color>");
        }
    }
}
