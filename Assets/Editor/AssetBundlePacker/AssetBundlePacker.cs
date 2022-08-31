﻿/**
 * AssetBundle打包工具
 */

using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System;
using System.Runtime.Serialization.Formatters.Binary;

namespace Editor.AssetBundlePacker
{
    public class AssetBundlePacker
    {
        [MenuItem("Tools/AssetBundlePacker/Build")]
        public static void Build()
        {
            //var builder = new AssetBundleBuilder();
            ///**
            // * 优先构建场景相关AssetBundle
            // */
            //builder.BuildSceneAssetBundles();
            var targetPlatform = EditorUserBuildSettings.activeBuildTarget;
            //BuildPipeline.SetAssetBundleEncryptKey(null);
            //if (LauncherConfig.ASSETBUNDLE_ENCRYPTKEY.Length > 0)
            //{
            //    options |= BuildAssetBundleOptions.EnableProtection;
            //    BuildPipeline.SetAssetBundleEncryptKey(LauncherConfig.ASSETBUNDLE_ENCRYPTKEY);
            //}

            //var abm = BuildPipeline.BuildAssetBundles(outputPath, _buildList.ToArray(), options, targetPlatform);
            //if (abm == null)
            //{
            //    LogUtil.Error("BuildPipeline.BuildAssetBundles Failed!");
            //    return false;
            //}
        }

        /// <summary>
        /// 构建测试AssetBundle
        /// </summary>
        [MenuItem("Tools/AssetBundlePacker/Test")]
        public static void Test()
        {
            var forcePacks = new HashSet<string>()
            {
                "Assets/ArtPack/Pack/Shaders"
            };
            var assetPaths = new string[]
            {
                "Assets/ArtPack/Pack/Tests/TestFolder/Test1.prefab",
                "Assets/ArtPack/Pack/Tests/TestFolder/Test4.prefab",
                //"Assets/ArtPack/Pack/Tests/Test3.prefab",
                //"Assets/ArtPack/Pack/Tests/TestScene.unity",
            };
            var builder = new AssetBundleBuilder();
            builder.Start(assetPaths, forcePacks);
        }


        /// <summary>
        /// 构建所有资源依赖
        /// </summary>
        [MenuItem("Tools/AssetBundlePacker/Dependencies")]
        public static void Dependencies()
        {
            var stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
            var builder = new AssetBundleBuilder();
            builder.GetAllAssetsDependencies();
            stopwatch.Stop();
            var timespan = stopwatch.Elapsed;
            Debug.Log($"<color=cyan>构建依赖耗时：{timespan.TotalMilliseconds}ms</color>");
        }
    }
}
