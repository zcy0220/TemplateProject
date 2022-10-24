/**
 * UI视图基类 
 */

namespace GameGraphics
{
    public class UIViewBase
    {
        /// <summary>
        /// 唯一Id
        /// </summary>
        private static int _uniqueId = 0;
        public int Id { get; private set; }

        /// <summary>
        /// 构造
        /// </summary>
        public UIViewBase()
        {
            Id = ++_uniqueId;
        }
    }
}