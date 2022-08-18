/**
 * 数据模块处理中心
 */

using GameMain.Datas;

namespace GameMain.Modules
{
    public class DataModuleManager
    {
        #region 数据模块
        public UserData UserData { get; private set; }
        public PlayerData PlayerData { get; private set; }
        #endregion

        #region 初始化
        /// <summary>
        /// 构造
        /// 绑定监听事件
        /// </summary>
        public DataModuleManager()
        {
            InitDatas();
            InitEvents();
        }

        /// <summary>
        /// 初始化数据模块
        /// </summary>
        private void InitDatas()
        {
            UserData = new UserData();
            PlayerData = new PlayerData();
        }

        /// <summary>
        /// 绑定事件
        /// </summary>
        private void InitEvents()
        {
        }
        #endregion
    }
}
