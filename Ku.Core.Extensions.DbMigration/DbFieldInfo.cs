using System;
using System.Collections.Generic;
using System.Text;

namespace Ku.Core.Extensions.DbMigration
{
    public class DbFieldInfo
    {
        public string Name { set; get; }

        public string DataType { set; get; }

        public bool Nullable { set; get; }

        public bool AutoIncrement { set; get; }

        public string DefValue { set; get; }

        public string Comment { set; get; }

        public bool IsKey { set; get; }
    }
}
