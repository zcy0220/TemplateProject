/**
 * 热更新
 */

using UnityEngine;
using UnityEngine.Networking;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// AssetBundle相关信息
/// </summary>
[Serializable]
public class AssetBundleResourceInfo
{
    /// <summary>
    /// AssetBundle包名
    /// </summary>
    public string Name;
    /// <summary>
    /// AssetBundle大小(KB)
    /// </summary>
    public long Size;
}

/// <summary>
/// AssetBundle版本信息
/// </summary>
[Serializable]
public class AssetBundleVersionConfig
{
    /// <summary>
    /// 版本(时间)
    /// </summary>
    public string Version = "0";
    /// <summary>
    /// AssetBundleResourceInfoList
    /// </summary>
    public List<AssetBundleResourceInfo> AssetBundleInfoList = new List<AssetBundleResourceInfo>();
}

/// <summary>
/// 渠道信息
/// </summary>
[Serializable]
public class ChannelInfo
{
    /// <summary>
    /// 渠道名称
    /// </summary>
    public string ChannelName;
    /// <summary>
    /// 热更新地址
    /// </summary>
    public string HotfixResourceAddress;
}

/// <summary>
/// 渠道配置
/// </summary>
[Serializable]
public class ChannelConfig
{
    public List<ChannelInfo> List = new List<ChannelInfo>();
}

/// <summary>
/// 热更新资源结果
/// </summary>
public enum EHotfixResourceStatus
{
    None,
    StartHotfix,
    EnterGame,
    ConfigError,
    DownloadError
}

public class HotfixResourceManager : MonoBehaviour
{
    /// <summary>
    /// VersionConfig的文件路径(相对于Application.streamingAssetsPath)
    /// </summary>
    public const string AssetBundleVersionConfigFile = "AssetBundleVersionConfig.json";
    /// <summary>
    /// 记录已下载AB的文件路径(相对于Application.persistentDataPath)
    /// </summary>
    public const string AssetBundleCompleteDownloadFile = "AssetBundleCompleteDownload.txt";
    /// <summary>
    /// AssetBundle存放根目录(相对于Application.streamingAssetsPath)
    /// </summary>
    public const string AssetBundlesFolder = "assetbundles";
    /// <summary>
    /// 脚本AB包名
    /// </summary>
    public const string DllAssetBundleName = "scripts";
    ////=============================================================
    /// <summary>
    /// 热更新服务器地址
    /// </summary>
    private string _hotfixResourceAddress;
    /// <summary>
    /// 本地版本配置信息
    /// </summary>
    private AssetBundleVersionConfig _localAssetBundleVersionConfig = new AssetBundleVersionConfig();
    /// <summary>
    /// 服务器版本配置信息
    /// </summary>
    private AssetBundleVersionConfig _serverAssetBundleVersionConfig = new AssetBundleVersionConfig();
    /// <summary>
    /// 当前状态
    /// </summary>
    private EHotfixResourceStatus _status = EHotfixResourceStatus.None;
    /// <summary>
    /// 需要总的下载资源数 = 服务器上与本地所有的AB差异数
    /// </summary>
    private int _totalDownloadCount = 0;
    /// <summary>
    /// 需要总的下载资源大小
    /// </summary>
    private long _totalDownloadSize = 0;
    private long _completeDownloadSize = 0;
    /// <summary>
    /// 需要热更新下载的AssetBundle队列 = _totalDownloadCount - _completeDownloadList
    /// </summary>
    private Queue<string> _needDownloadQueue = new Queue<string>();
    /// <summary>
    /// 正在下载队列数量
    /// </summary>
    private int _loadingAssetBundleCount = 0;
    /// <summary>
    /// 已经下载的列表
    /// </summary>
    private HashSet<string> _completeDownloadList;
    /// <summary>
    /// 已下载的资源写入Stream
    /// </summary>
    private StreamWriter _completeDownloadStreamWriter;
    /// <summary>
    /// AssetBundle信息映射
    /// </summary>
    private Dictionary<string, AssetBundleResourceInfo> _assetBundleInfoDict = new Dictionary<string, AssetBundleResourceInfo>();
    /// <summary>
    /// 当覆盖安装包时，StreamingAssets要比PresistentData新
    /// </summary>
    public bool _isStreamingAssetsVersionNew = false;
    ////=============================================================
    /// <summary>
    /// 状态回调
    /// </summary>
    public Action<AssetBundle>  OnEnterGame { get; set; }
    ////=============================================================
    /*
     * 表现层
     */
    public UnityEngine.UI.Text TextProgress;
    public UnityEngine.UI.Slider SliderProgress;
    public UnityEngine.UI.Text TextTips;

