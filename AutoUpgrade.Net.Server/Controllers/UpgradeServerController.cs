using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoUpgrade.Net.Core;
using AutoUpgrade.Net.Json;
using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;

namespace AutoUpgrade.Net.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UpgradeServerController : Controller
    {
        private DownloadServer serverDownload = new DownloadServer("Upgrade");
        private UpgradeServer upgradeServer = new UpgradeServer("Upgrade");

        /// <summary>
        /// 用文件名获取下载链接
        /// </summary>
        /// <remarks>
        /// 例子:
        /// Get api/UpgradeService/download?fileName=xxxxxxxxxxxx
        /// </remarks>
        /// <param name="fileName">文件名</param>
        /// <returns>文件下载请求结果</returns> 
        [Authorize]
        [HttpGet("download")]
        public async Task<IActionResult> Download(string fileName)
        {
            Stream stream = await serverDownload.GetDownloadStreamAsync(this.HttpContext, fileName);
            if (stream == null)
            {
                return NotFound();
            }
            return File(stream, this.HttpContext.Response.Headers[HeaderNames.ContentType].ToString(), true);
        }
        [Authorize]
        [HttpGet("checkVersion")]
        public async Task<IActionResult> CheckVersion(string version)
        {
            return await Task.Run(() =>
            {
                return Json(upgradeServer.CheckVersion(version));
            });
        }
        [HttpGet("upgradeable")]
        public async Task<IActionResult> Upgradeable(string version)
        {
            return await Task.Run(() =>
            {
                return Json(upgradeServer.Upgradeable(version));
            });
        }
    }
}
