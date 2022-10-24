/**
 * 打开UI指令接收器
 */

using GameBaseFramework.Patterns;
using GameUnityFramework.Log;

namespace GameGraphics
{
    internal class UIViewOpenCommandReceiver : CommandReceiver
    {
        public override void Execute(Command command)
        {
            var uiGraphicCommand = command as GameLogics.UIViewOpenCommand;
            Debuger.Log(uiGraphicCommand.UIViewName);
        }
    }
}
