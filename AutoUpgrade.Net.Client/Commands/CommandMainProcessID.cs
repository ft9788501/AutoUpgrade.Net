using System;
using System.Collections.Generic;
using System.Text;

namespace AutoUpgrade.Net.Client.Commands
{
    internal class CommandMainProcessID : Command
    {
        public override string Name => "主程序的进程ID";

        public override string Code => "-pid";

        public override string Descript => "使用这个ID来判断主程序是否开启/关闭";

        internal override bool Nullable => false;
        internal override bool NullableCheck(out string msg)
        {
            msg = null;
            var result = Nullable || (Value != null);
            if (result)
            {
                if (int.TryParse(Value, out int proecssID))
                {
                    return true;
                }
                else
                {
                    msg = $"{Name}({Code}): proecssID format error!";
                    return false;
                }
            }
            else
            {
                msg = $"{Name}({Code}): value can not be null!";
                return false;
            }
        }
    }
}
