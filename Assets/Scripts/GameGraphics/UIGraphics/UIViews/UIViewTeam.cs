/**
 * 组队界面
 */

namespace GameGraphics
{
    public class UIViewTeam : UIViewTeamBase
    {
        /// <summary>
        /// 关闭按钮
        /// </summary>
        protected override void OnButtonCloseClick()
        {
            Close();
        }

        /// <summary>
        /// 创建房间
        /// </summary>
        protected override void OnButtonCreateRoomClick()
        {
            GameLogics.MainLogic.Input(new GameLogics.CreateRoomCommand());
        }
    }
}