    /// <summary>
    /// 设置进度
    /// </summary>
    /// <param name="progress"></param>
    private void SetProgress(long completeDownloadSize, long totalDownloadSize)
    {
        var progress = 1.0f * completeDownloadSize / totalDownloadSize;
        if (TextProgress != null)
        {
            TextProgress.text = $"{Mathf.FloorToInt(progress * 100)}%";
        }
        if (SliderProgress != null)
        {
            SliderProgress.value = progress;
        }
        if (completeDownloadSize == totalDownloadSize)
        {
            TextTips.text = "更新完成";
        }
        else
        {
            var complete = (1.0f * completeDownloadSize / 1024).ToString("F2");
            var total = (1.0f * totalDownloadSize / 1024).ToString("F2");
            Debug.Log($"{complete}M/{total}M");
            TextTips.text = $"下载资源({complete}M/{total}M)";
        }
    }
    ////=============================================================
    /// <summary>
    /// 初始化
    /// </summary>
    private void Awake()
    {
        StartCoroutine(Startup());
    }

    /// <summary>
    /// 开始检测热更新
    /// </summary>
    public IEnumerator Startup()
    {
        AssetBundle.UnloadAllAssetBundles(true);
        yield return InitHotfixResourceAddress();
        yield return InitLocalAssetBundleVersionConfig();
        yield return InitServerAssetBundleVersionConfig();
        yield return CompareAssetBundleVersionConfig();
    }

    /// <summary>
    /// 获取本地渠道
    /// 拉取服务器渠道配置
    /// 获取对应渠道的热更新地址
    /// </summary>
    /// <returns></returns>
    private IEnumerator InitHotfixResourceAddress()
    {
        var localChannelConfigPath = GetStreamingAssetsFilePath(AppConfig.LocalChannelConfig);
        var channel = "";
        using (var uwr = new UnityWebRequest(localChannelConfigPath))
        {
            uwr.downloadHandler = new DownloadHandlerBuffer();
            uwr.disposeDownloadHandlerOnDispose = true;
            yield return uwr.SendWebRequest();
            if (uwr.result == UnityWebRequest.Result.Success)
            {
                channel = uwr.downloadHandler.text;
            }
            uwr.Dispose();
        }
        using (var uwr = new UnityWebRequest(AppConfig.ServerChanelConfig))
        {
            uwr.downloadHandler = new DownloadHandlerBuffer();
            uwr.disposeDownloadHandlerOnDispose = true;
            yield return uwr.SendWebRequest();
            if (uwr.result == UnityWebRequest.Result.Success)
            {
                var channelConfig = JsonUtility.FromJson<ChannelConfig>(uwr.downloadHandler.text);
                for(int i = 0; i < channelConfig.List.Count; i++)
                {
                    var channelInfo = channelConfig.List[i];
                    if (channelInfo.ChannelName == channel)
                    {
                        _hotfixResourceAddress = channelInfo.HotfixResourceAddress;
                        break;
                    }
                }
            }
            uwr.Dispose();
        }
        Debug.Log($"channel:{channel}");
    }

    /// <summary>
    /// 获得StreamingAssets下的资源文件路径
    /// </summary>
    private string GetStreamingAssetsFilePath(string filePath)
    {
        var path = $"{Application.streamingAssetsPath}/{filePath}";
        if (!path.Contains("://")) path = "file://" + path;
        return path;
    }

    /// <summary>
    /// 获得PresistentData下的资源文件路径
    /// </summary>
    private string GetPresistentDataFilePath(string filePath)
    {
        return $"{Application.persistentDataPath}/{filePath}";
    }

