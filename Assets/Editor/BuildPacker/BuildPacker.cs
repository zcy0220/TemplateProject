/**
 * 包体构建工具
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;

namespace Editor.BuildPacker
{
    public class BuildPacker
    {
        [MenuItem("Tools/BuildPacker/Windows(Release)")]
        public static void BuildWindowsRelease()
        {
        }


        [MenuItem("Tools/BuildPacker/Android(Release)")]
        public static void BuildAndroidRelease()
        {
        }

        
        public static void BuildAPK(bool isDebug)
        {
        }
    }
}
