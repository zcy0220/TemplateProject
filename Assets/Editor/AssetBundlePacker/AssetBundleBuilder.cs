/**
 * AssetBundle构建类
 */

using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;

namespace Editor.AssetBundlePacker
{
    /// <summary>
    /// 保存每个资源对应的AssetBundleName及其依赖关系
    /// </summary>
    public class AssetBundleItem
    {
        /// <summary>
        /// Asset路径
        /// </summary>
        public string AssetPath = string.Empty;
        /// <summary>
        /// AssetBundle包名
        /// </summary>
        public string AssetBundleName = string.Empty;
        /// <summary>
        /// 该资源的所有依赖项
        /// 不递归
        /// </summary>
        public List<string> Dependencies;
        /// <summary>
        /// 依赖该资源的所有资源项
        /// 不递归
        /// </summary>
        public List<string> BeDependencies;
    }

    public class AssetBundleBuilder
    {
        /// <summary>
        /// (AssetPath -> AssetBundleItem)
        /// </summary>
        private Dictionary<string, AssetBundleItem> _assetBundleItemDict = new Dictionary<string, AssetBundleItem>();
        //===========================================================================================================
        public string[] AllAssetPaths { get; set; }
        public string[] AllForcePacks { get; set; }
        public Dictionary<string, DependencyData> AllAssetsDependencies { get; set; }

        /// <summary>
        /// 根据路径获取对应的AssetBundleItem
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public AssetBundleItem GetAssetBundleItem(string path)
        {
            path = path.Replace("\\", "/");
            if (!_assetBundleItemDict.TryGetValue(path, out var item))
            {
                item = new AssetBundleItem();
                item.AssetPath = path;
                item.AssetBundleName = AssetBundleHelper.GetAssetBundleName(path);
                _assetBundleItemDict.Add(path, item);
            }
            return item;
        }

        /// <summary>
        /// 开始构建AssetBundle
        /// 检测依赖
        /// </summary>
        public void Start()
        {
            CreateAssetDependsMap();
            //GroupingAssetBundles();
        }

        /// <summary>
        /// 遍历资源，建立依赖映射
        /// </summary>
        public void CreateAssetDependsMap()
        {
        }

        /// <summary>
        /// 分组AssetBundles
        /// </summary>
        public void GroupingAssetBundles()
        {
        }

        /// <summary>
        /// 最终构建AssetBundles
        /// </summary>
        public void BuildAssetBundles()
        {
            var assetBundleBuildDict = new Dictionary<string, List<string>>();
            var assetBundleBuildList = new List<AssetBundleBuild>();
            foreach(var item in _assetBundleItemDict)
            {
                if (!assetBundleBuildDict.ContainsKey(item.Value.AssetBundleName))
                {
                    assetBundleBuildDict.Add(item.Value.AssetBundleName, new List<string>());
                }
                assetBundleBuildDict[item.Value.AssetBundleName].Add(item.Key);
            }
            foreach (var item in assetBundleBuildDict)
            {
                var build = new AssetBundleBuild();
                build.assetBundleName = item.Key;
                build.assetNames = item.Value.ToArray();
                assetBundleBuildList.Add(build);
            }
            var outputPath = AssetBundleConfig.AssetBundleExportPath;
            var options = BuildAssetBundleOptions.ChunkBasedCompression;
            var targetPlatform = EditorUserBuildSettings.activeBuildTarget;
            FileUtil.DeleteFileOrDirectory(outputPath);
            Directory.CreateDirectory(outputPath);
            var assetBundleManifest = BuildPipeline.BuildAssetBundles(outputPath, assetBundleBuildList.ToArray(), options, targetPlatform);
            if (assetBundleManifest == null)
            {
                Debug.LogError("BuildPipeline.BuildAssetBundles Failed!");
            }
            else
            {
                Debug.Log($"BuildPipeline.BuildAssetBundles Success!");
            }
            AssetDatabase.Refresh();
        }
    }
}
