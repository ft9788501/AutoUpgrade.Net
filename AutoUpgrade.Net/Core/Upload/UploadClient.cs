using AutoUpgrade.Net.Args;
using AutoUpgrade.Net.Delegates;
using AutoUpgrade.Net.Json;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace AutoUpgrade.Net.Core
{
    /// <summary> 客户端上传
    /// </summary>
    public class UploadClient
    {
        #region 事件
        /// <summary>
        /// 上传进度变化事件
        /// </summary>
        public event ProgressChangedHandler UploadProgressChanged;
        protected virtual void OnUploadProgressChanged(ProgressChangedArgs progressChangedArgs)
        {
            this.UploadProgressChanged?.Invoke(this, progressChangedArgs);
        }
        /// <summary>
        /// 上传速度变化事件
        /// </summary>
        public event SpeedChangedHandler UploadSpeedChanged;
        protected virtual void OnUploadSpeedChanged(SpeedChangedArgs speedChangedArgs)
        {
            this.UploadSpeedChanged?.Invoke(this, speedChangedArgs);
        }
        /// <summary>
        /// 上传完成事件
        /// </summary>
        /// <param name="progress"></param>
        /// <param name="speed"></param>
        public event CompletedHandler UploadCompleted;
        protected virtual void OnUploadCompleted(CompletedArgs completedArgs)
        {
            this.UploadCompleted?.Invoke(this, completedArgs);
        }
        /// <summary>
        /// 上传错误事件
        /// </summary>
        /// <param name="progress"></param>
        /// <param name="speed"></param>
        public event ErrorHandler UploadError;
        protected virtual void OnUploadError(ErrorArgs errorArgs)
        {
            this.UploadError?.Invoke(this, errorArgs);
        }
        #endregion
        #region const
        /// <summary>
        /// 上传缓存大小
        /// </summary>
        private const int UploadBufferSize = 1024 * 4;
        /// <summary>
        /// 分块大小
        /// </summary>
        private const int ChunkSize = 1024 * 512; //分块大小512k
        #endregion
        #region 变量
        /// <summary> 上传的url
        /// </summary>
        private string uploadUrl = string.Empty;
        /// <summary> 分块合并的url(可以为空，如果为空不分块上传)
        /// </summary>
        private string mergeURL = string.Empty;
        #endregion
        #region 属性
        #endregion
        /// <summary>
        /// 文件上传
        /// </summary>
        /// <param name="uploadUrl">上传的url</param>
        /// <param name="mergeURL">分块合并的url(可以为空，如果为空不分块上传)</param>
        public UploadClient(string uploadUrl, string mergeURL = null)
        {
            this.uploadUrl = uploadUrl;
            this.mergeURL = mergeURL;
        }
        public void UploadAsync(string filePath, string dir = null)
        {
            Task.Run(async () =>
            {
                FileInfo fileInfo = new FileInfo(filePath);
                long length = fileInfo.Length;
                if (length > ChunkSize)
                {
                    this.OnUploadCompleted(new CompletedArgs(await UploadLarge(filePath, dir)));
                }
                else
                {
                    this.OnUploadCompleted(new CompletedArgs(await UploadOnce(filePath, dir)));
                }
            });
        }
        public async Task<bool> UploadTaskAsync(string filePath, string dir = null)
        {
            FileInfo fileInfo = new FileInfo(filePath);
            long length = fileInfo.Length;
            if (length > ChunkSize)
            {
                return await UploadLarge(filePath, dir);
            }
            else
            {
                return await UploadOnce(filePath, dir);
            }
        }
        /// <summary>
        /// 一次性上传
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private async Task<bool> UploadOnce(string filePath, string dir)
        {
            using (FileStream fileStream = System.IO.File.OpenRead(filePath))
            using (HttpClient client = new HttpClient(new HttpClientHandler() { UseCookies = false }) { Timeout = TimeSpan.FromSeconds(10) })//若想手动设置Cookie则必须设置UseCookies = false
            {
                MultipartFormDataContent multipartFormDataContent = new MultipartFormDataContent();
                multipartFormDataContent.Add(new ProgressableStreamContent(
                    fileStream,
                    UploadBufferSize,
                    (r, u) => this.OnUploadProgressChanged(new ProgressChangedArgs(r, u, fileStream.Length)),
                    (s) => this.OnUploadSpeedChanged(new SpeedChangedArgs(s))),
                    "file",
                    Path.GetFileName(filePath));
                try
                {
                    var result = await client.PostAsync(new Uri(this.uploadUrl + "?uploadDir=" + dir), multipartFormDataContent);
                    if (result.IsSuccessStatusCode)
                    {
                        JsonRespondResult respondResult = JsonConvert.DeserializeObject<JsonRespondResult>(await result.Content.ReadAsStringAsync());
                        if (!respondResult.Result)
                        {
                            this.OnUploadError(new ErrorArgs(respondResult.Message));
                        }
                        return respondResult.Result;
                    }
                }
                catch (Exception ex)
                {
                    this.OnUploadError(new ErrorArgs(ex.Message));
                }
            }
            return false;
        }
        /// <summary>
        /// 上传大文件
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private async Task<bool> UploadLarge(string filePath, string dir)
        {
            try
            {
                bool success = true;
                using (FileStream fileStream = System.IO.File.OpenRead(filePath))
                {
                    long uploaded = 0;
                    string mergeDir = "Merge";
                    string chunkPath = Path.Combine(mergeDir, "chunk");
                    if (!Directory.Exists(mergeDir))
                    {
                        Directory.CreateDirectory(mergeDir);
                    }
                    for (long i = 0; i < fileStream.Length; i += ChunkSize)
                    {
                        fileStream.Position = i;
                        using (Stream writeStream = System.IO.File.Create(chunkPath))
                        {
                            byte[] buffer = new byte[ChunkSize];
                            int readLength = fileStream.Read(buffer, 0, buffer.Length);
                            writeStream.Write(buffer, 0, readLength);
                        }
                        using (Stream readStream = System.IO.File.OpenRead(chunkPath))
                        {
                            long readLength = readStream.Length;
                            if (!await UploadChunk(readStream, filePath, (i / ChunkSize).ToString(), uploaded, fileStream.Length, dir))
                            {
                                success = false;
                                break;
                            }
                            uploaded += readLength;
                        }
                    }
                    try
                    {
                        System.IO.Directory.Delete(mergeDir, true);
                    }
                    catch (Exception ex)
                    {
                        this.OnUploadError(new ErrorArgs(ex.Message));
                    }
                }
                if (success && await this.Merge(filePath, dir))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                this.OnUploadError(new ErrorArgs(ex.Message));
                return false;
            }
        }
        /// <summary>
        /// 上传分块
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="stream"></param>
        /// <param name="filePath"></param>
        /// <param name="chunkName"></param>
        /// <param name="tryCount"></param>
        /// <returns></returns>
        private async Task<bool> UploadChunk(Stream stream, string filePath, string chunkName, long uploaded, long totalLength, string dir, int tryCount = 0)
        {
            bool success = false;
            long loaded = 0;
            using (HttpClient client = new HttpClient(new HttpClientHandler() { UseCookies = false }) { Timeout = TimeSpan.FromSeconds(10) }) //若想手动设置Cookie则必须设置UseCookies = false
            {
                MultipartFormDataContent multipartFormDataContent = new MultipartFormDataContent();
                multipartFormDataContent.Add(new ProgressableStreamContent(
                    stream,
                    UploadBufferSize, (r, u) =>
                    {
                        loaded = uploaded + u;
                        this.OnUploadProgressChanged(new ProgressChangedArgs(r, uploaded + u, totalLength));
                    },
                    (s) => this.OnUploadSpeedChanged(new SpeedChangedArgs(s))),
                    chunkName,
                    Path.GetFileName(filePath));
                try
                {
                    var result = await client.PostAsync(new Uri(this.uploadUrl + "?uploadDir=" + dir), multipartFormDataContent);
                    if (result.IsSuccessStatusCode)
                    {
                        JsonRespondResult respondResult = JsonConvert.DeserializeObject<JsonRespondResult>(await result.Content.ReadAsStringAsync());
                        success = respondResult.Result;
                        if (!respondResult.Result)
                        {
                            this.OnUploadError(new ErrorArgs(respondResult.Message));
                        }
                    }
                }
                catch (Exception ex)
                {
                    this.OnUploadError(new ErrorArgs(ex.Message));
                }
            }
            if (!success && tryCount < 3)
            {
                this.OnUploadProgressChanged(new ProgressChangedArgs((int)(uploaded - loaded), uploaded, totalLength));
                success |= await UploadChunk(stream, filePath, chunkName, uploaded, totalLength, dir, tryCount + 1);
            }
            return success;
        }
        /// <summary>
        /// 合并上传的分块
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private async Task<bool> Merge(string filePath, string dir)
        {
            using (HttpClient httpClient = new HttpClient() { Timeout = TimeSpan.FromSeconds(10) })
            {
                try
                {
                    string fileName = Path.GetFileName(filePath);
                    if (!string.IsNullOrEmpty(dir))
                    {
                        fileName = Path.Combine(dir, Path.GetFileName(filePath)).Replace(Path.DirectorySeparatorChar, '/');
                    }
                    var result = await httpClient.GetAsync(this.mergeURL + "?fileName=" + fileName);
                    if (result.IsSuccessStatusCode)
                    {
                        JsonRespondResult respondResult = JsonConvert.DeserializeObject<JsonRespondResult>(await result.Content.ReadAsStringAsync());
                        if (!respondResult.Result)
                        {
                            this.OnUploadError(new ErrorArgs(respondResult.Message));
                        }
                        return respondResult.Result;
                    }
                }
                catch (Exception ex)
                {
                    this.OnUploadError(new ErrorArgs(ex.Message));
                }
            }
            return false;
        }
        private class ProgressableStreamContent : StreamContent
        {
            private Stream content = null;
            private int bufferSize = 4096;
            private Action<int, long> uploaded = null;
            private Action<float> speed = null;
            public ProgressableStreamContent(Stream content, int bufferSize, Action<int, long> uploaded, Action<float> speed) : base(content, bufferSize)
            {
                this.content = content;
                this.bufferSize = bufferSize;
                this.uploaded = uploaded;
                this.speed = speed;
            }

            protected override Task SerializeToStreamAsync(Stream stream, TransportContext context)
            {
                return Task.Run(() =>
                {
                    var buffer = new Byte[this.bufferSize];
                    long upload = 0;
                    int readLength = 0;
                    decimal downloadSpeed = 0;//下载速度
                    var beginSecond = DateTime.Now.Second;//当前时间秒
                    while (content.CanRead && (readLength = content.Read(buffer, 0, buffer.Length)) != 0)
                    {
                        upload += readLength;
                        downloadSpeed += readLength;
                        stream.Write(buffer, 0, readLength);
                        var endSecond = DateTime.Now.Second;
                        if (endSecond != beginSecond)//计算速度
                        {
                            downloadSpeed = downloadSpeed / (endSecond - beginSecond);
                            this.speed?.Invoke((float)(downloadSpeed / 1024));
                            beginSecond = DateTime.Now.Second;
                            downloadSpeed = 0;//清空
                        }
                        this.uploaded?.Invoke(readLength, upload);
                    }
                });
            }
        }
    }
}
