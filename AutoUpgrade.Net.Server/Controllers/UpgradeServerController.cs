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
        private UploadServer serverUpload = new UploadServer("Upgrade");
        private UpgradeServer upgradeServer = new UpgradeServer("Upgrade");

        [HttpPost("authenticate")]
        public IActionResult Authenticate()
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var authTime = DateTime.UtcNow;
            var expiresAt = authTime.AddDays(7);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
            new Claim(JwtClaimTypes.Audience,"api"),
            new Claim(JwtClaimTypes.Issuer,"YFAPICommomCore2"),
                }),
                Expires = expiresAt,
                SigningCredentials = new SigningCredentials(Startup.symmetricKey, SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);
            return Ok(new
            {
                access_token = tokenString,
                token_type = "Bearer",
                profile = new
                {
                    sid = "1",
                    name = "xxxx",
                    auth_time = new DateTimeOffset(authTime).ToUnixTimeSeconds(),
                    expires_at = new DateTimeOffset(expiresAt).ToUnixTimeSeconds()
                }
            });
        }
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
        [HttpGet("download1")]
        public async Task<IActionResult> Download1()
        {
            return Content("aaaaaaaaaaaaaaa");
        }
        [HttpGet("download2")]
        public async Task<IActionResult> Download2()
        {
            return Content("aaaaaaaaaaaaaaa");
        }
        /// <summary>
        /// 上传文件
        /// </summary>
        /// <remarks>
        /// 分块上传和小文件上传通用:
        /// 大于512k的文件使用分块上传，分块上传的时候头文件中的name值使用分块序号0~N
        /// </remarks>
        /// <returns>返回RespondResult</returns>
        [HttpPost("upload")]
        public async Task<IActionResult> Upload(string uploadDir)
        {
            //这里一定要formFiles这个参数，不然Request.Form.Files就获取不到了(后来发现有时可以有时不行，是.netcore的bug？)
            //发现有的时候name必须为“file”才可以，其他的就不行了
            //后来发现都不是 只是调试的时候过不了而已
            return Json(await serverUpload.Upload(this.Request, uploadDir));
        }
        /// <summary>
        /// 上传分块完成之后执行分块的合并
        /// </summary>
        /// <param name="fileName">合并的文件名</param>
        /// <returns>返回RespondResult</returns>
        [HttpGet("merge")]
        public async Task<IActionResult> Merge(string fileName)
        {
            return Json(await serverUpload.Merge(fileName));
        }
        [HttpDelete("deleteVersion")]
        public async Task<IActionResult> DeleteVersion(string version)
        {
            return await Task.Run(() =>
            {
                return Json(upgradeServer.DeleteVersion(version));
            });
        }
        [HttpDelete("deleteFile")]
        public async Task<IActionResult> DeleteFile(string fileName)
        {
            return await Task.Run(() =>
            {
                return Json(upgradeServer.DeleteFile(fileName));
            });
        }
        [HttpPost("createVersion")]
        public async Task<IActionResult> CreateVersion(JsonReleaseVersion jsonReleaseVersion)
        {
            return await Task.Run(() =>
            {
                return Json(upgradeServer.CreateVersion(jsonReleaseVersion));
            });
        }
        [HttpGet("getVersionList")]
        public async Task<IActionResult> GetVersionList()
        {
            return await Task.Run(() =>
            {
                return Json(upgradeServer.GetVersionList());
            });
        }
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
        /// <summary>
        /// 获取服务器上的文件的版本信息
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        [HttpGet("getFileVersion")]
        public async Task<IActionResult> GetFileVersion(string fileName)
        {
            return await Task.Run(() =>
            {
                return Json(upgradeServer.GetFileVersion(fileName));
            });
        }
    }
}
