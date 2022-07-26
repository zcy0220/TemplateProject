﻿/**
 * 快捷键设置
 */

using UnityEditor;
using UnityEditor.SceneManagement;

namespace Editor.KeyboardSettings
{
    public class KeyboardSettings
    {
        [MenuItem("Tools/KeyboardSettings/开始游戏 _F5")]
        private static void StartGame()
        {
            var launcherScene = "Assets/Scenes/GameLauncherScene.unity";
            if (!EditorApplication.isPlaying)
            {
                EditorSceneManager.OpenScene(launcherScene);
                EditorApplication.isPlaying = true;
            }
        }
    }
}

