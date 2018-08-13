using System;
using System.Collections.Generic;
using System.Text;

namespace Dnc.Extensions.Dapper.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ConditionAttribute : Attribute
    {
        /// <summary>
        /// 字段名，如果为空，则默认为当前属性名
        /// </summary>
        public string Name { set; get; }

        /// <summary>
        /// 操作符
        /// </summary>
        public ConditionOperation Operation { set; get; } = ConditionOperation.Equal;

        /// <summary>
        /// true:当值为NULL时，忽略当前条件
        /// false：当值为NULL时，添加条件==NULL
        /// </summary>
        public WhenNull WhenNull { set; get; } = WhenNull.Ignore;

        /// <summary>
        /// Like用
        /// </summary>
        public string LeftChar { set; get; } = "%";

        /// <summary>
        /// Like用
        /// </summary>
        public string RightChar { set; get; } = "%";
    }

    public enum ConditionOperation
    {
        Equal,
        NotEqual,
        In,
        NotIn,
        Like,
        NotLike,
        Greater,
        GreaterOrEqual,
        Less,
        LessOrEqual,

    }

    public enum WhenNull
    {
        Ignore,
        IsNull,
    }
}
