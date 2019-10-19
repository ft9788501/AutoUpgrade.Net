using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoUpgrade.Net.Client.ServerAPIs
{
    public class APIProvider
    {
        public static IAuthAPI AuthAPI { get; } = new APIMock.AuthAPIMock();
        public static IUpgradeAPI UpgradeAPI { get; } = new APIMock.UpgradeAPIMock();
    }
}
