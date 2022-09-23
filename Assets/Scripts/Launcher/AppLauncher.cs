/**
 * App启动项 
 */

using UnityEngine;

public class AppLauncher : MonoBehaviour
{
    /// <summary>
    /// 存一份脚本AssetBundle供外部使用
    /// </summary>
    public static AssetBundle DllAssetBundle;

    /// <summary>
    /// 初始化
    /// </summary>
    private void Awake()
    {
        if (AppConfig.AssetBundleEncryptkey.Length > 0)
        {
            AssetBundle.SetAssetBundleDecryptKey(AppConfig.AssetBundleEncryptkey);
        }
    }

    /// <summary>
    /// 热更新
    /// </summary>
    private void Start()
    {
        var prefab = Resources.Load<GameObject>("HotfixResourceManager");
        var hotfixResourceManager = GameObject.Instantiate(prefab).GetComponent<HotfixResourceManager>();
        hotfixResourceManager.OnEnterGame = EnterGame;
    }

    /// <summary>
    /// 进入启动游戏
    /// </summary>
    private void EnterGame(AssetBundle dllAssetBundle)
    {
        Debug.Log("app launcher ==> enter game");
        DllAssetBundle = dllAssetBundle;
        var dllBytes = dllAssetBundle.LoadAsset<TextAsset>("Assembly-CSharp.dll.bytes");
        var gameMainAssembly = System.Reflection.Assembly.Load(dllBytes.bytes);
        var gameLauncherType = gameMainAssembly.GetType("GameLauncher");
        var startMethod = gameLauncherType.GetMethod("Start");
        startMethod.Invoke(null, null);
    }
}
