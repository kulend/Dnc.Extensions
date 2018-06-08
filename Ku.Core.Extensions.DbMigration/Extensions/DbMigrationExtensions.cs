using Ku.Core.Extensions.DbMigration;
using Ku.Core.Extensions.DbMigration.AssemblyLocator;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ku.Core.Extensions.DbMigration
{
    public static class DbMigrationExtensions
    {
        public static IServiceCollection ConfigureServices(IServiceCollection self, Action<DbMigrationOptions> options = null)
        {
            self.TryAddSingleton<IAssemblyLocator, MyAssemblyLocator>();

            if (options == null)
            {
                options = (opt => {
                });
            }
            self.Configure(options);

            return self;
        }
    }
}
