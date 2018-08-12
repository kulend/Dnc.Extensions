using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Builder;

namespace Dnc.Extensions.Dapper.Test
{
    public class BaseTest
    {
        protected IServiceProvider serviceProvider;

        public BaseTest()
        {

            IConfigurationBuilder configurationBuilder = new ConfigurationBuilder();

            var Configuration = configurationBuilder.Build();

            IServiceCollection services = new ServiceCollection();


            //string connection = Configuration.GetConnectionString("MysqlDatabase");
            //services.AddDbContext<KuDbContext>(options => options.UseMySql(connection));
            services.AddDapper(options => options.UseMySql("1"));

            var builder = new ContainerBuilder();
            //builder.RegisterModule(new AppModule());

            // 将原本注册在内置 DI 组件中的依赖迁移入 Autofac 中
            builder.Populate(services);

            //构建容器
            var container = builder.Build();
            serviceProvider = new AutofacServiceProvider(container);

            serviceProvider.UseDapper();
        }
    }
}
