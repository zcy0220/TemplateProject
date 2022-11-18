/**
 * 逻辑层模块相关的指令
 */

using GameBaseFramework.Patterns;

namespace GameLogics
{
    public class LogicModuleCommand : Command { }

    /// <summary>
    /// 登录成功后传入数据的指令
    /// 接SDK或本地模拟都放在GameLogics之外去执行
    /// </summary>
    public class LoginCommand : LogicModuleCommand
    {
        /// <summary>
        /// 用户唯一标志
        /// </summary>
        public string UserId;
    }

    /// <summary>
    /// 创建房间指令
    /// </summary>
    public class CreateRoomCommand : LogicModuleCommand
    {

    }
}
