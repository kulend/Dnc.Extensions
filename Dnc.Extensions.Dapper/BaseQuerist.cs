using Dnc.Extensions.Dapper.Sql;
using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace Dnc.Extensions.Dapper
{
    public class BaseQuerist<TEntity> where TEntity : class
    {
        protected string TableName { set; get; }

        protected string TableSchema { set; get; }

        protected SQLCreater _creater;

        public BaseQuerist(IOptions<DapperOptions> options)
        {
            var type = typeof(TEntity);

            var attr = type.GetCustomAttribute<TableAttribute>();
            TableName = attr != null ? attr.Name : type.Name;
            TableSchema = attr != null ? attr.Schema : string.Empty;

            _creater = new SQLCreater(options);
            _creater.TableName = TableName;
            _creater.TableSchema = TableSchema;

        }

    }
}
