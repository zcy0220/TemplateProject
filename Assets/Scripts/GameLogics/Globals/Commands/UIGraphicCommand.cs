/**
 * UIGraphicCommand
 */

using GameBaseFramework.Base;
using GameBaseFramework.Patterns;

namespace GameLogics
{

    /// <summary>
    /// UI表现基类命令
    /// </summary>
    public class UIGraphicCommand : Command
    {
    }

    /// <summary>
    /// 打开界面指令
    /// </summary>
    public class UIViewOpenCommand : UIGraphicCommand
    {
        /// <summary>
        /// 界面名
        /// </summary>
        public string Name;
        /// <summary>
        /// 参数数据
        /// </summary>
        public object Data;
    }
}
