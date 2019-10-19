using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoUpgrade.Net.Client.Commands
{
    public static class CommandProvider
    {
        public static Command Version { get; } = new CommandVersion();
        private static Command MainProcessID { get; } = new CommandMainProcessID();
        public static int ProcessID => int.Parse(CommandProvider.MainProcessID.Value);

        public static int CommandCount => AllCommands.Length;
        private static Command[] AllCommands => typeof(CommandProvider)
            .GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)
            .Where(p => p.PropertyType == typeof(Command) || p.PropertyType.IsSubclassOf(typeof(Command)))
            .Select(p => p.GetValue(null))
            .OfType<Command>()
            .ToArray();
        public static void Init(string[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].Substring(0, 1) == "-" && i + 1 < args.Length && args[i + 1].Substring(0, 1) != "-")
                {
                    string code = args[i++];
                    string value = args[i];
                    Command command = AllCommands.FirstOrDefault(c => c.Code == code);
                    if (command != null)
                    {
                        command.Value = value;
                    }
                }
            }
        }
        public static bool NullableCheck(out string msg)
        {
            List<string> msgs = new List<string>();
            bool result = true;
            foreach (var command in AllCommands)
            {
                result &= command.NullableCheck(out string checkMsg);
                if (checkMsg != null)
                {
                    msgs.Add(checkMsg);
                }
            }
            msg = string.Join(";", msgs);
            return result;
        }
        public static string GetInfo()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("请输入参数命令打开本程序：Process.Start+Arguments");
            stringBuilder.AppendLine("输出指令：");
            stringBuilder.AppendLine("-error：出现错误");
            stringBuilder.AppendLine("-upgradable：可升级状态，通常情况会进入升级流程");
            stringBuilder.AppendLine("-unupgradable：不可升级状态，通常说明这是最新的版本");
            stringBuilder.AppendLine("输入命令：");
            stringBuilder.AppendLine("命令代码介绍：");
            stringBuilder.AppendLine(string.Join("\r\n", AllCommands.Select(c => c.ToString())));
            return stringBuilder.ToString();
        }
    }
}

