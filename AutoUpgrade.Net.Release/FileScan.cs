using AutoUpgrade.Net.Json;
using AutoUpgrade.Net.Tools;
using System.IO;

namespace AutoUpgrade.Net.Release
{
    public class FileScan
    {
        /// <summary> 比较结果
        /// </summary>
        public enum CompareResult
        {
            /// <summary>
            /// 正常
            /// </summary>
            Normal = 0,
            /// <summary>
            /// 新增
            /// </summary>
            Filter = 1,
            /// <summary>
            /// 新增
            /// </summary>
            Add = 2,
            /// <summary>
            /// 移除
            /// </summary>
            Remove = 3,
            /// <summary>
            /// 更新
            /// </summary>
            Update = 4
        }
        /// <summary>
        /// 文件名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 文件名
        /// </summary>
        public string FullPath { get; set; }
        /// <summary>
        /// 文件MD5
        /// </summary>
        public string MD5 { get; set; }
        /// <summary>
        /// 文件大小
        /// </summary>
        public long Length { get; set; }
        /// <summary>
        /// 文件版本号
        /// </summary>
        public string Version { get; set; }
        /// <summary>
        /// 文件比对结果
        /// </summary>
        public CompareResult Result { get; set; }
        public FileScan(JsonFileDetail jsonFileDetail)
        {
            this.Name = jsonFileDetail.Name;
            this.MD5 = jsonFileDetail.MD5;
            this.Length = jsonFileDetail.Length;
        }
        public FileScan(string dir, string name, bool calcMD5 = true)
        {
            this.FullPath = Path.Combine(dir, name);
            this.Length = new FileInfo(this.FullPath).Length;
            this.Version = System.Diagnostics.FileVersionInfo.GetVersionInfo(this.FullPath).ProductVersion;
            this.Name = name;
            if (calcMD5)
            {
                this.MD5 = MD5Tools.GetFileMd5(this.FullPath);
            }
        }
        public override string ToString()
        {
            return Name + "-----" + MD5;
        }
    }
}
