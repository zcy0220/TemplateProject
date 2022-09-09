/**
 * 游戏启动项
 */

using UnityEngine;
using GameUnityFramework.Log;
using GameUnityFramework.Resource;

namespace GameMain
{
    public class GameLauncher : MonoBehaviour
    {
        /// <summary>
        /// 固定帧的间隔时间（毫秒）
        /// </summary>
        private int _fixedDeltaTime;
        /// <summary>
        /// 游戏控制
        /// </summary>
        private bool _isRunning = false;

        /// <summary>
        /// 初始化
        /// </summary>
        private void Awake()
        {
            _fixedDeltaTime = (int)(Time.fixedDeltaTime * 1000);
            ResourcePathHelper.ResourcePathPrefix = "Assets/ArtPack/Pack";
            ResourcePathHelper.AssetBundlesFolder = "assetbundles";
            ResourcePathHelper.PackConfigPath = "Assets/ArtPack/Pack/PackConfig.txt";
            DontDestroyOnLoad(gameObject);
        }

        /// <summary>
        /// 开始游戏
        /// </summary>
        private void Start()
        {
            if (GameConfig.AssetBundleEncryptkey.Length > 0)
            {
                AssetBundle.SetAssetBundleDecryptKey(GameConfig.AssetBundleEncryptkey);
            }
            if (GameConfig.IsOpenHotfixResource)
            {
                HotfixResource();
            }
            else
            {
                EnterGame();
            }
        }

        /// <summary>
        /// 热更新
        /// </summary>
        private void HotfixResource()
        {
            var obj = Resources.Load<GameObject>("HotfixResourceManager");
            var hotfixResourceManager = GameObject.Instantiate(obj).GetComponent<HotfixResourceManager>();
            hotfixResourceManager.OnStatusCallback = (status) =>
            {
                switch (status)
                {
                    case EHotfixResourceStatus.StartHotfix:
                        break;
                    case EHotfixResourceStatus.EnterGame:
                        EnterGame();
                        break;
                }
            };
            StartCoroutine(hotfixResourceManager.Startup(GameConfig.HotfixResourceAddress));
        }

        /// <summary>
        /// 正式进入游戏
        /// </summary>
        private void EnterGame()
        {
            _isRunning = true;
            Debuger.Log("进入游戏", "cyan");
            G.UnityObjectManager.SyncGameObjectInstantiate("Tests/Test4.prefab");
        }

        /// <summary>
        /// Update总入口
        /// </summary>
        private void Update()
        {
            if (_isRunning)
            {
                G.UIManager.Update();
                G.UnityObjectManager.Update();
            }
        }

        /// <summary>
        /// FixedUpdate总入口
        /// </summary>
        private void FixedUpdate()
        {
        }
    }
}