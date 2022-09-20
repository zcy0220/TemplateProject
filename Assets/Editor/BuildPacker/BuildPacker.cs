/**
 * 包体构建工具
 */

using System;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build.Reporting;

namespace Editor.BuildPacker
{
    public class BuildPacker
    {
        [MenuItem("Tools/BuildPacker/Windows(Release)")]
        public static void BuildWindowsRelease()
        {
        }

        [MenuItem("Tools/BuildPacker/Android(Debug)")]
        public static void BuildAndroidDebug()
        {
            BuildAPK(true, false);
        }

        [MenuItem("Tools/BuildPacker/Android(Release)")]
        public static void BuildAndroidRelease()
        {
            BuildAPK(false, true);
        }
        
        public static void BuildAPK(bool isDebug, bool isBuildAssetBundle)
        {
            if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.Android)
            {
                Debug.Log($"build apk ==> switch active build target to android");
                EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
            }
            if (EditorUserBuildSettings.development != isDebug)
            {
                Debug.Log($"build apk ==> editor user build settings.development to {isDebug}");
                EditorUserBuildSettings.development = isDebug;
            }

            FileUtil.CopyFileOrDirectory(AssetBundlePacker.AssetBundleConfig.AssetBundlesStreamingAssetsPath, Application.streamingAssetsPath);

            AssetDatabase.Refresh();

            var options = BuildOptions.ShowBuiltPlayer | BuildOptions.DetailedBuildReport;
            if (isDebug)
            {
                options |= BuildOptions.Development;
            }

            if (isBuildAssetBundle)
            {
                if (!AssetBundlePacker.AssetBundlePacker.Build())
                {
                    return;
                }
            //}
            //else
            //{
            //    options |= BuildOptions.BuildScriptsOnly;
            }

            var apkName = PlayerSettings.productName + "_" + (isDebug ? "Debug" : "Release") + ".apk";
            var apkPath = Path.Combine("Builds/Android", apkName);
            var report = BuildPipeline.BuildPlayer(GetBuildScenes(), apkPath, BuildTarget.Android, options);

            FileUtil.DeleteFileOrDirectory(Application.streamingAssetsPath + ".meta");
            FileUtil.DeleteFileOrDirectory(Application.streamingAssetsPath);
            if (report.summary.result != BuildResult.Succeeded)
            {
                Debug.LogError($"build apk ==> build {apkName} failed! build result:{report.summary.result}");
                return;
            }
        }

        /// <summary>
        /// 打包的场景列表
        /// </summary>
        private static string[] GetBuildScenes()
        {
            //List<string> names = new List<string>();
            //foreach (var scene in EditorBuildSettings.scenes)
            //{
            //    if (scene == null) continue;
            //    if (scene.enabled) names.Add(scene.path);
            //}
            //return names.ToArray();
            return new string[] { "Assets/Scenes/GameLauncherScene.unity" };
        }
    }
}
