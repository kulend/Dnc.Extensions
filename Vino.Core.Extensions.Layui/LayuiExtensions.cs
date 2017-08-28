using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using Vino.Core.Extensions.Layui;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class LayuiExtensions
    {
        public static IServiceCollection AddLayui(this IServiceCollection services, Action<LayuiOption> options = null)
        {
            //替代系统提供的HtmlHelper
            //services.TryAddTransient(typeof(IHtmlHelper), typeof(LayuiHtmlHelper));
            services.TryAddTransient(typeof(IHtmlHelper<>), typeof(LayuiHtmlHelper<>));
            if (options == null)
            {
                options = (opt => {
                });
            }
            services.Configure(options);
            return services;
        }
    }
}
