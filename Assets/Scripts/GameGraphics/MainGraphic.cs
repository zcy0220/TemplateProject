/**
 * 游戏渲染入口
 */

using GameBaseFramework.Patterns;

namespace GameGraphics
{
    public class MainGraphic
    {
        /// <summary>
        /// 输入模块
        /// </summary>
        public static InputManager InputManager { get; private set; } = new InputManager();

        /// <summary>
        /// 输入
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="command"></param>
        internal static void Input<T>(T command) where T : Command
        {
            InputManager.AddCommand(command);
        }

        /// <summary>
        /// 渲染驱动
        /// </summary>
        public static void Update()
        {
            InputManager.Update();
        }
    }
}