/**
 * 游戏开始界面
 */

using System.Collections.Generic;
using GameBaseFramework.Base;

namespace GameGraphics
{
    public class UIViewStartGame : UIViewStartGameBase
    {
        /// <summary>
        /// 组队模式
        /// </summary>
        protected override void OnButtonTeamModeClick()
        {
            Open("UIViewTeam", GameLogics.EUIViewOpenType.Overlying);
        }
    }
}
