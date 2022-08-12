/**
 * 全局公用模块
 */

using GameBaseFramework.Event;
using GameUnityFramework.Resource;
using GameUnityFramework.Utils;

namespace GameMain
{
    public class G
    {
        #region UnityObjectManager
        public static UnityObjectManager _unityObjectManager;
        public static UnityObjectManager UnityObjectManager
        {
            get
            {
                if (_unityObjectManager == null)
                {
                    _unityObjectManager = new("Assets/ArtPack/Pack");
                }
                return _unityObjectManager;
            }
        }
        #endregion

        #region UIEventManager
        public static EventManager EventManager = new();
        #endregion
    }
}
