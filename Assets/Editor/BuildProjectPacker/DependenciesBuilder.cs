/**
 * 全资源依赖构建
 */

using UnityEditor;
using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;

namespace Editor.BuildProjectPacker
{
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
        /// 直接依赖的资源（不递归）
        /// </summary>
        public string[] Dependencies;
        /// <summary>
        /// 所有依赖的资源（递归）
        /// </summary>
        public string[] AllDependencies;
        /// <summary>
        /// 被依赖的资源
        /// </summary>
        public List<string> BeDependencies = new List<string>();
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

    public class DependenciesBuilder
    {
        private const string HASH_NONE = "00000000000000000000000000000000";
        /// <summary>
        /// 依赖缓存
        /// </summary>
        private AllAssetsDependencies _allAssetsDependencies;
        /// <summary>
        /// 文件夹名称或文件名称 -> 索引
        /// </summary>
        private Dictionary<string, int> _allDirectoryAndFileNamesDict = new Dictionary<string, int>();
        /// <summary>
        /// 依赖缓存存储地址
        /// </summary>
        private static string _dependenciesCachePath = Path.Combine(Directory.GetParent(Application.dataPath).ToString(), "Builds/AssetBundleCaches/DependenciesCache.txt");
        
        /// <summary>
        /// 获取
        /// </summary>
        public Dictionary<string, DependencyData> GetAllAssetsDependencies(string rootPath)
        {
            _allAssetsDependencies = ReadDependenciesCache();
            var fileList = Directory.GetFiles(rootPath, "*.*", SearchOption.AllDirectories);
            var needRefreshDependenciesList = new List<string>();
            for (int i = 0; i < fileList.Length; i++)
            {
                var filePath = fileList[i];
                if (Path.GetExtension(filePath).ToLower() != ".meta")
                {
                    filePath = filePath.Replace("\\", "/");
                    var indexFilePath = GetIndexFilePath(filePath);
                    if (_allAssetsDependencies.DependencyDataDict.TryGetValue(indexFilePath, out var dependencyData) && dependencyData.AllDependencies != null)
                    {
                        for (int j = 0; j < dependencyData.AllDependencies.Length; j++)
                        {
                            var indexDependPath = dependencyData.AllDependencies[j];
                            if (_allAssetsDependencies.DependencyDataDict.TryGetValue(indexDependPath, out var item))
                            {
                                var dependPath = GetRealFilePath(indexDependPath);
                                var hash = AssetDatabase.GetAssetDependencyHash(dependPath);
                                if (item.AssetDependencyHash != hash)
                                {
                                    needRefreshDependenciesList.Add(filePath);
                                    break;
                                }
                            }
                            else
                            {
                                needRefreshDependenciesList.Add(filePath);
                                break;
                            }
                        }
                    }
                    else
                    {
                        needRefreshDependenciesList.Add(filePath);
                    }
                }
            }
            needRefreshDependenciesList.ForEach((filePath) => RefreshDependencies(filePath));
            ClearNoneAssets();
            SaveDependenciesCache(_allAssetsDependencies);
            
            var dependencies = new Dictionary<string, DependencyData>();
            foreach (var item in _allAssetsDependencies.DependencyDataDict)
            {
                if (item.Value.AllDependencies != null)
                {
                    for (int i = 0; i < item.Value.AllDependencies.Length; i++)
                    {
                        item.Value.AllDependencies[i] = GetRealFilePath(item.Value.AllDependencies[i]);
                    }
                }
                if (item.Value.Dependencies != null)
                {
                    for (int i = 0; i < item.Value.Dependencies.Length; i++)
                    {
                        item.Value.Dependencies[i] = GetRealFilePath(item.Value.Dependencies[i]);
                    }
                }
                if (item.Value.BeDependencies != null)
                {
                    for (int i = 0; i < item.Value.BeDependencies.Count; i++)
                    {
                        item.Value.BeDependencies[i] = GetRealFilePath(item.Value.BeDependencies[i]);
                    }
                }
                dependencies.Add(GetRealFilePath(item.Key), item.Value);
            }
            Debug.Log($"<color=cyan>总资源数：{dependencies.Count}</color>");
            return dependencies;
        }

        /// <summary>
        /// 清空已经删除的资源
        /// </summary>
        private void ClearNoneAssets()
        {
            var removeList = new List<string>();
            foreach (var item in _allAssetsDependencies.DependencyDataDict)
            {
                var filePath = GetRealFilePath(item.Key);
                if (AssetDatabase.GetAssetDependencyHash(filePath).ToString() == HASH_NONE)
                {
                    removeList.Add(item.Key);
                }

                if (item.Value.AllDependencies != null)
                {
                    var list = new List<string>();
                    for (int i = 0; i < item.Value.AllDependencies.Length; i++)
                    {
                        var dependPath = GetRealFilePath(item.Value.AllDependencies[i]);
                        if (AssetDatabase.GetAssetDependencyHash(dependPath).ToString() != HASH_NONE)
                        {
                            list.Add(item.Value.AllDependencies[i]);
                        }
                    }
                    item.Value.AllDependencies = list.ToArray();
                }
                if (item.Value.Dependencies != null)
                {
                    var list = new List<string>();
                    for (int i = 0; i < item.Value.Dependencies.Length; i++)
                    {
                        var dependPath = GetRealFilePath(item.Value.Dependencies[i]);
                        if (AssetDatabase.GetAssetDependencyHash(dependPath).ToString() != HASH_NONE)
                        {
                            list.Add(item.Value.Dependencies[i]);
                        }
                    }
                    item.Value.Dependencies = list.ToArray();
                }
                if (item.Value.BeDependencies != null)
                {
                    var list = new List<string>();
                    for (int i = 0; i < item.Value.BeDependencies.Count; i++)
                    {
                        var dependPath = GetRealFilePath(item.Value.BeDependencies[i]);
                        if (AssetDatabase.GetAssetDependencyHash(dependPath).ToString() != HASH_NONE)
                        {
                            list.Add(item.Value.BeDependencies[i]);
                        }
                    }
                    item.Value.BeDependencies = list;
                }
            }
            for (int i = 0; i < removeList.Count; i++)
            {
                _allAssetsDependencies.DependencyDataDict.Remove(removeList[i]);
            }
        }

