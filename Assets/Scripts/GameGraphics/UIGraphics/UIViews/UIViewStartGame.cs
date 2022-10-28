/**
 * 游戏开始界面
 */

using System.Collections.Generic;
using GameBaseFramework.Base;

namespace GameGraphics
{
    public class UIViewStartGame : UIViewStartGameBase
    {
        protected override void Enter(object data = null)
        {
            Debuger.Log("UIViewStartGame");
        }

        protected override void OnButtonSingleModeClick()
        {
            Debuger.Log("单人模式");
        }
    }
}
