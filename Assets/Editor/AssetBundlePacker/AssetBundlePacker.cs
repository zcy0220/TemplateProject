/**
 * AssetBundle打包工具
 */

using UnityEditor;
using System.Collections.Generic;

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
            var assetPaths = new string[]
            {
                "Assets/ArtPack/Pack/Tests/TestFolder/Test1.prefab",
                "Assets/ArtPack/Pack/Tests/TestFolder/Test2.prefab",
                "Assets/ArtPack/Pack/Tests/Test3.prefab",
                "Assets/ArtPack/Pack/Tests/TestScene.unity",
            };
            var builder = new AssetBundleBuilder();
            builder.Start(assetPaths);
        }
    }
}
