/**
 * Logic输入管理
 */

using GameBaseFramework.Patterns;

namespace GameLogics
{
    internal class InputManager : CommandManager
    {
        /// <summary>
        /// 逻辑指令接收
        /// </summary>
        private LogicModuleCommandReceiver _logicModuleCommandReceiver = new LogicModuleCommandReceiver();

        /// <summary>
        /// 构造绑定
        /// </summary>
        public InputManager()
        {
            BindCommandReceiver<LoginCommand>(_logicModuleCommandReceiver);
            BindCommandReceiver<CreateRoomCommand>(_logicModuleCommandReceiver);
        }
    }
}
