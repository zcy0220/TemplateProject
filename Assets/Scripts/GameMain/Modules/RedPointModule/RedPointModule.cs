/**
 * 红点模块
 */

using System.Collections.Generic;
using GameBaseFramework.Base;

namespace GameMain.Modules
{
    public class RedPointModule
    {
        /// <summary>
        /// 红点模块数据映射
        /// </summary>
        public Dictionary<ERedPointModule, RedPointData> _redPointDataDict = new Dictionary<ERedPointModule, RedPointData>();

        /// <summary>
        /// 初始化
        /// </summary>
        public RedPointModule()
        {
            var mailRedPointData = new RedPointData();
            var mailSystemRedPointData = new RedPointData();
            mailSystemRedPointData.BeDepends.Add(mailRedPointData);
            var mailFriendRedPointData = new RedPointData();
            mailFriendRedPointData.BeDepends.Add(mailRedPointData);
            _redPointDataDict.Add(ERedPointModule.Mail, mailRedPointData);
            _redPointDataDict.Add(ERedPointModule.MailSystem, mailSystemRedPointData);
            _redPointDataDict.Add(ERedPointModule.MailFriend, mailFriendRedPointData);
        }

        /// <summary>
        /// 获取指定模块中的红点数
        /// </summary>
        /// <param name="eRedPointModule"></param>
        public DataValue<int> GetRedPointNum(ERedPointModule eRedPointModule)
        {
            if (_redPointDataDict.TryGetValue(eRedPointModule, out var data))
            {
                return data.Num;
            }
            return null;
        }

        /// <summary>
        /// 添加红点数量
        /// </summary>
        /// <param name="eRedPointModule"></param>
        public void AddRedPointNum(ERedPointModule eRedPointModule, int num)
        {
            if (_redPointDataDict.TryGetValue(eRedPointModule, out var data))
            {
                var value = data.Num.Get() + num;
                data.Num.Set(value);
                if (data.BeDepends != null)
                {
                    for (int i = 0; i < data.BeDepends.Count; i++)
                    {
                        var oldNum = data.BeDepends[i].Num.Get();
                        data.BeDepends[i].Num.Set(oldNum + num);
                    }
                }
            }
        }
    }
}
