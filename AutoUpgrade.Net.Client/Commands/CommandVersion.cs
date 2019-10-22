using System;
using System.Collections.Generic;
using System.Text;

namespace AutoUpgrade.Net.Client.Commands
{
    internal class CommandVersion : Command
    {
        public override string Name => "主程序的版本";

        public override string Code => "-vs";

        public override string Descript => "使用这个版本号去请求服务器来判断是否需要升级";

        internal override bool Nullable => false;
    }
}
