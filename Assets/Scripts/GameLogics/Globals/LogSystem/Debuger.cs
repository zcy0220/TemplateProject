/**
 * 日志
 */

using System;

namespace GameLogics
{
    public class Debuger
    {
        public static bool EnableLog;
        //==============================================================
        public static Action<string> LogCallback;
        //==============================================================
        
        /// <summary>
        /// Log
        /// </summary>
        /// <param name="msg"></param>
        internal static void Log(string msg)
        {
            if (EnableLog)
            {
                LogCallback?.Invoke(msg);
            }
        }
    }
}
