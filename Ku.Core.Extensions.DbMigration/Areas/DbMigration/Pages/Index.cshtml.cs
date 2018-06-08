using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Ku.Core.Extensions.DbMigration.AssemblyLocator;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;

namespace Ku.Core.Extensions.DbMigration.DbMigration.Pages
{
    [IgnoreAntiforgeryToken(Order = 1001)]
    public class IndexModel : PageModel
    {
        private readonly IAssemblyLocator _locator;
        private static IList<Assembly> Assemblies;
        private readonly DbMigrationOptions _options;
        private readonly IDbTool _dbTool;

        public IndexModel(IAssemblyLocator locator, IOptions<DbMigrationOptions> option, IDbTool dbTool)
        {
            _locator = locator;
            _options = option.Value;
            _dbTool = dbTool;
        }

        public List<TypeInfo> Types { set; get; }

        public void OnGet()
        {
            Types = new List<TypeInfo>();
            Assemblies = _locator.GetAssemblies();
            foreach (var x in Assemblies)
            {
                //查找带有TableAttribute的类
                var tps = x.DefinedTypes.Where(y => y.GetCustomAttribute(typeof(TableAttribute), true) != null);
                Types.AddRange(tps);
            }
        }

        /// <summary>
        /// 取得列表数据
        /// </summary>
        public async Task<IActionResult> OnGetDataAsync(string assembly, string poco)
        {
            var asm = Assemblies.Single(x => x.FullName.Equals(assembly));
            var type = asm.DefinedTypes.Single(x => x.FullName.Equals(poco));
            var properties = type.GetProperties(BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Public).Where(x => !x.GetAccessors()[0].IsVirtual && x.GetCustomAttribute<NotMappedAttribute>() == null);

            var items = new List<PocoField>();

            foreach (var property in properties)
            {
                var field = new PocoField();
                field.Name = property.Name;
                var attr = property.GetCustomAttribute<DisplayAttribute>();
                if (attr != null)
                {
                    field.Description = attr.Name;
                }

                //字段类型
                field.Type = GetFieldType(property);

                //是否可空
                field.Nullable = IsNullableField(property);

                //是否主键
                field.IsKey = property.GetCustomAttribute<KeyAttribute>() != null;


                items.Add(field);
            }

            //取得DB端信息
            var tableName = type.GetCustomAttribute<TableAttribute>()?.Name;

            var fields = await _dbTool.GetTableFieldsAsync(tableName);
            foreach (var item in items)
            {
                List<string> diff = new List<string>();
                //开始判断是否有差异
                var field = fields.SingleOrDefault(x=>x.FieldName.Equals(item.Name, StringComparison.OrdinalIgnoreCase));
                if (field == null)
                {
                    //数据库没有该字段
                    diff.Add("new");
                }
                else
                {
                    //数据库有该字段
                    //类型处理
                    var fieldType = GetFieldType(field);
                    item.DbDescription = field.Comment;
                    item.DbNullable = field.Nullable;
                    item.DbType = fieldType;
                }
            }

            return new DbMigrationJsonResult(new LayuiPagerResult<PocoField>(items, 1, 999, items.Count));
        }

        private bool IsNullableField(PropertyInfo property)
        {
            if (property.PropertyType == typeof(string))
            {
                if (property.GetCustomAttribute<RequiredAttribute>() != null)
                {
                    return false;
                }
                return true;
            }
            else
            {
                return IsNullableType(property.PropertyType);
            }
        }

        /// <summary>
        /// 获取字段类型
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        private string GetFieldType(PropertyInfo property)
        {
            var type = GetRealType(property.PropertyType);
            if (type == typeof(string))
            {
                //取得长度
                var maxLength = 0;
                var stringLengthAttribute = property.GetCustomAttribute<StringLengthAttribute>();
                if (stringLengthAttribute != null)
                {
                    maxLength = stringLengthAttribute.MaximumLength;
                }
                var maxLengthAttribute = property.GetCustomAttribute<MaxLengthAttribute>();
                if (maxLengthAttribute != null)
                {
                    var vmax = maxLengthAttribute.Length;
                    if (maxLength < vmax) maxLength = vmax;
                }
                if (maxLength == 0)
                {
                    return "longtext";
                }
                else
                {
                    return $"varchar({maxLength})";
                }
            }
            else if (type == typeof(bool))
            {
                return "bit";
            }
            else if (type == typeof(int) || type == typeof(Int32))
            {
                return "int";
            }
            else if (type == typeof(short) || type == typeof(Int16))
            {
                return "smallint";
            }
            else if (type == typeof(long) || type == typeof(Int64))
            {
                return "bigint";
            }
            else if (type == typeof(decimal))
            {
                return "decimal(65, 30)";
            }
            else if (type == typeof(float))
            {
                return "float";
            }
            else if (type == typeof(double))
            {
                return "double";
            }
            else if (type == typeof(DateTime))
            {
                return "datetime(6)";
            }
            else if (type.IsEnum)
            {
                return "short";
            }
            else
            {
                return "";
            }
        }

        private bool IsNullableType(Type theType)
        {
            return (theType.IsGenericType && theType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)));
        }

        private Type GetRealType(Type theType)
        {
            if (IsNullableType(theType))
            {
                return theType.GetGenericArguments()[0];
            }
            return theType;
        }

        private string GetFieldType(DbFieldInfo field)
        {
            if (field.DataType.Equals("varchar"))
            {
                return $"varchar({field.Length})";
            }
            else if (field.DataType.Equals("char"))
            {
                return $"char({field.Length})";
            }
            else
            {
                return field.DataType;
            }
        }
    }

    public class PocoField
    {
        public string Name { set; get; }

        public string Description { set; get; }
        public string DbDescription { set; get; }

        public string Type { set; get; }
        public string DbType { set; get; }

        public bool IsKey { set; get; }
        public bool DbIsKey { set; get; }

        public bool Nullable { set; get; }
        public bool DbNullable { set; get; }

        public string Diff { set; get; }
    }
}