/**
 * 游戏逻辑入口
 */

using GameBaseFramework.Base;
using GameBaseFramework.Patterns;

namespace GameLogics
{
    public class MainLogic
    {
        /// <summary>
        /// 输入模块
        /// </summary>
        private static InputManager _inputManager;
        /// <summary>
        /// 游戏上下文（即主体）
        /// </summary>
        private static GameContext _gameContext;
        /// <summary>
        /// 输出模块
        /// </summary>
        private static CommandManager _outputManager;

        /// <summary>
        /// 初始化游戏
        /// </summary>
        public static void StartGame()
        {
            Debuger.Log("main logic ==> start game");
            _inputManager = new InputManager();
            //_gameContext = new GameContext();
            //_gameContext.SetState(new StartGameState());
            _inputManager.AddCommand(new LoginCommand());
        }

        /// <summary>
        /// 绑定渲染
        /// </summary>
        public static void Bind(CommandManager commandManager)
        {
            _outputManager = commandManager;
        }

        /// <summary>
        /// 输入
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="command"></param>
        public static void Input<T>(T command) where T : Command
        {
            _inputManager.AddCommand(command);
        }

        /// <summary>
        /// 输出
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="command"></param>
        internal static void Output<T>(T command) where T : Command
        {
            _outputManager?.AddCommand(command);
        }

        /// <summary>
        /// 逻辑驱动
        /// </summary>
        public static void Update()
        {
            _inputManager.Update();
            //_gameContext.Update();
        }
    }
}