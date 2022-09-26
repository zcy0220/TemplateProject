/**
 * 强制打成一个AssetBundle的资源配置
 * 即使强制勾选打成一个包
 * AssetBundleBuilder也会根据其被依赖情况打包，防止冗余
 */

using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System;

namespace Editor.BuildProjectPacker
{
    public class ForcePackAssetInfo
    {
        public bool Foldout;
        public int IndentLevel;
        public bool Toggle;
        public string AssetPath;
        public List<ForcePackAssetInfo> Children = new List<ForcePackAssetInfo>();
    }

    [Serializable]
    public class ForcePackAssetList
    {
        public List<string> List = new List<string>();
    }

    public class ForcePackAssetBuilder : EditorWindow
    {
        /// <summary>
        /// 根目录
        /// </summary>
        private static ForcePackAssetInfo _forcePackData;
        /// <summary>
        /// 映射数据
        /// </summary>
        private static List<string> _forcePackAssetDataList = new List<string>();
        /// <summary>
        /// GUI内容
        /// </summary>
        private static Dictionary<string, GUIContent> _contentDict = new Dictionary<string, GUIContent>();

        /// <summary>
        /// 初始化
        /// </summary>
        public static void Open()
        {
            var window = EditorWindow.GetWindow(typeof(ForcePackAssetBuilder));
            Refresh();
            window.Show();
        }


        /// <summary>
        /// 刷新AssetBundle目录数据
        /// </summary>
        private static void Refresh()
        {
            Clear();
            var configPath = BuildProjectConfig.ForcePackAssetConfig;
            if (File.Exists(configPath))
            {
                var text = File.ReadAllText(configPath);
                var jsonData = JsonUtility.FromJson<ForcePackAssetList>(text);
                _forcePackAssetDataList = jsonData.List;
            }
            _forcePackData = new ForcePackAssetInfo();
            _forcePackData.Foldout = false;
            _forcePackData.IndentLevel = 0;
            _forcePackData.AssetPath = BuildProjectConfig.PackRootPath;
            _forcePackData.Toggle = _forcePackAssetDataList.Contains(configPath);
            //_packagesData.IsFolder = true;
            LoadDirectories(_forcePackData);
        }

        /// <summary>
        /// 导出配置文件
        /// </summary>
        private static void Export()
        {
            var data = new ForcePackAssetList();
            data.List = _forcePackAssetDataList;
            var text = JsonUtility.ToJson(data, true);
            File.WriteAllText(BuildProjectConfig.ForcePackAssetConfig, text);
        }

        /// <summary>
        /// 清理缓存
        /// </summary>
        private static void Clear()
        {
            _forcePackData = null;
            _contentDict.Clear();
            _forcePackAssetDataList.Clear();
        }

        /// <summary>
        /// 递归加载文件数据
        /// </summary>
        private static bool LoadDirectories(ForcePackAssetInfo data)
        {
            foreach (var dirPath in Directory.GetDirectories(data.AssetPath))
            {
                var child = new ForcePackAssetInfo();
                child.IndentLevel = data.IndentLevel + 1;
                child.AssetPath = dirPath.Replace("\\", "/");
                child.Toggle = _forcePackAssetDataList.Contains(dirPath);
                data.Children.Add(child);
                data.Foldout = true;
                if (LoadDirectories(child) || child.Toggle)
                {
                    data.Foldout = true;
                }
            }
            return data.Foldout;
        }


        #region 绘制界面
        private Vector2 scrollPos;

        private void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Refresh"))
            {
                Refresh();
            }

            if (GUILayout.Button("Export"))
            {
                Export();
            }
            EditorGUILayout.EndHorizontal();

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            DrawData(_forcePackData);
            EditorGUILayout.EndScrollView();
        }

        private void OnDestroy()
        {
            Clear();
        }

        /// <summary>
        /// 绘制数据
        /// </summary>
        /// <param name="data"></param>
        private static void DrawData(ForcePackAssetInfo data)
        {
            var content = GetGUIContent(data.AssetPath);
            if (content == null)
            {
                return;
            }
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(data.IndentLevel * 20);
            var tag = data.Toggle;
            data.Toggle = GUILayout.Toggle(data.Toggle, "", GUILayout.Width(15));
            if (tag != data.Toggle)
            {
                if (data.Toggle)
                {
                    _forcePackAssetDataList.Add(data.AssetPath);
                }
                else
                {
                    _forcePackAssetDataList.Remove(data.AssetPath);
                }
            }
            if (data.Children.Count > 0)
            {
                data.Foldout = EditorGUILayout.Foldout(data.Foldout, content, true);
            }
            else
            {
                EditorGUILayout.LabelField(content, GUILayout.Height(15));
            }
            EditorGUILayout.EndHorizontal();
            if (data.Foldout)
            {
                for (int i = 0; i < data.Children.Count; i++)
                {
                    var child = data.Children[i];
                    DrawData(child);
                }
            }
        }

        /// <summary>
        /// 获取资源图标
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static GUIContent GetGUIContent(string path)
        {
            path = path.Replace("\\", "/");
            _contentDict.TryGetValue(path, out var content);
            if (content != null)
            {
                return content;
            }
            var asset = AssetDatabase.LoadAssetAtPath(path, typeof(UnityEngine.Object));
            if (asset != null)
            {
                content = new GUIContent(asset.name, AssetDatabase.GetCachedIcon(path));
                _contentDict[path] = content;
                return content;
            }
            return null;
        }

        #endregion
    }
}
