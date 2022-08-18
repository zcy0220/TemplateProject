/**
 * 红点数据
 */

namespace GameMain.Datas
{
    /// <summary>
    /// 红点模块
    /// </summary>
    public enum ERedPointModule
    {
        None,
        Mail
    }

    public class RedPointData
    {
        /// <summary>
        /// 红点模块名称
        /// </summary>
        public string Name;
        /// <summary>
        /// 红点数量
        /// </summary>
        public int Num = -1;
        //public List<string> BeDepends = 
    }
}
