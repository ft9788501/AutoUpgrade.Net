using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoUpgrade.Net.Client.Json
{
    public class UpgradeInfo
    {
        public string Version { get; set; }

        public FileItem[] Files { get; set; }

        public string UpgradeLog { get; set; }
    }
}
