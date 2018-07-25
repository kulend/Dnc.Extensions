using MySql.Data.MySqlClient;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DapperExtensions
    {
        public static IServiceCollection AddDapper(this IServiceCollection services, Action<Ku.Core.Extensions.Dapper.DapperOptions> options)
        {
            services.Configure(options);
            services.AddTransient<Ku.Core.Extensions.Dapper.IDapper, Ku.Core.Extensions.Dapper.Dapper>();
            return services;
        }
    }

    public static class MySqlDapperOptionsExtensions
    {
        public static Ku.Core.Extensions.Dapper.DapperOptions UseMySql(this Ku.Core.Extensions.Dapper.DapperOptions options, string connectionString, int? timeout = null)
        {
            options.DbType = Ku.Core.Extensions.Dapper.DbType.MySql;
            options.SqlDialect = new Ku.Core.Extensions.Dapper.SqlDialect.MySqlDialect();
            options.DbConnection = () => new MySqlConnection(connectionString);
            options.Timeout = null;
            return options;
        }
    }
}

namespace Microsoft.AspNetCore.Builder
{
    public static class DapperBuilderExtensions
    {
        public static IApplicationBuilder UseDapper(this IApplicationBuilder app)
        {
            Ku.Core.Extensions.Dapper.DapperFactory.services = app.ApplicationServices;
            return app;
        }

        public static IServiceProvider UseDapper(this IServiceProvider provider)
        {
            Ku.Core.Extensions.Dapper.DapperFactory.services = provider;
            return provider;
        }
    }
}

