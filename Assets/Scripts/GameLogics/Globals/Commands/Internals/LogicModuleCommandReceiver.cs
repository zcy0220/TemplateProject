/**
 * 逻辑层模块指令接收器
 * 同时为所有模块的处理中心
 */

using GameLogics.Modules;
using GameBaseFramework.Patterns;

namespace GameLogics
{
    internal class LogicModuleCommandReceiver : CommandReceiver
    {
        #region 所有模块定义
        private LoginModule _loginModule = new LoginModule();
        #endregion

        /// <summary>
        /// 构造绑定指令回调
        /// </summary>
        public LogicModuleCommandReceiver()
        {
            Bind<LoginCommand>(OnLoginCommandCallback);
        }

        /// <summary>
        /// 登录指令回调
        /// </summary>
        /// <param name="command"></param>
        public void OnLoginCommandCallback(LoginCommand command)
        {
            //_loginModule.Login()
        }
    }
}
