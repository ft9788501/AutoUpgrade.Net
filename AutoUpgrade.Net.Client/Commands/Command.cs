using System;
using System.Collections.Generic;
using System.Text;

namespace AutoUpgrade.Net.Client.Commands
{
    public abstract class Command
    {
        public abstract string Name { get; }
        public abstract string Code { get; }
        public abstract string Descript { get; }
        public virtual string Value { get; set; }
        internal virtual bool Nullable { get; } = true;
        internal virtual bool NullableCheck(out string msg)
        {
            var result = Nullable || (Value != null);
            msg = result ? null : $"{Name}({Code}): value can not be null!";
            return result;
        }
        public override string ToString()
        {
            return Code + "：" + Name + "(" + Descript + ")";
        }
    }
}