    /// <summary>
    /// 获取真正的本地资源
    /// </summary>
    private string GetLocalFilePath(string filePath)
    {
        if (!_isStreamingAssetsVersionNew)
        {
            var path = GetPresistentDataFilePath(filePath);
            if (File.Exists(path))
            {
                return path;
            }
        }
        return GetStreamingAssetsFilePath(filePath);
    }

    /// <summary>
    /// 路径拼接
    /// </summary>
    private string PathCombine(string path1, string path2)
    {
        return $"{path1}/{path2}";
    }

    /// <summary>
    /// 获取服务器上的资源路径
    /// </summary>
    /// <returns></returns>
    private string GetServerAssetURL(string path)
    {
        return _hotfixResourceAddress + "/" + path;
    }

    /// <summary>
    /// 初始化本地版本配置信息
    /// 当覆盖安装时，StreamingAssets的版本信息要比PresistentData新
    /// </summary>
    private IEnumerator InitLocalAssetBundleVersionConfig()
    {
        var presistentDataConfigPath = GetPresistentDataFilePath(AssetBundleVersionConfigFile);
        using (var uwr = new UnityWebRequest(presistentDataConfigPath))
        {
            uwr.downloadHandler = new DownloadHandlerBuffer();
            uwr.disposeDownloadHandlerOnDispose = true;
            yield return uwr.SendWebRequest();
            if (uwr.result == UnityWebRequest.Result.Success)
            {
                _localAssetBundleVersionConfig = JsonUtility.FromJson<AssetBundleVersionConfig>(uwr.downloadHandler.text);
                Debug.Log($"presistentdata assetbundle versionconfig:{_localAssetBundleVersionConfig.Version}");
            }
            uwr.Dispose();
        }

        var streamingAssetsConfigPath = GetStreamingAssetsFilePath(AssetBundleVersionConfigFile);
        using (var uwr = new UnityWebRequest(streamingAssetsConfigPath))
        {
            uwr.downloadHandler = new DownloadHandlerBuffer();
            uwr.disposeDownloadHandlerOnDispose = true;
            yield return uwr.SendWebRequest();
            if (uwr.result == UnityWebRequest.Result.Success)
            {
                var streamingAssetsVersionConfig = JsonUtility.FromJson<AssetBundleVersionConfig>(uwr.downloadHandler.text);
                Debug.Log($"streamingassets assetbundle versionconfig:{streamingAssetsVersionConfig.Version}");
                if (streamingAssetsVersionConfig != null)
                {
                    var pLocalVersion = long.Parse(_localAssetBundleVersionConfig.Version);
                    var sLocalVersion = long.Parse(streamingAssetsVersionConfig.Version);
                    if (sLocalVersion > pLocalVersion)
                    {
                        _isStreamingAssetsVersionNew = true;
                        _localAssetBundleVersionConfig = streamingAssetsVersionConfig;
                    }
                }
            }
            uwr.Dispose();
        }
        Debug.Log($"local assetbundle versionconfig:{_localAssetBundleVersionConfig.Version}");
    }

    /// <summary>
    /// 初始化服务器版本配置信息
    /// </summary>
    private IEnumerator InitServerAssetBundleVersionConfig()
    {
        var serverVersionConfigPath = GetServerAssetURL(AssetBundleVersionConfigFile);
        using (var uwr = new UnityWebRequest(serverVersionConfigPath))
        {
            uwr.downloadHandler = new DownloadHandlerBuffer();
            uwr.disposeDownloadHandlerOnDispose = true;
            yield return uwr.SendWebRequest();
            if (uwr.result == UnityWebRequest.Result.Success)
            {
                _serverAssetBundleVersionConfig = JsonUtility.FromJson<AssetBundleVersionConfig>(uwr.downloadHandler.text);
                for (int i = 0; i < _serverAssetBundleVersionConfig.AssetBundleInfoList.Count; i++)
                {
                    var info = _serverAssetBundleVersionConfig.AssetBundleInfoList[i];
                    _assetBundleInfoDict.Add(info.Name, info);
                }
            }
            uwr.Dispose();
        }
        Debug.Log($"server assetbundle versionconfig:{_serverAssetBundleVersionConfig.Version}");
    }

