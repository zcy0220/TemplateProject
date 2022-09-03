﻿/**
 * 包体配置文件构建
 */

using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;

namespace Editor.AssetBundlePacker
{
    [Serializable]
    public class PackConfig
    {
        /// <summary>
        /// AssetPath -> AssetBundleName
        /// </summary>
        public Dictionary<string, string> DependencyDataDict = new Dictionary<string, string>();
        /// <summary>
        /// 把文件夹名和文件名映射成它对应的位置索引
        /// 减少存储量
        /// </summary>
        public List<string> AllDirectoryAndFileNames = new List<string>();
    }

    public class PackConfigBuilder
    {
        private PackConfig _packConfig = new PackConfig();
        /// <summary>
        /// 文件夹名称或文件名称 -> 索引
        /// </summary>
        private Dictionary<string, int> _allDirectoryAndFileNamesDict = new Dictionary<string, int>();

        /// <summary>
        /// 构建
        /// </summary>
        public void Build(Dictionary<string, AssetBundleItem> assetBundleItemDict)
        {
            foreach(var item in assetBundleItemDict)
            {
                if (item.Key.StartsWith(AssetBundleConfig.PackRootPath))
                {
                    var assetPath = GetIndexFilePath(item.Key);
                    var assetBundleName = AssetBundleHelper.GetAssetBundleName(item.Value.AssetBundleName);
                    _packConfig.DependencyDataDict.Add(assetPath, assetBundleName);
                }
            }
            SavePackConfig(_packConfig);
        }

        /// <summary>
        /// 保存依赖缓存
        /// </summary>
        private void SavePackConfig(PackConfig config)
        {
            using (var fileStream = new FileStream(AssetBundleConfig.PackConfigPath, FileMode.OpenOrCreate))
            {
                var binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(fileStream, config);
            }
        }

        /// <summary>
        /// 获取文件名称映射为索引之后的文件路径
        /// Assets/ArtPack/Pack/Test.png -> 1/2/3/4/5
        /// </summary>
        private string GetIndexFilePath(string filePath)
        {
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
                    for (int j = 0; j < _packConfig.AllDirectoryAndFileNames.Count; j++)
                    {
                        if (_packConfig.AllDirectoryAndFileNames[j] == name)
                        {
                            index = j;
                            break;
                        }
                    }
                    if (index == -1)
                    {
                        index = _packConfig.AllDirectoryAndFileNames.Count;
                        _packConfig.AllDirectoryAndFileNames.Add(name);
                    }
                    _allDirectoryAndFileNamesDict.Add(name, index);
                    result += "/" + index;
                }
            }
            return result.Substring(1);
        }
    }
}
