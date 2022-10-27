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
        /// 相机指令接收器
        /// </summary>
        private CameraControlCommandReceiver _cameraControlCommandReceiver = new CameraControlCommandReceiver();

        /// <summary>
        /// 初始化
        /// 添加各类指令接收器
        /// </summary>
        public InputManager()
        {
            BindCommandReceiver<GameLogics.UIViewOpenCommand>(_uiViewOpenCommandReceiver);
        }

        /// <summary>
        /// 更新
        /// </summary>
        public override void Update()
        {
            //处理指令
            base.Update();
            _cameraControlCommandReceiver.Update();
            _uiViewOpenCommandReceiver.Update();
        }
    }
}