/**
 * UI基类
 */

using UnityEngine;

namespace GameMain.Views.UI
{
    public class UIBase
    {
        /// <summary>
        /// 唯一Id
        /// </summary>
        private static int _uniqueId = 0;
        public int Id { get; private set; }

        /// <summary>
        /// 构造
        /// </summary>
        public UIBase()
        {
            Id = ++_uniqueId;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="root"></param>
        public virtual void Init(GameObject root) { }
    }
}
