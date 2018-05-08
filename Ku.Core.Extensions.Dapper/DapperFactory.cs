using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ku.Core.Extensions.Dapper
{
    /// <summary>
    /// Dapper创建工厂
    /// </summary>
    public static class DapperFactory
    {
        internal static IServiceProvider services;

        /// <summary>
        /// 创建新的Dapper实例
        /// </summary>
        /// <returns>IDapper</returns>
        public static IDapper Create()
        {
            return services.GetService<IDapper>();
        }

        /// <summary>
        /// 创建新的Dapper实例并开启事务
        /// </summary>
        /// <returns>IDapper</returns>
        /// <remarks>开启事务后可调用IDapper.Commit()提交事务，调用IDapper.Rollback()回滚事务</remarks>
        public static IDapper CreateWithTrans()
        {
            var dapper = services.GetService<IDapper>();
            dapper.BeginTrans();
            return dapper;
        }
    }
}
