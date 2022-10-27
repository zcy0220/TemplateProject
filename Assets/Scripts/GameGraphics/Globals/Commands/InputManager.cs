/**
 * Graphic输入管理
 */

using GameBaseFramework.Patterns;

namespace GameGraphics
{
    public class InputManager : CommandManager
    {
        /// <summary>
        /// UIView打开指令接收器
        /// </summary>
        private UIViewOpenCommandReceiver _uiViewOpenCommandReceiver = new UIViewOpenCommandReceiver();

        /// <summary>
        /// 初始化
        /// 添加各类指令接收器
        /// </summary>
        public InputManager()
        {
            BindCommandReceiver<GameLogics.UIViewOpenCommand>(_uiViewOpenCommandReceiver);
        }

        public override void Update()
        {
            base.Update();
            _uiViewOpenCommandReceiver.Update();
        }
    }
}