using AutoUpgrade.Net.Client.Commands;
using AutoUpgrade.Net.Client.ServerAPIs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutoUpgrade.Net.Client.Winform
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            //args = "-v aaa -pid aaaa".Split(' ');
            if (args.Length == 0 || args.Contains("/?") || args.Contains("?"))
            {
                Console.WriteLine("upgrader started with no args");
                MessageBox.Show(CommandProvider.GetInfo());
            }
            else
            {
                Console.WriteLine($"upgrader started with args");
                CommandProvider.Init(args);
                Console.WriteLine($"args count : {CommandProvider.CommandCount}");
                if (!CommandProvider.NullableCheck(out string msg))
                {
                    Console.Error.WriteLine("-error -- " + msg);
                    return;
                }
                if (APIProvider.UpgradeAPI.IsNeedUpgrade(CommandProvider.Version.Value))
                {
                    Console.WriteLine("-upgradable");
                    Process.GetProcessById(CommandProvider.ProcessID);
                    string token = APIProvider.AuthAPI.GetToken("account", "password");
                    var upgradeInfo = APIProvider.UpgradeAPI.GetUpgradeInfo("vvvvv", token);
                    Console.WriteLine($"upgradable version:{upgradeInfo.Version}");
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new Form1());
                }
                else
                {
                    Console.WriteLine("-unupgradable");
                }
            }
        }
    }
}
