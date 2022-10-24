/**
 * Graphic输入管理
 */

using GameBaseFramework.Patterns;

namespace GameGraphics
{
    public class InputManager : CommandManager
    {
        /// <summary>
        /// 初始化
        /// 添加各类指令接收器
        /// </summary>
        public InputManager()
        {
            var uiViewOpenCommandReceiver = new UIViewOpenCommandReceiver();
            BindCommandReceiver<GameLogics.UIViewOpenCommand>(uiViewOpenCommandReceiver);
        }
    }
}