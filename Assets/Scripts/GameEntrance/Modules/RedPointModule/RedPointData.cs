/**
 * 红点数据
 */

using GameBaseFramework.Base;
using System.Collections.Generic;

namespace GameMain.Modules
{
    /// <summary>
    /// 红点模块
    /// </summary>
    public enum ERedPointModule
    {
        /*=======Mail=======*/
        Mail,
        MailSystem,
        MailFriend
    }

    /// <summary>
    /// 红点类型
    /// </summary>
    public enum ERedPointType
    {
        /// <summary>
        /// 单纯的是否显示红点
        /// </summary>
        BoolRedPoint,
        /// <summary>
        /// 显示数字型红点
        /// </summary>
        NumRedPoint
    }

    public class RedPointData
    {
        /// <summary>
        /// 红点数量
        /// </summary>
        public DataValue<int> Num = new DataValue<int>(0);
        /// <summary>
        /// 被依赖的列表
        /// </summary>
        public List<RedPointData> BeDepends = new List<RedPointData>();
    }
}