    /// <summary>
    /// 对比版本，检测资源
    /// </summary>
    /// <returns></returns>
    private IEnumerator CompareAssetBundleVersionConfig()
    {
        if (_localAssetBundleVersionConfig != null && _serverAssetBundleVersionConfig != null)
        {
            var localVersion = long.Parse(_localAssetBundleVersionConfig.Version);
            var serverVersion = long.Parse(_serverAssetBundleVersionConfig.Version);
            Debug.Log($"local version:{localVersion} ==> server version:{serverVersion}");
            if (serverVersion > localVersion)
            {
                yield return CompareResources();
            }
            else
            {
                _status = EHotfixResourceStatus.EnterGame;
                OnFinished();
            }
        }
        else
        {
            _status = EHotfixResourceStatus.ConfigError;
            OnFinished();
        }
    }

    /// <summary>
    /// 对比资源列表
    /// </summary>
    private IEnumerator CompareResources()
    {
        var localAllAssetBundlesDict = new Dictionary<string, Hash128>();
        var manifestAssetBundlePath = PathCombine(AssetBundlesFolder, AssetBundlesFolder);

        var localManifestAssetBundlePath = GetLocalFilePath(manifestAssetBundlePath);
        using (var uwr = new UnityWebRequest(localManifestAssetBundlePath))
        {
            uwr.downloadHandler = new DownloadHandlerBuffer();
            uwr.disposeDownloadHandlerOnDispose = true;
            yield return uwr.SendWebRequest();
            if (uwr.result == UnityWebRequest.Result.Success)
            {
                var abBytes = uwr.downloadHandler.data;
                var localManifestAssetBundle = AssetBundle.LoadFromMemory(abBytes);
                var localManifest = localManifestAssetBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
                var localAllAssetBundles = new List<string>(localManifest.GetAllAssetBundles());
                foreach (var abName in localAllAssetBundles)
                {
                    localAllAssetBundlesDict.Add(abName, localManifest.GetAssetBundleHash(abName));
                }
                localManifestAssetBundle.Unload(true);
            }
            else
            {
                Debug.LogError($"load local manifest assetbundle:{uwr.result}");
            }
            uwr.Dispose();
        }

        var serverManifestAssetBundlePath = GetServerAssetURL(manifestAssetBundlePath);
        using (var uwr = UnityWebRequestAssetBundle.GetAssetBundle(serverManifestAssetBundlePath))
        {
            yield return uwr.SendWebRequest();
            if (uwr.result == UnityWebRequest.Result.Success)
            {
                var serverManifestAssetBundle = DownloadHandlerAssetBundle.GetContent(uwr);
                var serverManifest = serverManifestAssetBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
                var serverAllAssetBundles = new List<string>(serverManifest.GetAllAssetBundles());
                foreach (var assetBundleName in serverAllAssetBundles)
                {
                    if (localAllAssetBundlesDict.ContainsKey(assetBundleName))
                    {
                        var serverAssetBundleHash = serverManifest.GetAssetBundleHash(assetBundleName);
                        if (localAllAssetBundlesDict[assetBundleName] != serverAssetBundleHash)
                        {
                            AddNeedDownLoadResource(assetBundleName);
                        }
                    }
                    else
                    {
                        AddNeedDownLoadResource(assetBundleName);
                    }
                }
                serverManifestAssetBundle.Unload(true);
                if (_assetBundleInfoDict.TryGetValue(AssetBundlesFolder, out var info))
                {
                    _totalDownloadSize += info.Size;
                }
                if (_completeDownloadList != null)
                {
                    foreach (var assetBundleName in _completeDownloadList)
                    {
                        if (_assetBundleInfoDict.ContainsKey(assetBundleName))
                        {
                            var assetBundleInfo = _assetBundleInfoDict[assetBundleName];
                            _completeDownloadSize += assetBundleInfo.Size;
                        }
                    }
                    SetProgress(_completeDownloadSize, _totalDownloadSize);
                }
            }
            uwr.Dispose();
            UnityWebRequest.ClearCookieCache();
        }
        _status = EHotfixResourceStatus.StartHotfix;
    }

