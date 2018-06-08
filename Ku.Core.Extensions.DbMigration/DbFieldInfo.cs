using System;
using System.Collections.Generic;
using System.Text;

namespace Ku.Core.Extensions.DbMigration
{
    public class DbFieldInfo
    {
        public string TableName { set; get; }

        public string FieldName { set; get; }

        public string DataType { set; get; }

        public long? Length { set; get; }

        public long? NumLength { set; get; }

        public long? Scale { set; get; }

        public bool Nullable { set; get; }

        public bool Increment { set; get; }

        public string DefValue { set; get; }

        public string Comment { set; get; }
    }
}
