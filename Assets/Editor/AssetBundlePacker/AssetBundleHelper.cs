/**
 * AssetBundle构建辅助工具
 */

using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace Editor.AssetBundlePacker
{
    public class AssetBundleHelper
    {
        /// <summary>
        /// 需要过滤的资源类型后缀
        /// </summary>
        public static readonly List<string> FilterAssetTyteExtensions = new List<string>()
        {
            "",
            ".cs",
            ".md",
            ".zip",
            ".dll",
            ".meta",
            ".ds_store"
        };

        /// <summary>
        /// 是否为有效的资源路径
        /// </summary>
        public static bool IsValidAssetPath(string path)
        {
            var pathExtension = Path.GetExtension(path);
            if (FilterAssetTyteExtensions.Any(ext => ext.Equals(pathExtension, StringComparison.OrdinalIgnoreCase)))
            {
                return false;
            }
            return true;
        }
    }
}
