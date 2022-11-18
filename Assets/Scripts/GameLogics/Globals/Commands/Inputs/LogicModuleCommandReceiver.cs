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
        private TeamModule _teamModule = new TeamModule();
        #endregion

        /// <summary>
        /// 构造绑定指令回调
        /// </summary>
        public LogicModuleCommandReceiver()
        {
            Bind<LoginCommand>(_loginModule.Login);
            Bind<CreateRoomCommand>(OnCreateRoomCommand);
        }

        #region 组队相关
        private void OnCreateRoomCommand(CreateRoomCommand command)
        {
            var userData = _loginModule.GetUserData();
            _teamModule.CreateRoom();
        }
        #endregion
    }
}
