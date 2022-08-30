/**
 * AssetBundle构建类
 */

using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

namespace Editor.AssetBundlePacker
{
    /// <summary>
    /// 保存每个资源对应的AssetBundleName及其依赖关系
    /// </summary>
    public class AssetBundleItem
    {
        /// <summary>
        /// AssetBundle包名
        /// </summary>
        public string AssetBundleName = string.Empty;
        /// <summary>
        /// 该资源的所有依赖项
        /// 不递归
        /// </summary>
        public List<string> Dependencies = new List<string>();
        /// <summary>
        /// 依赖该资源的所有资源项
        /// 不递归
        /// </summary>
        public List<string> BeDependencies = new List<string>();
    }

    public class AssetBundleBuilder
    {
        /// <summary>
        /// (AssetPath -> AssetBundleItem)
        /// </summary>
        private Dictionary<string, AssetBundleItem> _assetBundleItemDict = new Dictionary<string, AssetBundleItem>();

        /// <summary>
        /// 开始构建AssetBundle
        /// 检测依赖
        /// </summary>
        public void Start(string[] assetPaths)
        {
            for (var i = 0; i < assetPaths.Length; i++)
            {
                var path = assetPaths[i].Replace("\\", "/");
                if (AssetBundleHelper.IsValidAssetPath(path))
                {
                    if (!_assetBundleItemDict.TryGetValue(path, out var item))
                    {
                        item = new AssetBundleItem();
                        item.AssetBundleName = AssetBundleHelper.GetAssetBundleName(path);
                        _assetBundleItemDict.Add(path, item);
                    }
                    var dependencies = AssetDatabase.GetDependencies(path, false);
                    for (int j = 0; j < dependencies.Length; j++)
                    {
                        var dependPath = dependencies[j];
                        if (AssetBundleHelper.IsValidAssetPath(dependPath))
                        {
                            item.Dependencies.Add(dependPath);
                            if (!_assetBundleItemDict.TryGetValue(dependPath, out var dependItem))
                            {
                                dependItem = new AssetBundleItem();
                                dependItem.AssetBundleName = AssetBundleHelper.GetAssetBundleName(dependPath);
                                _assetBundleItemDict.Add(dependPath, dependItem);
                            }
                            dependItem.BeDependencies.Add(path);
                        }
                    }
                }
            }

            BuildAssetBundles();
        }

        /// <summary>
        /// 分组AssetBundles
        /// </summary>
        public static void GroupingAssetBundles()
        {
            /* 清除同层依赖 把同层之间被依赖的节点下移 (a->b, a->c, b->c) ==> (a->b->c)
             *      a              a
             *     /  \    ==>    /
             *    b -> c         b
             *                  /
             *                 c 
             *  例如：prefab上挂着mat, mat依赖shder。特别注意，此时prefab同时依赖mat,和shader
             *  (prefab->mat, prefab->shader, mat->shader) ==> (prefab->mat->shader)
             */
            //var removeList = new List<string>();
            //foreach (var item in _assetItemDict)
            //{
            //    removeList.Clear();
            //    var path = item.Key;
            //    var assetItem = item.Value;
            //    foreach (var depend in assetItem.Depends)
            //    {
            //        var dependAssetItem = GetAssetBundleItem(depend);
            //        foreach (var beDepend in dependAssetItem.BeDepends)
            //        {
            //            if (assetItem.Depends.Contains(beDepend))
            //                removeList.Add(depend);
            //        }
            //    }
            //    foreach (var depend in removeList)
            //    {
            //        assetItem.Depends.Remove(depend);
            //        var dependAssetItem = GetAssetBundleItem(depend);
            //        dependAssetItem.BeDepends.Remove(path);
            //    }
            //}

            /* 向上归并依赖
             *      a        e                 
             *       \      /                    
             *        b    f     ==>  (a,b,c,h) -> (d) <- (e,f)
             *      / | \ /                          
             *     c  h  d      
             */
            //foreach (var item in _assetItemDict)
            //{
            //    var assetItem = item.Value;
            //    while (assetItem.BeDepends.Count == 1)
            //    {
            //        assetItem = GetAssetBundleItem(assetItem.BeDepends[0]);
            //        if (assetItem.BeDepends.Count != 1)
            //        {
            //            item.Value.Name = assetItem.Name;
            //        }
            //    }
            //}
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
