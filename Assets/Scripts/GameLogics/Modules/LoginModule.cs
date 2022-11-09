/**
 * 登录模块
 */

using GameLogics.Datas;

namespace GameLogics.Modules
{
    internal class LoginModule
    {
        /// <summary>
        /// 用户数据
        /// </summary>
        private UserData _userData = new UserData();

        /// <summary>
        /// 登录
        /// </summary>
        public void Login(LoginCommand command)
        {
            _userData.UserId = command.UserId;
            MainLogic.Output(new UIViewOpenCommand() { Name = "UIViewStartGame" });
        }
    }
}
