/**
 * 游戏启动项
 */

using UnityEngine;
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
        /// 初始化
        /// </summary>
        private void Awake()
        {
            _fixedDeltaTime = (int)(Time.fixedDeltaTime * 1000);
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
            else
            {
                EnterGame();
            }
        }

        /// <summary>
        /// 正式进入游戏
        /// </summary>
        private void EnterGame()
        {
            Debug.LogError("进入游戏");
        }

        /// <summary>
        /// Update总入口
        /// </summary>
        private void Update()
        {
            G.UIManager.Update();
            G.UnityObjectManager.Update();
        }

        /// <summary>
        /// FixedUpdate总入口
        /// </summary>
        private void FixedUpdate()
        {
        }
    }
}