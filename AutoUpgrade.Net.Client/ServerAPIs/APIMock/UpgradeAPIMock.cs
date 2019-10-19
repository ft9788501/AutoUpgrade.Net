using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoUpgrade.Net.Client.Json;

namespace AutoUpgrade.Net.Client.ServerAPIs.APIMock
{
    internal class UpgradeAPIMock : IUpgradeAPI
    {
        public bool DownloadFile(FileItem fileItem, string token)
        {
            return true;
        }

        public UpgradeInfo GetUpgradeInfo(string version, string token)
        {
            return new UpgradeInfo()
            {
                UpgradeLog = "this is upgrage log",
                Version = "1.1.1.1",
                Files = new FileItem[]
                {
                    new FileItem()
                    {
                        Length =1,
                        MD5 ="md51",
                        Name ="f1",
                        Url ="url1"
                    },
                    new FileItem()
                    {
                        Length =2,
                        MD5 ="md52",
                        Name ="f2",
                        Url ="url2"
                    }
                }
            };
        }

        public bool IsNeedUpgrade(string version)
        {
            return true;
        }
    }
}