    /// <summary>
    /// 添加需要下载的资源
    /// 对比下当前已下载的文件中是否已经下载
    /// </summary>
    private void AddNeedDownLoadResource(string assetBundleName)
    {
        _totalDownloadCount++;
        Debug.Log($"need downLoad resource:{assetBundleName}");
        if (_assetBundleInfoDict.TryGetValue(assetBundleName, out var info))
        {
            _totalDownloadSize += info.Size;
        }
        if (_completeDownloadList == null)
        {
            var completeDownloadFilePath = GetPresistentDataFilePath(AssetBundleCompleteDownloadFile);
            if (File.Exists(completeDownloadFilePath))
            {
                using (var fileStream = new StreamReader(completeDownloadFilePath))
                {
                    var str = fileStream.ReadToEnd();
                    if (!string.IsNullOrEmpty(str))
                    {
                        _completeDownloadList = new HashSet<string>();
                        var list = str.Split(',');
                        for (int i = 0; i < list.Length; i++)
                        {
                            if (!string.IsNullOrEmpty(list[i]))
                            {
                                _completeDownloadList.Add(list[i]);
                            }
                        }
                    }
                }
            }
        }

        if (_completeDownloadList == null) _completeDownloadList = new HashSet<string>();

        if (!_completeDownloadList.Contains(assetBundleName))
        {
            _needDownloadQueue.Enqueue(assetBundleName);
        }
    }

    /// <summary>
    /// 替换本地的资源
    /// </summary>
    private void ReplaceLocalResource(string assetBundleName, byte[] data)
    {
        try
        {
            Directory.CreateDirectory($"{Application.persistentDataPath}/{AssetBundlesFolder}");
            var assetBundlePath = AssetBundlesFolder + "/" + assetBundleName;
            var path = GetPresistentDataFilePath(assetBundlePath);
            File.WriteAllBytes(path, data);
            if (_completeDownloadStreamWriter == null)
            {
                var completeDownloadFilePath = GetPresistentDataFilePath(AssetBundleCompleteDownloadFile);
                _completeDownloadStreamWriter = new StreamWriter(completeDownloadFilePath, true);
            }
            _completeDownloadStreamWriter.Write(assetBundleName + ",");
            _completeDownloadStreamWriter.Flush();
            if (!_completeDownloadList.Contains(assetBundleName))
            {
                _completeDownloadList.Add(assetBundleName);
            }
            if (_assetBundleInfoDict.TryGetValue(assetBundleName, out var assetBundleInfo))
            {
                _completeDownloadSize += assetBundleInfo.Size;
            }
            SetProgress(_completeDownloadSize, _totalDownloadSize);
        }
        catch (System.Exception e)
        {
            Debug.LogError(e.Message);
        }
    }

    ///// <summary>
    ///// 检测文件并创建对应文件夹
    ///// </summary>
    //public static bool CheckFileAndCreateDirWhenNeeded(string filePath)
    //{
    //    if (string.IsNullOrEmpty(filePath)) return false;
    //    var fileInfo = new FileInfo(filePath);
    //    var dirInfo = fileInfo.Directory;
    //    if (!dirInfo.Exists) Directory.CreateDirectory(dirInfo.FullName);
    //    return true;
    //}

    ///// <summary>
    ///// WriteAllText.
    ///// </summary>
    //private bool WriteAllText(string outFile, string outText)
    //{
    //    try
    //    {
    //        if (!CheckFileAndCreateDirWhenNeeded(outFile)) return false;
    //        if (File.Exists(outFile)) File.SetAttributes(outFile, FileAttributes.Normal);
    //        File.WriteAllText(outFile, outText);
    //        return true;
    //    }
    //    catch (System.Exception e)
    //    {
    //        Debug.LogError($"WriteAllText failed! path = {outFile} with err = {e.Message}");
    //        return false;
    //    }
    //}

