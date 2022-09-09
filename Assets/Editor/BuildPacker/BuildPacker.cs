/**
 * 包体构建工具
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using Editor.AssetBundlePacker;

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

            AssetDatabase.Refresh();

            var options = BuildOptions.ShowBuiltPlayer | BuildOptions.DetailedBuildReport;
            if (isDebug)
            {
                options |= BuildOptions.Development;
            }
            //if (isBuildAssetBundle)
            //{
            //    if (!AssetBundlePacker.Build())
            //    {
            //        LogUtil.Error($"Build {apkName} failed.");
            //        return;
            //    }
            //}
            //else
            //{
            //    LogUtil.Debug($"BuildOptions.BuildScriptsOnly");
            //    options |= BuildOptions.BuildScriptsOnly;
            //}

            var apkName = PlayerSettings.productName + "_" + (isDebug ? "Debug" : "Release") + ".apk";
            var apkPath = Path.Combine("Builds/Android", apkName);
            var report = BuildPipeline.BuildPlayer(GetBuildScenes(), apkPath, BuildTarget.Android, options);
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