        /// <summary>
        /// 获取文件名称映射为索引之后的文件路径
        /// Assets/ArtPack/Pack/Test.png -> 1/2/3/4/5
        /// </summary>
        private string GetIndexFilePath(string filePath)
        {
            //return filePath;
            var result = "";
            var names = filePath.Split("/");
            for (int i = 0; i < names.Length; i++)
            {
                var name = names[i];
                if (_allDirectoryAndFileNamesDict.ContainsKey(name))
                {
                    result += "/" + _allDirectoryAndFileNamesDict[name];
                }
                else
                {
                    var index = -1;
                    for (int j = 0; j < _allAssetsDependencies.AllDirectoryAndFileNames.Count; j++)
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
            //return indexFilePath;
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
            Debug.Log($"变化：{path}");
            var allDependenciesList = new HashSet<string>();
            var stack = new Stack<string>();
            stack.Push(path);
            while (stack.Count > 0)
            {
                var filePath = stack.Pop();
                allDependenciesList.Add(filePath);
                var dependencies = AssetDatabase.GetDependencies(filePath, false);
                var indexFilePath = GetIndexFilePath(filePath);
                if (!_allAssetsDependencies.DependencyDataDict.TryGetValue(indexFilePath, out var dependencyData))
                {
                    dependencyData = new DependencyData();
                    _allAssetsDependencies.DependencyDataDict.Add(indexFilePath, dependencyData);
                }

                //新老依赖对比、去除依赖关系解除的
                if (dependencyData.Dependencies != null)
                {
                    for (int i = 0; i < dependencyData.Dependencies.Length; i++)
                    {
                        var dependPath = dependencyData.Dependencies[i];
                        if (_allAssetsDependencies.DependencyDataDict.TryGetValue(dependPath, out var beDdependencyData))
                        {
                            beDdependencyData.BeDependencies.Remove(indexFilePath);
                        }
                    }
                }

                dependencyData.AssetDependencyHash = AssetDatabase.GetAssetDependencyHash(filePath);
                dependencyData.Dependencies = new string[dependencies.Length];
                for (int i = 0; i < dependencies.Length; i++)
                {
                    dependencyData.Dependencies[i] = GetIndexFilePath(dependencies[i]);
                }
                for (int i = 0; i < dependencies.Length; i++)
                {
                    var dependPath = dependencies[i];
                    //已经检查过的资源（存在极少数情况2个资源互相依赖）
                    if (!allDependenciesList.Contains(dependPath))
                    {
                        allDependenciesList.Add(dependPath);
                        stack.Push(dependPath);
                    }
                    var indexDependPath = GetIndexFilePath(dependPath);
                    if (!_allAssetsDependencies.DependencyDataDict.TryGetValue(indexDependPath, out var beDdependencyData))
                    {
                        beDdependencyData = new DependencyData();
                        _allAssetsDependencies.DependencyDataDict.Add(indexDependPath, beDdependencyData);
                    }
                    beDdependencyData.AssetDependencyHash = AssetDatabase.GetAssetDependencyHash(dependPath);
                    if (!beDdependencyData.BeDependencies.Contains(indexFilePath))
                    {
                        beDdependencyData.BeDependencies.Add(indexFilePath);
                    }
                }
            }

            var indexRootFilePath = GetIndexFilePath(path);
            if (_allAssetsDependencies.DependencyDataDict.TryGetValue(indexRootFilePath, out var rootDependencyData))
            {
                rootDependencyData.AllDependencies = new string[allDependenciesList.Count];
                var index = 0;
                foreach (var item in allDependenciesList)
                {
                    rootDependencyData.AllDependencies[index] = GetIndexFilePath(item);
                    index++;
                }
            }
        }

        /// <summary>
        /// 获取依赖缓存
        /// </summary>
        private AllAssetsDependencies ReadDependenciesCache()
        {
            var dirPath = Directory.GetParent(_dependenciesCachePath).ToString();
            if (Directory.Exists(dirPath))
            {
                using (var fileStream = new FileStream(_dependenciesCachePath, FileMode.OpenOrCreate))
                {
                    if (fileStream.Length > 0)
                    {
                        var binaryFormatter = new BinaryFormatter();
                        var obj = binaryFormatter.Deserialize(fileStream);
                        return obj as AllAssetsDependencies;
                    }
                }
            }
            return new AllAssetsDependencies();
        }

        /// <summary>
        /// 保存依赖缓存
        /// </summary>
        private void SaveDependenciesCache(AllAssetsDependencies cache)
        {
            var dirPath = Directory.GetParent(_dependenciesCachePath).ToString();
            Directory.CreateDirectory(dirPath);
            using (var fileStream = new FileStream(_dependenciesCachePath, FileMode.OpenOrCreate))
            {
                var binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(fileStream, cache);
            }
        }
    }
}
