/**
 * 网络指令接收器
 */

using GameBaseFramework.Networks;

namespace GameLogics
{
    public class NetworkCommandReceiver
    {
        /// <summary>
        /// 网络管理器
        /// </summary>
        private NetManager _netManager;

        /// <summary>
        /// 构造初始化
        /// </summary>
        public NetworkCommandReceiver()
        {
            _netManager = new NetManager(EConnectionType.Kcp);
        }
    }
}
