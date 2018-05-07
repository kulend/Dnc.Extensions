using System;
using System.Collections.Generic;
using System.Text;

namespace Ku.Core.Extensions.Dapper.Attributes
{
    /// <summary>
    /// 逻辑删除
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class LogicalDeleteAttribute : Attribute
    {
        public string Field { get; set; } = "IsDeleted";

        public object DeletedValue { set; get; } = true;

        public object NormalValue { set; get; } = false;
    }
}
