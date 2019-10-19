using AutoUpgrade.Net.Client.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoUpgrade.Net.Client.ServerAPIs
{
    public interface IUpgradeAPI
    {
        bool IsNeedUpgrade(string version);

        UpgradeInfo GetUpgradeInfo(string version, string token);

        bool DownloadFile(FileItem fileItem, string token);
    }
}
