using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoUpgrade.Net.Client.ServerAPIs
{
    public interface IAuthAPI
    {
        string GetToken(string userName, string password);
    }
}
