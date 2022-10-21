/**
 * 游戏上下文
 */

using GameBaseFramework.Patterns;

namespace GameLogics
{
    internal class GameContext
    {
        /// <summary>
        /// 当前游戏状态
        /// </summary>
        private BaseState _gameState;

        /// <summary>
        /// 设置游戏状态
        /// </summary>
        public void SetState(BaseState gameState)
        {
            _gameState?.Exit();

            _gameState = gameState;

            _gameState?.Enter();
        }

        /// <summary>
        /// 逻辑驱动
        /// </summary>
        public void Update()
        {
            _gameState.Update();
        }
    }
}
