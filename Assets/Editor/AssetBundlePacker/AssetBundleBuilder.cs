﻿/**
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
    /// 资源依赖数据
    /// </summary>
    [Serializable]
    public class DependencyData
    {
        /// <summary>
        /// 资源依赖哈希值
        /// 资源的哈希值不变，且它的所有依赖哈希值也不变
        /// 代表该资源的全部依赖关系没变
        /// </summary>
        public Hash128 AssetDependencyHash;
        /// <summary>
        /// 所有依赖资源
        /// AssetDatabase.GetDependencies（所以会包含自己）
        /// </summary>
        public string[] Dependencies;
    }

    [Serializable]
    public class AllAssetsDependencies
    {
        /// <summary>
        /// AssetPath -> Dependencies
        /// </summary>
        public Dictionary<string, DependencyData> DependencyDataDict = new Dictionary<string, DependencyData>();
        /// <summary>
        /// 把文件夹名和文件名映射成它对应的位置索引
        /// 减少存储量
        /// </summary>
        public List<string> AllDirectoryAndFileNames = new List<string>();
    }

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
        /// 依赖缓存
        /// </summary>
        private AllAssetsDependencies _allAssetsDependencies;
        /// <summary>
        /// (AssetPath -> AssetBundleItem)
        /// </summary>
        private Dictionary<string, AssetBundleItem> _assetBundleItemDict = new Dictionary<string, AssetBundleItem>();
        /// <summary>
        /// 文件夹名称或文件名称 -> 索引
        /// </summary>
        private Dictionary<string, int> _allDirectoryAndFileNamesDict = new Dictionary<string, int>();
        
        /// <summary>
        /// 获取
        /// </summary>
        public void GetAllAssetsDependencies()
        {
            _allAssetsDependencies = ReadDependenciesCache();
            var fileList = Directory.GetFiles(AssetBundleConfig.PackRootPath, "*.*", SearchOption.AllDirectories);
            for (int i = 0; i < fileList.Length; i++)
            {
                var filePath = fileList[i];
                if (AssetBundleHelper.IsValidAssetPath(filePath))
                {
                    filePath = filePath.Replace("\\", "/");
                    var indexFilePath = GetIndexFilePath(filePath);
                    if (_allAssetsDependencies.DependencyDataDict.TryGetValue(indexFilePath, out var dependencyData))
                    {
                        if (dependencyData.Dependencies != null)
                        {
                            for (int j = 0; j < dependencyData.Dependencies.Length; j++)
                            {
                                var indexDependPath = dependencyData.Dependencies[j];
                                if (_allAssetsDependencies.DependencyDataDict.TryGetValue(indexDependPath, out var item))
                                {
                                    var dependPath = GetRealFilePath(indexDependPath);
                                    var hash = AssetDatabase.GetAssetDependencyHash(dependPath);
                                    if (item.AssetDependencyHash != hash)
                                    {
                                        RefreshDependencies(filePath);
                                        break;
                                    }
                                }
                                else
                                {
                                    RefreshDependencies(filePath);
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        RefreshDependencies(filePath);
                    }
                }
            }

            SaveDependenciesCache(_allAssetsDependencies);
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// 获取文件名称映射为索引之后的文件路径
        /// Assets/ArtPack/Pack/Test.png -> 1/2/3/4/5
        /// </summary>
        private string GetIndexFilePath(string filePath)
        {
            var result = "";
            var names = filePath.Split("/");
            for(int i = 0; i < names.Length; i++)
            {
                var name = names[i];
                if (_allDirectoryAndFileNamesDict.ContainsKey(name))
                {
                    result += "/" + _allDirectoryAndFileNamesDict[name];
                }
                else
                {
                    var index = -1;
                    for(int j = 0; j < _allAssetsDependencies.AllDirectoryAndFileNames.Count; j++)
                    {
                        if (_allAssetsDependencies.AllDirectoryAndFileNames[j] == name)
                        {
                            index = j;
                            break;
                        }
                    }
                    if (index == -1)
                    {
                        index = _allAssetsDependencies.AllDirectoryAndFileNames.Count;
                        _allAssetsDependencies.AllDirectoryAndFileNames.Add(name);
                    }
                    _allDirectoryAndFileNamesDict.Add(name, index);
                    result += "/" + index;
                }
            }
            return result.Substring(1);
        }

        /// <summary>
        /// 根据索引拼的路径获得真实的资源路径
        /// </summary>
        /// <param name="indexFilePath"></param>
        /// <returns></returns>
        private string GetRealFilePath(string indexFilePath)
        {
            var result = "";
            var indexs = indexFilePath.Split("/");
            for (int i = 0; i < indexs.Length; i++)
            {
                var index = int.Parse(indexs[i]);
                var name = _allAssetsDependencies.AllDirectoryAndFileNames[index];
                result += "/" + name;
            }
            return result.Substring(1);
        }

        /// <summary>
        /// 更新依赖
        /// </summary>
        private void RefreshDependencies(string path)
        {
            Debug.LogError($"变化：{path}");
            var dependencyData = new DependencyData();
            var dependencies = AssetDatabase.GetDependencies(path);
            dependencyData.Dependencies = new string[dependencies.Length];
            for (int i = 0; i < dependencies.Length; i++)
            {
                dependencyData.Dependencies[i] = GetIndexFilePath(dependencies[i]);
            }
            var indexFilePath = GetIndexFilePath(path);
            if (_allAssetsDependencies.DependencyDataDict.ContainsKey(indexFilePath))
            {
                _allAssetsDependencies.DependencyDataDict[indexFilePath] = dependencyData;
            }
            else
            {
                _allAssetsDependencies.DependencyDataDict.Add(indexFilePath, dependencyData);
            }
            for (int i = 0; i < dependencies.Length; i++)
            {
                var dependPath = dependencies[i];
                var indexDependFilePath = GetIndexFilePath(dependPath);
                if (!_allAssetsDependencies.DependencyDataDict.ContainsKey(indexDependFilePath))
                {
                    _allAssetsDependencies.DependencyDataDict.Add(indexDependFilePath, new DependencyData());
                }
                _allAssetsDependencies.DependencyDataDict[indexDependFilePath].AssetDependencyHash = AssetDatabase.GetAssetDependencyHash(dependPath);
            }
        }

        /// <summary>
        /// 获取依赖缓存
        /// </summary>
        private AllAssetsDependencies ReadDependenciesCache()
        {
            using (var fileStream = new FileStream(AssetBundleConfig.DependenciesCachePath, FileMode.OpenOrCreate))
            {
                if (fileStream.Length > 0)
                {
                    var binaryFormatter = new BinaryFormatter();
                    var obj = binaryFormatter.Deserialize(fileStream);
                    return obj as AllAssetsDependencies;
                }
            }
            return new AllAssetsDependencies();
        }

        /// <summary>
        /// 保存依赖缓存
        /// </summary>
        private void SaveDependenciesCache(AllAssetsDependencies cache)
        {
            using (var fileStream = new FileStream(AssetBundleConfig.DependenciesCachePath, FileMode.OpenOrCreate))
            {
                var binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(fileStream, cache);
            }
        }

        /// <summary>
        /// 根据路径获取对应的AssetBundleItem
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public AssetBundleItem GetAssetBundleItem(string path)
        {
            path = path.Replace("\\", "/");
            if (!AssetBundleHelper.IsValidAssetPath(path))
            {
                return null;
            }
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
        public void Start(string[] assetPaths, HashSet<string> forcePacks)
        {
            /**
             * 建立全资源依赖关系
             * 不递归
             */
            for (var i = 0; i < assetPaths.Length; i++)
            {
                //var item = GetAssetBundleItem(assetPaths[i]);
                //if (item != null)
                //{
                //    var dependencies = AssetDatabase.GetDependencies(item.AssetPath, false);
                //    for (int j = 0; j < dependencies.Length; j++)
                //    {
                //        var dependPath = dependencies[j];
                //        var dependItem = GetAssetBundleItem(dependPath);
                //        if (dependItem != null)
                //        {
                //            item.Dependencies.Add(dependItem.AssetPath);
                //            dependItem.BeDependencies.Add(item.AssetPath);
                //        }
                //    }
                //}
                var hash = AssetDatabase.GetAssetDependencyHash(assetPaths[i]);
                Debug.Log(hash);
            }

            /* 向上归并依赖
             *      a        e                 
             *       \      /                    
             *        b    f     ==>  (a,b,c,h) -> (d) <- (e,f)
             *      / | \ /                          
             *     c  h  d      
             */
            //foreach (var item in _assetBundleItemDict)
            //{
            //    var assetItem = item.Value;
            //    while (assetItem.BeDependencies.Count == 1)
            //    {
            //        assetItem = GetAssetBundleItem(assetItem.BeDependencies[0]);
            //        if (assetItem.BeDependencies.Count != 1)
            //        {
            //            item.Value.AssetBundleName = assetItem.AssetBundleName;
            //        }
            //    }
            //}
            //BuildAssetBundles();
        }

        /// <summary>
        /// 分组AssetBundles
        /// </summary>
        public void GroupingAssetBundles()
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
