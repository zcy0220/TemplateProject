/**
 * AssetBundle打包工具
 */

using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

namespace Editor.AssetBundlePacker
{
    public class AssetBundlePacker
    {
        /// <summary>
        /// 构建测试AssetBundle
        /// </summary>
        [MenuItem("Tools/AssetBundlePacker/BuildAssetBundles")]
        public static bool Build()
        {
            var stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();

            //需要打包的资源列表
            var packAssetList = new List<string>();
            var fileList = Directory.GetFiles(AssetBundleConfig.PackRootPath, "*.*", SearchOption.AllDirectories);
            for (int i = 0; i < fileList.Length; i++)
            {
                if (AssetBundleHelper.IsValidAssetPath(fileList[i]))
                {
                    packAssetList.Add(fileList[i].Replace("\\", "/"));
                }
            }

            var allForcePacks = new string[]
            {
                //"Assets/ArtPack/Pack",
            };

            var dependenciesbuilder = new DependenciesBuilder();
            var allAssetsDependencies = dependenciesbuilder.GetAllAssetsDependencies(AssetBundleConfig.PackRootPath);
            var assetBundleBuilder = new AssetBundleBuilder();
            assetBundleBuilder.AllAssetPaths = packAssetList.ToArray();
            assetBundleBuilder.AllForcePacks = allForcePacks;
            assetBundleBuilder.AllAssetsDependencies = allAssetsDependencies;
            assetBundleBuilder.Start();
            stopwatch.Stop();
            var timespan = stopwatch.Elapsed;
            Debug.Log($"<color=cyan>构建耗时：{timespan.TotalMilliseconds}ms</color>");
            return true;
        }

        /// <summary>
        /// 构建所有资源依赖
        /// </summary>
        [MenuItem("Tools/AssetBundlePacker/BuildAllDependencies")]
        public static void Dependencies()
        {
            var stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
            var builder = new DependenciesBuilder();
            var allAssetsDependencies = builder.GetAllAssetsDependencies(AssetBundleConfig.PackRootPath);
            stopwatch.Stop();
            var timespan = stopwatch.Elapsed;
            Debug.Log($"<color=cyan>总资源数：{allAssetsDependencies.Count}</color>");
            Debug.Log($"<color=cyan>构建依赖耗时：{timespan.TotalMilliseconds}ms</color>");
        }
    }
}
