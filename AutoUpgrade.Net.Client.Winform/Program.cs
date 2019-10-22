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
            var asd = APIProvider.AuthAPI.GetToken("4MfCH65CufRztreOkao2kgCWH1cEe59z", "OZUegy8FE6vwT4EpeZxxOcp1RHswF2CB").Result;
            MainStart.Start(args, () =>
             {
                 if (CommandProvider.UpgraderVisible)
                 {
                     Application.EnableVisualStyles();
                     Application.SetCompatibleTextRenderingDefault(false);
                     Application.Run(new Form1());
                 }
                 else
                 {
                     Application.Run();
                 }
             });
            Console.WriteLine("-unupgradable");
        }
    }
}
