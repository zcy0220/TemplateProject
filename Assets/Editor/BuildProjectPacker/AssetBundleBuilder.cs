﻿/**
 * AssetBundle构建类
 */

using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using HybridCLR.Editor.Commands;
using HybridCLR.Editor;
using System.Linq;
using GameUnityFramework.Resource;

namespace Editor.BuildProjectPacker
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
        /// 锁住资源对应的AB包名
        /// </summary>
        public bool LockAssetBundleName = false;
        /// <summary>
        /// 该资源的所有依赖项
        /// 不递归
        /// </summary>
        public List<string> AllDependencies;
        /// <summary>
        /// 依赖该资源的所有资源项
        /// 不递归
        /// </summary>
        public List<string> BeDependencies;
    }

    public class AssetBundleBuilder
    {
        ///=========================================================================================================================================================
        ///=========================================================================================================================================================
        ///=========================================================================================================================================================
        /// <summary>
        /// 构建脚本AssetBundle
        /// </summary>
        public void BuildScriptAssetBundles(BuildTarget target)
        {
            var scriptAssetBundle = GetScriptAssetBundle(target);
            var assetBundleBuilds = new AssetBundleBuild[] { scriptAssetBundle };
            BuildAssetBundles(target, assetBundleBuilds);
            BuildAssetBundleVersionConfig();
        }

        /// <summary>
        /// 构建资源相关AssetBundles
        /// </summary>
        /// <param name="target"></param>
        public void BuildResourceAssetBundles(BuildTarget target)
        {
            CreateAssetDependsMap();
            GroupingAssetBundles();
            CreatePackConfig();
            var assetBundleBuilds = GetResourceAssetBundles();
            BuildAssetBundles(target, assetBundleBuilds.ToArray());
            BuildAssetBundleVersionConfig();
        }

        /// <summary>
        /// 构建全部AssetBundles
        /// </summary>
        /// <param name="target"></param>
        public void BuildAllAssetBundles(BuildTarget target)
        {
            CreateAssetDependsMap();
            GroupingAssetBundles();
            CreatePackConfig();
            var assetBundleBuilds = GetResourceAssetBundles();
            var scriptAssetBundle = GetScriptAssetBundle(target);
            assetBundleBuilds.Add(scriptAssetBundle);
            BuildAssetBundles(target, assetBundleBuilds.ToArray());
            BuildAssetBundleVersionConfig();
        }
        ///=========================================================================================================================================================
        ///=========================================================================================================================================================
        ///=========================================================================================================================================================
        /// <summary>
        /// 获取脚本AssetBundle
        /// </summary>
        /// <returns></returns>
        private AssetBundleBuild GetScriptAssetBundle(BuildTarget target)
        {
            var tempDir = $"{BuildProjectConfig.ScriptAssetBundleTempPath}/{target}";
            Directory.CreateDirectory(tempDir);
            CompileDllCommand.CompileDll(target);

            var assetList = new List<string>();
            var hotfixDllSrcDir = SettingsUtil.GetHotUpdateDllsOutputDirByTarget(target);
            foreach (var dll in SettingsUtil.HotUpdateAssemblyFiles)
            {
                var dllPath = $"{hotfixDllSrcDir}/{dll}";
                var dllBytesPath = $"{tempDir}/{dll}.bytes";
                File.Copy(dllPath, dllBytesPath, true);
                assetList.Add(dllBytesPath);
                Debug.Log($"build script assetbundles ==> copy hotfix dll {dllPath} -> {dllBytesPath}");
            }

            var aotDllDir = SettingsUtil.GetAssembliesPostIl2CppStripDir(target);
            var manifest = BuildProjectConfig.HotUpdateAssemblyManifest;
            if (manifest == null)
            {
                throw new Exception($"build script assetbundles ==> resource asset:{nameof(HotUpdateAssemblyManifest)} 配置不存在，请在Resources目录下创建");
            }
            var AOTMetaAssemblies = (manifest.AOTMetadataDlls ?? Array.Empty<string>()).ToList();
            foreach (var dll in AOTMetaAssemblies)
            {
                var dllPath = $"{aotDllDir}/{dll}.dll";
                if (!File.Exists(dllPath))
                {
                    Debug.LogError($"ab中添加AOT补充元数据dll:{dllPath} 时发生错误,文件不存在。裁剪后的AOT dll在BuildPlayer时才能生成，因此需要你先构建一次游戏App后再打包。");
                    continue;
                }
                var dllBytesPath = $"{tempDir}/{dll}.bytes";
                File.Copy(dllPath, dllBytesPath, true);
                assetList.Add(dllBytesPath);
                Debug.Log($"build script assetbundles ==> copy AOT dll {dllPath} -> {dllBytesPath}");
            }

            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            var scriptAssetBundle = new AssetBundleBuild()
            {
                assetBundleName = "scripts",
                assetNames = assetList.Select(s => s.Substring(s.IndexOf("Assets/"))).ToArray()
            };
            return scriptAssetBundle;
        }
        ///=========================================================================================================================================================
        ///=========================================================================================================================================================
        ///=========================================================================================================================================================
        private Dictionary<string, AssetBundleItem> _assetBundleItemDict = new Dictionary<string, AssetBundleItem>();
        public string[] AllAssetPaths { get; set; }
        public string[] AllForcePacks { get; set; }
        public Dictionary<string, DependencyData> AllAssetsDependencies { get; set; }
        /// <summary>
        /// 遍历资源，建立依赖映射
        /// </summary>
        private void CreateAssetDependsMap()
        {
            Debug.Log($"<color=cyan>assetbundle build ==> ：create asset depends map</color>");
            //建立所有资源的AssetBundleItem
            for (int i = 0; i < AllAssetPaths.Length; i++)
            {
                var path = AllAssetPaths[i];
                path = path.Replace("\\", "/");
                if (AllAssetsDependencies.TryGetValue(path, out var dependencyData))
                {
                    if (dependencyData.AllDependencies != null)
                    {
                        for (int j = 0; j < dependencyData.AllDependencies.Length; j++)
                        {
                            var dependPath = dependencyData.AllDependencies[j];
                            if (!_assetBundleItemDict.ContainsKey(dependPath))
                            {
                                var item = new AssetBundleItem();
                                item.AssetPath = dependPath;
                                item.AssetBundleName = dependPath;
                                item.LockAssetBundleName = dependPath.EndsWith(".unity");
                                if (AllAssetsDependencies.TryGetValue(dependPath, out var data))
                                {
                                    if (data.BeDependencies != null)
                                    {
                                        item.BeDependencies = new List<string>();
                                        foreach (var beDependPath in data.BeDependencies)
                                        {
                                            //是否是有效的资源
                                            if (IsValidAsset(beDependPath))
                                            {
                                                item.BeDependencies.Add(beDependPath);

                                                if (beDependPath.EndsWith(".unity") &&
                                                    item.AssetPath.StartsWith(BuildProjectConfig.PackRootPath, System.StringComparison.OrdinalIgnoreCase))
                                                {
                                                    //如果该资源需要被代码动态加载，然后被依赖项中有场景，就锁住AssetBundleName
                                                    //因为Unity不能从一个Scene AssetBundle中加载资源
                                                    item.LockAssetBundleName = true;
                                                }
                                            }
                                        }
                                    }
                                    if (data.AllDependencies != null)
                                    {
                                        item.AllDependencies = new List<string>(data.AllDependencies);
                                    }
                                }
                                _assetBundleItemDict.Add(dependPath, item);
                            }
                        }
                    }
                }
            }

            //根据需要强制打成一个包的配置
            for (int i = 0; i < AllForcePacks.Length; i++)
            {
                var path = AllForcePacks[i].Replace("\\", "/");
                if (File.Exists(path))
                {
                    if (_assetBundleItemDict.TryGetValue(path, out var item))
                    {
                        item.LockAssetBundleName = true;
                    }
                }
                else if (Directory.Exists(path))
                {
                    var files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
                    for (var j = 0; j < files.Length; j++)
                    {
                        var fpath = files[j].Replace("\\", "/");
                        if (_assetBundleItemDict.TryGetValue(fpath, out var item))
                        {
                            //item.BeDependencies.Clear();
                            item.BeDependencies.Add(path);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 判断是否为有效的资源
        /// </summary>
        private bool IsValidAsset(string path)
        {
            var stack = new Stack<string>();
            stack.Push(path);
            while (stack.Count > 0)
            {
                path = stack.Pop();
                //是否是有效的资源
                if (path.StartsWith(BuildProjectConfig.PackRootPath, System.StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
                if (_assetBundleItemDict.TryGetValue(path, out var item))
                {
                    for (int i = 0; i < item.BeDependencies.Count; i++)
                    {
                        stack.Push(item.BeDependencies[i]);
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 分组AssetBundles
        /// </summary>
        private void GroupingAssetBundles()
        {
            Debug.Log($"<color=cyan>assetbundle build ==> ：grouping assetbundles</color>");
            foreach (var item in _assetBundleItemDict)
            {
                if (AssetBundleHelper.IsValidAssetPath(item.Key) && item.Value.BeDependencies != null)
                {
                    if (item.Value.BeDependencies.Count == 1)
                    {
                        var beDependPath = item.Value.BeDependencies[0];
                        if (_assetBundleItemDict.TryGetValue(beDependPath, out var abItem))
                        {
                            if (abItem.AllDependencies == null)
                            {
                                abItem.AllDependencies = new List<string>() { item.Key };
                            }
                            else
                            {
                                if (!abItem.AllDependencies.Contains(item.Key))
                                {
                                    abItem.AllDependencies.Add(item.Key);
                                }
                            }
                            ModifyDependAssetBundleName(item.Value, abItem.AssetBundleName);
                        }
                        else
                        {
                            ModifyDependAssetBundleName(item.Value, beDependPath);
                        }
                    }

                    if (item.Value.BeDependencies.Count > 1)
                    {
                        //防止有极特殊的情况下，有相互依赖的资源
                        var checkHashSet = new HashSet<string>();
                        var rootDependPath = string.Empty;
                        var sameRootDependPath = true;
                        var stack = new Stack<string>();
                        stack.Push(item.Key);
                        while (stack.Count > 0)
                        {
                            var path = stack.Pop();
                            if (_assetBundleItemDict.TryGetValue(path, out var abItem))
                            {
                                if (abItem.BeDependencies.Count == 0)
                                {
                                    if (string.IsNullOrEmpty(rootDependPath))
                                    {
                                        rootDependPath = path;
                                    }
                                    else
                                    {
                                        if (rootDependPath != path)
                                        {
                                            sameRootDependPath = false;
                                            break;
                                        }
                                    }
                                }

                                for (int i = 0; i < abItem.BeDependencies.Count; i++)
                                {
                                    var beDependPath = abItem.BeDependencies[i];
                                    if (!checkHashSet.Contains(beDependPath))
                                    {
                                        checkHashSet.Add(beDependPath);
                                        stack.Push(beDependPath);
                                    }
                                }
                            }
                            else
                            {
                                if (string.IsNullOrEmpty(rootDependPath))
                                {
                                    rootDependPath = path;
                                }
                                else
                                {
                                    if (rootDependPath != path)
                                    {
                                        sameRootDependPath = false;
                                        break;
                                    }
                                }
                            }
                            if (!sameRootDependPath)
                            {
                                break;
                            }
                        }
                        if (!string.IsNullOrEmpty(rootDependPath) && sameRootDependPath)
                        {
                            ModifyDependAssetBundleName(item.Value, rootDependPath);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 修改依赖中的AssetBundleName
        /// </summary>
        /// <param name="assetBundleName"></param>
        private void ModifyDependAssetBundleName(AssetBundleItem item, string assetBundleName)
        {
            if (!item.LockAssetBundleName)
            {
                if (item.AllDependencies != null)
                {
                    for (int i = 0; i < item.AllDependencies.Count; i++)
                    {
                        var dependPath = item.AllDependencies[i];
                        if (dependPath != item.AssetPath && _assetBundleItemDict.TryGetValue(dependPath, out var abItem))
                        {
                            if (abItem.AssetBundleName == item.AssetBundleName)
                            {
                                abItem.AssetBundleName = assetBundleName;
                            }
                        }
                    }
                }
                item.AssetBundleName = assetBundleName;
            }
        }

        /// <summary>
        /// 生成AssetBundleConfig配置文件
        /// </summary>
        private void CreatePackConfig()
        {
            Debug.Log($"<color=cyan>assetbundle build ==> ：create packconfig</color>");
            var packConfigPath = BuildProjectConfig.PackConfigPath;
            var packConfigBuilder = new PackConfigBuilder();
            packConfigBuilder.Build(_assetBundleItemDict);
            var item = new AssetBundleItem();
            item.AssetPath = packConfigPath;
            item.AssetBundleName = packConfigPath;
            if (_assetBundleItemDict.ContainsKey(packConfigPath))
            {
                _assetBundleItemDict[packConfigPath] = item;
            }
            else
            {
                _assetBundleItemDict.Add(packConfigPath, item);
                AssetDatabase.Refresh();
            }
        }
        /// <summary>
        /// 获取脚本AssetBundle
        /// </summary>
        /// <returns></returns>
        private List<AssetBundleBuild> GetResourceAssetBundles()
        {
            var assetBundleBuildDict = new Dictionary<string, List<string>>();
            var assetBundleBuildList = new List<AssetBundleBuild>();
            foreach (var item in _assetBundleItemDict)
            {
                if (AssetBundleHelper.IsValidAssetPath(item.Key))
                {
                    if (!assetBundleBuildDict.ContainsKey(item.Value.AssetBundleName))
                    {
                        assetBundleBuildDict.Add(item.Value.AssetBundleName, new List<string>());
                    }
                    //和场景同AssetBundle的资源不用设置AB包名
                    if (!(!item.Key.EndsWith(".unity") && item.Value.AssetBundleName.EndsWith(".unity")))
                    {
                        assetBundleBuildDict[item.Value.AssetBundleName].Add(item.Key);
                    }
                }
            }
            foreach (var item in assetBundleBuildDict)
            {
                var build = new AssetBundleBuild();
                build.assetBundleName = ResourcePathHelper.GetAssetBundleName(item.Key);
                build.assetNames = item.Value.ToArray();
                assetBundleBuildList.Add(build);
            }
            return assetBundleBuildList;
        }
        ///=========================================================================================================================================================
        ///=========================================================================================================================================================
        ///=========================================================================================================================================================
        /// <summary>
        /// 构建AssetBundle信息列表
        /// </summary>
        private void BuildAssetBundleVersionConfig()
        {
            var assetBundleVersionConfig = new AssetBundleVersionConfig();
            var now = System.DateTime.Now;
            assetBundleVersionConfig.Version = now.ToString("yyyyMMddHHmm");
            try
            {
                if (AppConfig.AssetBundleEncryptkey.Length > 0)
                {
                    AssetBundle.SetAssetBundleDecryptKey(AppConfig.AssetBundleEncryptkey);
                }
                var assetBundlePath = Path.Combine(BuildProjectConfig.AssetBundleOutputPath, BuildProjectConfig.AssetBundleFolder);
                var mainAssetBundle = AssetBundle.LoadFromFile(assetBundlePath);
                var assetBundleManifest = mainAssetBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
                var allAssetBundles = assetBundleManifest.GetAllAssetBundles();
                for (int i = 0; i < allAssetBundles.Length; i++)
                {
                    var assetBundleInfo = GetAssetBundleResourceInfo(allAssetBundles[i]);
                    assetBundleVersionConfig.AssetBundleInfoList.Add(assetBundleInfo);
                }
                assetBundleVersionConfig.AssetBundleInfoList.Add(GetAssetBundleResourceInfo(BuildProjectConfig.AssetBundleFolder));
            }
            catch (Exception e)
            {
                Debug.LogWarning(e.Message);
            }
            var exportPath = Path.Combine(BuildProjectConfig.ProjectBuildStreamingAssetsPath, BuildProjectConfig.AssetBundleVersionConfigFile);
            File.WriteAllText(exportPath, JsonUtility.ToJson(assetBundleVersionConfig, true));
        }

        /// <summary>
        /// 获取AssetBundleInfo
        /// </summary>
        /// <param name="assetBundleName"></param>
        /// <returns></returns>
        private AssetBundleResourceInfo GetAssetBundleResourceInfo(string assetBundleName)
        {
            var assetBundleInfo = new AssetBundleResourceInfo();
            assetBundleInfo.Name = assetBundleName;
            var path = Path.Combine(BuildProjectConfig.AssetBundleOutputPath, assetBundleInfo.Name);
            var fileInfo = new FileInfo(path);
            assetBundleInfo.Size = fileInfo.Length;
            return assetBundleInfo;
        }

        /// <summary>
        /// 最终构建AssetBundles
        /// </summary>
        private bool BuildAssetBundles(BuildTarget target, AssetBundleBuild[] assetBundleBuildList)
        {
            Debug.Log($"<color=cyan>assetbundle build ==> ：build assetbundles</color>");
            var outputPath = BuildProjectConfig.AssetBundleOutputPath;
            Directory.CreateDirectory(outputPath);
            var options = BuildAssetBundleOptions.ChunkBasedCompression;
            if (AppConfig.AssetBundleEncryptkey.Length > 0)
            {
                options |= BuildAssetBundleOptions.EnableProtection;
                BuildPipeline.SetAssetBundleEncryptKey(AppConfig.AssetBundleEncryptkey);
            }
            else
            {
                BuildPipeline.SetAssetBundleEncryptKey(null);
            }
            var assetBundleManifest = BuildPipeline.BuildAssetBundles(outputPath, assetBundleBuildList, options, target);
            if (assetBundleManifest == null)
            {
                Debug.LogError($"assetbundle build ==> ：build pipeline.build assetbundles failed!");
                return false;
            }
            else
            {
                Debug.Log($"<color=cyan>assetbundle build ==> ：build pipeline.build assetbundles success</color>");
                Debug.Log($"<color=cyan>assetbundle build ==> : assetbundle count：{assetBundleManifest.GetAllAssetBundles().Length}</color>");
                CleanAssetBundles(assetBundleBuildList);
                return true;
            }
        }

        // <summary>
        // 清理多余的AssetBundles
        // </summary>
        private void CleanAssetBundles(AssetBundleBuild[] assetBundleBuildList)
        {
            Debug.Log($"<color=cyan>assetbundle build ==> ：clean assetbundles</color>");
            var assetBundleNameHashSet = new HashSet<string>();
            for(int i = 0; i < assetBundleBuildList.Length; i++)
            {
                assetBundleNameHashSet.Add(assetBundleBuildList[i].assetBundleName);
            }
            string outputPath = BuildProjectConfig.AssetBundleOutputPath;
            var outputDirName = Path.GetFileName(outputPath);
            var fileList = Directory.GetFiles(outputPath, "*.*", SearchOption.AllDirectories);
            foreach (var file in fileList)
            {
                var fileName = Path.GetFileName(file).Split('.')[0];
                if (fileName != outputDirName && !assetBundleNameHashSet.Contains(fileName))
                {
                    FileUtil.DeleteFileOrDirectory(file);
                }
            }
            AssetDatabase.Refresh();
        }
    }
}
