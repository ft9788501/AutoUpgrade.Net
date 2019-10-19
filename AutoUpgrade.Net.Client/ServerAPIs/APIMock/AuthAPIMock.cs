using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoUpgrade.Net.Client.ServerAPIs.APIMock
{
    internal class AuthAPIMock : IAuthAPI
    {
        public string GetToken(string userName, string password)
        {
            return "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
        }
    }
}
