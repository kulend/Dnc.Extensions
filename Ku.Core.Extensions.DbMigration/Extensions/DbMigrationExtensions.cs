using Ku.Core.Extensions.DbMigration;
using Ku.Core.Extensions.DbMigration.AssemblyLocator;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DbMigrationExtensions
    {
        public static IServiceCollection AddDbMigration(this IServiceCollection self, Action<DbMigrationOptions> options = null)
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
