using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Dnc.Extensions.Dapper
{
    internal class DapperDynamicField
    {
        public string Name { set; get; }

        public object Value { set; get; }

        public bool IsKey { set; get; } = false;
    }
}
