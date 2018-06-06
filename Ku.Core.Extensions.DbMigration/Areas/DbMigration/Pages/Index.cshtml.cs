using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Ku.Core.Extensions.DbMigration.AssemblyLocator;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ku.Core.Extensions.DbMigration.DbMigration.Pages
{
    [IgnoreAntiforgeryToken(Order = 1001)]
    public class IndexModel : PageModel
    {
        private readonly IAssemblyLocator _locator;
        private static IList<Assembly> Assemblies;


        public IndexModel(IAssemblyLocator locator)
        {
            _locator = locator;
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

        private string GetFieldType(PropertyInfo property)
        {
            var type = GetRealType(property.PropertyType);
            if (type == typeof(string))
            {
                return "varchar";
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
            else
            {
                return "varchar";
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
    }

    public class PocoField
    {
        public string Name { set; get; }

        public string Description { set; get; }

        public string Type { set; get; }

        public bool IsKey { set; get; }

        public bool Nullable { set; get; }
    }
}