/**
 * 断点续传
 */

using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using Microsoft.VisualBasic;
public class DownloadHandlerFileRange : DownloadHandlerScript
{
    /// <summary>
    /// 文件保存路径
    /// </summary>
    private string _saveFilePath;
    /// <summary>
    /// 文件临时路径
    /// </summary>
    private string _tempFilePath;
    /// <summary>
    /// 文件出力流
    /// </summary>
    private FileStream _fileStream;
    /// <summary>
    /// 文件总数据长度
    /// </summary>
    private long _totalLength = 0;
    /// <summary>
    /// 当前已下载长度
    /// </summary>
    private long _currentLength = 0;
    /// <summary>
    /// 本次需要下载的数据长度
    /// </summary>
    private long _contentLength = 0;
    /// <summary>
    /// UnityWebRequest
    /// ReceiveData里面有个bug要用到
    /// </summary>
    private UnityWebRequest _unityWebRequest;
    //======================================================================================
    public long CurrentLength { get { return _currentLength; } }
    //======================================================================================

    /// <summary>
    /// 构造初始化
    /// </summary>
    /// <param name="filePath">文件保存路径:下载到tmp文件，完成时重命名</param>
    public DownloadHandlerFileRange(string saveFilePath, UnityWebRequest unityWebRequest) : base(new byte[1024 * 1024])
    {
        _saveFilePath = saveFilePath;
        _tempFilePath = $"{saveFilePath}.temp";
        _fileStream = new FileStream(_tempFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
        _unityWebRequest = unityWebRequest;
    }

    /// <summary>
    /// 清理资源,该方法没办法重写,只能隐藏,如果想要强制中止下载,并清理资源(UnityWebRequest.Dispose()),该方法并不会被调用,这让人很苦恼
    /// </summary>
    public new void Dispose()
    {
        Close();
    }

    /// <summary>
    /// 关闭文件流
    /// </summary>
    public void Close()
    {
        if (_fileStream != null)
        {
            _fileStream.Flush();
            _fileStream.Dispose();
            _fileStream = null;
        }
    }

    /// <summary>
    /// 收到Content-Length标头调用的回调
    /// </summary>
    /// <param name="contentLength"></param>
    protected override void ReceiveContentLengthHeader(ulong contentLength)
    {
        _contentLength = (long)contentLength;
        _totalLength = _contentLength + _currentLength;
    }

    /// <summary>
    /// 从远程服务器收到数据时的回调
    /// </summary>
    /// <param name="data"></param>
    /// <param name="dataLength"></param>
    /// <returns></returns>
    protected override bool ReceiveData(byte[] data, int dataLength)
    {
        if (data == null || dataLength == 0 || _unityWebRequest.responseCode > 400)
        {
            return false;
        }
        _fileStream.Write(data, 0, dataLength);
        _currentLength += dataLength;

        //todo:progress

        return true;
    }

    /// <summary>
    /// 下载完成后清理资源
    /// </summary>
    protected override void CompleteContent()
    {
        base.CompleteContent();
        Close();
        
        if (_contentLength <= 0)
        {
            Debug.LogError("download content length:0");
            return;
        }

        if (File.Exists(_saveFilePath))
        {
            File.Delete(_saveFilePath);
        }

        File.Move(_tempFilePath, _saveFilePath);
    }

    /// <summary>
    /// 析构
    /// </summary>
    ~DownloadHandlerFileRange()
    {
        Close();
    }
}
