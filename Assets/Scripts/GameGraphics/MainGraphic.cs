/**
 * 游戏渲染入口
 */

using GameBaseFramework.Patterns;
using GameUnityFramework.Resource;

namespace GameGraphics
{
    public class MainGraphic
    {
        #region 全局模块
        /// <summary>
        /// 输入模块
        /// </summary>
        public static InputManager InputManager { get; private set; }
        /// <summary>
        /// 资源加载模块
        /// </summary>
        public static UnityObjectManager UnityObjectManager { get; private set; }
        #endregion

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

        /// <summary>
        /// 初始化
        /// </summary>
        public static void Init()
        {
            UnityObjectManager = new UnityObjectManager();
            InputManager = new InputManager();
        }
    }
}