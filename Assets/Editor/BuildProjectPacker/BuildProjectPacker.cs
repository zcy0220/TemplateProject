/**
 * 包体构建工具
 */

using HybridCLR.Editor.Commands;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Editor.BuildProjectPacker
{
    public enum EBuildAssetBundle
    {
        AllAssetBundle,
        OnlyScriptAssetBundle,
        OnlyResourceAssetBundle,
        NoAssetBundle
    }

    public class BuildProjectPacker : EditorWindow
    {
        /// <summary>
        /// 所有渠道列表
        /// </summary>
        private string[] _channelList = new string[] { "default", "official" };
        /// <summary>
        /// 当前选中的渠道
        /// </summary>
        private static string _channel = "default";
        /// <summary>
        /// 当前选中的平台
        /// </summary>
        private static BuildTarget _target = BuildTarget.Android;
        /// <summary>
        /// 是否Debug
        /// </summary>
        private static bool _isDebug = false;
        /// <summary>
        /// 构建AssetBundle类型
        /// </summary>
        private static EBuildAssetBundle _assetBundleType = EBuildAssetBundle.AllAssetBundle;

        /// <summary>
        /// 窗口入口
        /// </summary>
        [MenuItem("Tools/BuildProjectPacker")]
        public static void Open()
        {
            var window = EditorWindow.GetWindow(typeof(BuildProjectPacker));
            window.Show();
        }

        /// <summary>
        /// 绘制窗口
        /// </summary>
        private void OnGUI()
        {
            //选择编译平台
            _target = (BuildTarget)EditorGUILayout.EnumPopup("BuildTarget", _target);

            //选择渠道
            GUILayout.BeginHorizontal();
            GUILayout.Label("Channel", GUILayout.Width(147));
            if (EditorGUILayout.DropdownButton(new GUIContent(_channel), FocusType.Keyboard))
            {
                var menu = new GenericMenu();
                for(int i = 0; i < _channelList.Length; i++)
                {
                    var value = _channelList[i];
                    menu.AddItem(new GUIContent(value), _channel.Equals(value), (obj) => { _channel = obj.ToString(); }, value);
                }
                menu.ShowAsContext();
            }
            GUILayout.EndHorizontal();

            _assetBundleType = (EBuildAssetBundle)EditorGUILayout.EnumPopup("AssetBundleType", _assetBundleType);

            //AssetBundle构建相关
            GUILayout.BeginHorizontal();
            GUILayout.Label("AssetBundleBuild", GUILayout.Width(147));
            if (GUILayout.Button("ForcePackConfig", GUILayout.Height(20), GUILayout.Width(200)))
            {
                ForcePackAssetBuilder.Open();
            }
            if (GUILayout.Button("Build", GUILayout.Height(20)))
            {
                BuildAssetBundle(true);
            }
            GUILayout.EndHorizontal();

            //选择Debug、OnlyScriptAssetBundle、OnlyResourceAssetBundle
            GUILayout.BeginHorizontal();
            _isDebug = GUILayout.Toggle(_isDebug, "Debug", GUILayout.Width(149));
            GUILayout.EndHorizontal();

            //构建按钮
            GUILayout.BeginHorizontal();
            
            if (GUILayout.Button("BuildPackage", GUILayout.Height(30)))
            {
                Build();
            }
            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// 构建
        /// </summary>
        private void Build()
        {
            switch (_target)
            {
                case BuildTarget.Android:
                    BuildAndroid();
                    break;
            }
        }

        /// <summary>
        /// 写入渠道配置
        /// </summary>
        private void BuildChannelConfig()
        {
            var filePath = Path.Combine(BuildProjectConfig.ProjectBuildStreamingAssetsPath, AppConfig.LocalChannelConfig);
            Directory.CreateDirectory(BuildProjectConfig.ProjectBuildStreamingAssetsPath);
            using (var sw = new StreamWriter(filePath))
            {
                sw.Write(_channel);
            }
        }

        /// <summary>
        /// 构建AssetBundles
        /// </summary>
        private void BuildAssetBundle(bool isOpenURL = false)
        {
            switch (_assetBundleType)
            {
                case EBuildAssetBundle.AllAssetBundle:
                    AssetBundlePacker.BuildAllAssetBundles(_target);
                    break;
                case EBuildAssetBundle.OnlyScriptAssetBundle:
                    AssetBundlePacker.BuildScriptAssetBundles(_target);
                    break;
                case EBuildAssetBundle.OnlyResourceAssetBundle:
                    AssetBundlePacker.BuildResourceAssetBundles(_target);
                    break;
            }
#if UNITY_EDITOR
            if (isOpenURL)
            {
                Application.OpenURL($"file:///{BuildProjectConfig.ProjectBuildStreamingAssetsPath}");
            }
#endif
        }

        /// <summary>
        /// 打包的场景列表
        /// </summary>
        private string[] GetBuildScenes()
        {
            return new string[] { "Assets/Scenes/AppLauncherScene.unity" };
        }

        /// <summary>
        /// 构建APK
        /// </summary>
        private void BuildAndroid()
        {
            if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.Android)
            {
                Debug.Log($"build apk ==> switch active build target to android");
                EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
            }
            EditorUserBuildSettings.development = _isDebug;

            var apkName = PlayerSettings.productName + "_" + (_isDebug ? "Debug" : "Release") + ".apk";
            var outputPath = $"{BuildProjectConfig.ProjectBuildPath}/Android";
            var location = Path.Combine(outputPath, apkName);
            var buildOptions = BuildOptions.CompressWithLz4;

            PrebuildCommand.GenerateAll();

            var buildPlayerOptions = new BuildPlayerOptions()
            {
                scenes = GetBuildScenes(),
                locationPathName = location,
                options = buildOptions,
                target = _target,
                targetGroup = BuildTargetGroup.Android,
            };

            Debug.Log("build apk ==> 第1次打包(为了生成补充AOT元数据dll)");
            BuildPipeline.BuildPlayer(buildPlayerOptions);
            FileUtil.DeleteFileOrDirectory(Application.streamingAssetsPath + ".meta");
            FileUtil.DeleteFileOrDirectory(Application.streamingAssetsPath);
            BuildChannelConfig();
            if (_assetBundleType == EBuildAssetBundle.NoAssetBundle)
            {
                var sourceChannelConfigPath = Path.Combine(BuildProjectConfig.ProjectBuildStreamingAssetsPath, AppConfig.LocalChannelConfig);
                var destChannelConfigPath = Path.Combine(Application.streamingAssetsPath, AppConfig.LocalChannelConfig);
                Directory.CreateDirectory(Application.streamingAssetsPath);
                FileUtil.CopyFileOrDirectory(sourceChannelConfigPath, destChannelConfigPath);
            }
            else
            {
                BuildAssetBundle();
                FileUtil.CopyFileOrDirectory(BuildProjectConfig.ProjectBuildStreamingAssetsPath, Application.streamingAssetsPath);
            }
            Debug.Log("build apk ==> 第2次打包");
            AssetDatabase.Refresh();
            var report = BuildPipeline.BuildPlayer(buildPlayerOptions);
            if (report.summary.result == BuildResult.Succeeded)
            {
#if UNITY_EDITOR
                Application.OpenURL($"file:///{outputPath}");
#endif
            }
            else
            {
                Debug.LogError($"build apk ==> build {apkName} failed! build result:{report.summary.result}");
            }
            FileUtil.DeleteFileOrDirectory(Application.streamingAssetsPath + ".meta");
            FileUtil.DeleteFileOrDirectory(Application.streamingAssetsPath);
            AssetDatabase.Refresh();
        }
    }
}
