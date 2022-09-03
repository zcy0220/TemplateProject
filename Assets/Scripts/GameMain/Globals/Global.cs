/**
 * 全局公用模块
 */

using GameBaseFramework.Event;
using GameUnityFramework.Resource;
using GameMain.Modules;
using GameMain.Views.UI;

namespace GameMain
{
    public class G
    {
        #region EventManager
        public static EventManager EventManager = new EventManager();
        #endregion

        #region UIManager
        public static UIManager UIManager = new UIManager();
        #endregion

        #region DataModuleManager
        public static DataModuleManager DataModuleManager = new DataModuleManager();
        #endregion

        #region UnityObjectManager
        public static UnityObjectManager _unityObjectManager;
        public static UnityObjectManager UnityObjectManager
        {
            get
            {
                if (_unityObjectManager == null)
                {
                    _unityObjectManager = new UnityObjectManager("Assets/ArtPack/Pack");
                }
                return _unityObjectManager;
            }
        }
        #endregion
    }
}
