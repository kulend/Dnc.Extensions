using Dnc.Extensions.Dapper;
using Dnc.Extensions.Dapper.SqlDialect;
using MySql.Data.MySqlClient;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DapperExtensions
    {
        public static IServiceCollection AddDapper(this IServiceCollection services, Action<DapperOptions> options)
        {
            services.Configure(options);
            services.AddTransient<IDapper, Dnc.Extensions.Dapper.Dapper>();
            return services;
        }
    }

    public static class MySqlDapperOptionsExtensions
    {
        public static DapperOptions UseMySql(this DapperOptions options, string connectionString, int? timeout = null)
        {
            options.DbType = DbType.MySql;
            options.SqlDialect = new MySqlDialect();
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
            DapperFactory.services = app.ApplicationServices;
            return app;
        }

        public static IServiceProvider UseDapper(this IServiceProvider provider)
        {
            DapperFactory.services = provider;
            return provider;
        }
    }
}

