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
        #region UIManager
        private static UIManager _uiManager;
        public static UIManager UIManager
        {
            get
            {
                if (_uiManager == null)
                {
                    _uiManager = new UIManager();
                }
                return _uiManager;
            }
        }
        #endregion
        #region EventManager
        private static EventManager _eventManager;
        public static EventManager EventManager
        {
            get
            {
                if (_eventManager == null)
                {
                    _eventManager = new EventManager();
                }
                return _eventManager;
            }
        }
        #endregion
        #region DataModuleManager
        private static DataModuleManager _dataModuleManager;
        public static DataModuleManager DataModuleManager
        {
            get
            {
                if (_dataModuleManager == null)
                {
                    _dataModuleManager = new DataModuleManager();
                }
                return _dataModuleManager;
            }
        }
        #endregion
        #region UnityObjectManager
        public static UnityObjectManager _unityObjectManager;
        public static UnityObjectManager UnityObjectManager
        {
            get
            {
                if (_unityObjectManager == null)
                {
                    _unityObjectManager = new UnityObjectManager();
                }
                return _unityObjectManager;
            }
        }
        #endregion
    }
}
