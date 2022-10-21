/**
 * 数据模块处理中心
 */

using GameBaseFramework.Base;

namespace GameMain.Modules
{
    public class DataModuleManager
    {
        #region 数据
        #endregion

        #region 模块
        private RedPointModule _redPointModule = new RedPointModule();
        #endregion

        #region 初始化
        /// <summary>
        /// 构造
        /// 绑定监听事件
        /// </summary>
        public DataModuleManager()
        {
            InitModules();
            InitDatas();
            InitEvents();
        }

        /// <summary>
        /// 初始化模块
        /// </summary>
        private void InitModules()
        {
        }

        /// <summary>
        /// 初始化数据模块
        /// </summary>
        private void InitDatas()
        {
        }

        /// <summary>
        /// 绑定事件
        /// </summary>
        private void InitEvents()
        {
        }
        #endregion
        #region RedPointModule
        public DataValue<int> GetRedPointNum(ERedPointModule eRedPointModule)
        {
            return _redPointModule.GetRedPointNum(eRedPointModule);
        }

        public void TestAddRedPointNum(ERedPointModule eRedPointModule)
        {
            _redPointModule.AddRedPointNum(eRedPointModule, 1);
        }
        #endregion
    }
}
