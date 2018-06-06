using System;
using System.Collections.Generic;
using System.Text;

namespace Ku.Core.Extensions.DbMigration
{
    public class DbMigrationOptions
    {
        internal List<string> PocoAssemblys { set; get; } = new List<string>();

        public void AddPocoAssemblys(params string[] assembly)
        {
            if (PocoAssemblys == null)
            {
                PocoAssemblys = new List<string>();
            }
            PocoAssemblys.AddRange(assembly);
        }

    }
}