    ///// <summary>
    ///// WriteAllBytes
    ///// </summary>
    //private bool WriteAllBytes(string outFile, byte[] outBytes)
    //{
    //    try
    //    {
    //        if (!CheckFileAndCreateDirWhenNeeded(outFile)) return false;
    //        if (File.Exists(outFile)) File.SetAttributes(outFile, FileAttributes.Normal);
    //        File.WriteAllBytes(outFile, outBytes);
    //        return true;
    //    }
    //    catch (System.Exception e)
    //    {
    //        Debug.LogError($"WriteAllBytes failed! path = {outFile} with err = {e.Message}");
    //        return false;
    //    }
    //}

    /// <summary>
    /// 检测下载队列
    /// </summary>
    private void Update()
    {
        while (_needDownloadQueue.Count > 0 && _loadingAssetBundleCount < 10)
        {
            _loadingAssetBundleCount++;
            var assetBundleName = _needDownloadQueue.Dequeue();
            StartCoroutine(DownloadAssetBundle(assetBundleName));
        }

        if (_status == EHotfixResourceStatus.StartHotfix)
        {
            if (_completeDownloadList == null || _completeDownloadList.Count == _totalDownloadCount)
            {
                StartCoroutine(DownloadFinished());
            }
        }
    }

    /// <summary>
    /// 下载AssetBundle
    /// </summary>
    /// <param name="assetBundleName"></param>
    /// <returns></returns>
    private IEnumerator DownloadAssetBundle(string assetBundleName)
    {
        var url = GetServerAssetURL(PathCombine(AssetBundlesFolder, assetBundleName));
        using (var uwr = new UnityWebRequest(url))
        {
            uwr.downloadHandler = new DownloadHandlerBuffer();
            uwr.timeout = 30;
            uwr.disposeDownloadHandlerOnDispose = true;
            yield return uwr.SendWebRequest();
            if (uwr.result == UnityWebRequest.Result.Success)
            {
                ReplaceLocalResource(assetBundleName, uwr.downloadHandler.data);
            }
            else
            {
                //超时或错误重新加入到下载列表中
                _needDownloadQueue.Enqueue(assetBundleName);
                Debug.LogError($"assetbundle name:{assetBundleName} {uwr.error}");
            }
            uwr.Dispose();
            UnityWebRequest.ClearCookieCache();
            _loadingAssetBundleCount--;
        }
    }

    /// <summary>
    /// 资源更新完成
    /// </summary>
    /// <returns></returns>
    private IEnumerator DownloadFinished()
    {
        if (_completeDownloadStreamWriter != null) _completeDownloadStreamWriter.Close();
        File.Delete(GetPresistentDataFilePath(AssetBundleCompleteDownloadFile));

        var assetBundlePath = PathCombine(AssetBundlesFolder, AssetBundlesFolder);
        var url = GetServerAssetURL(assetBundlePath);
        using (var uwr = new UnityWebRequest(url))
        {
            uwr.downloadHandler = new DownloadHandlerBuffer();
            uwr.timeout = 30;
            uwr.disposeDownloadHandlerOnDispose = true;
            yield return uwr.SendWebRequest();
            if (uwr.result == UnityWebRequest.Result.Success)
            {
                var localAssetBundlePath = GetPresistentDataFilePath(assetBundlePath);
                File.WriteAllBytes(localAssetBundlePath, uwr.downloadHandler.data);
                yield return null;

                var versionConfigPath = GetPresistentDataFilePath(AssetBundleVersionConfigFile);
                var text = JsonUtility.ToJson(_serverAssetBundleVersionConfig);
                File.WriteAllText(versionConfigPath, text);

                _isStreamingAssetsVersionNew = false;
                SetProgress(_totalDownloadSize, _totalDownloadSize);
                yield return null;
                _status = EHotfixResourceStatus.EnterGame;
                OnFinished();
            }
        }
    }

    /// <summary>
    /// 结束
    /// </summary>
    private void OnFinished()
    {
        Debug.Log($"hotfix finished ==> {_status}");
        if (_status == EHotfixResourceStatus.EnterGame)
        {
            var assetBundlePath = GetLocalFilePath($"{AssetBundlesFolder}/{DllAssetBundleName}");
            var dllAssetBundle = AssetBundle.LoadFromFile(assetBundlePath);
            OnEnterGame?.Invoke(dllAssetBundle);
        }
        GameObject.Destroy(gameObject);
    }
}
