using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Dnc.Extensions.Dapper.Sql
{
    internal static class ExpressionHelper
    {


        //public static object GetValue(Expression member)
        //{
        //    var objectMember = Expression.Convert(member, typeof(object));
        //    var getterLambda = Expression.Lambda<Func<object>>(objectMember);
        //    var getter = getterLambda.Compile();
        //    return getter();
        //}

        //public static string GetSqlOperator(ExpressionType type)
        //{
        //    switch (type)
        //    {
        //        case ExpressionType.Equal:
        //        case ExpressionType.Not:
        //        case ExpressionType.MemberAccess:
        //            return "=";

        //        case ExpressionType.NotEqual:
        //            return "!=";

        //        case ExpressionType.LessThan:
        //            return "<";

        //        case ExpressionType.LessThanOrEqual:
        //            return "<=";

        //        case ExpressionType.GreaterThan:
        //            return ">";

        //        case ExpressionType.GreaterThanOrEqual:
        //            return ">=";

        //        case ExpressionType.AndAlso:
        //        case ExpressionType.And:
        //            return "AND";

        //        case ExpressionType.Or:
        //        case ExpressionType.OrElse:
        //            return "OR";

        //        case ExpressionType.Default:
        //            return string.Empty;

        //        default:
        //            throw new NotSupportedException(type + " isn't supported");
        //    }
        //}

        //public static string GetMethodCallSqlOperator(string methodName)
        //{
        //    switch (methodName)
        //    {
        //        case "Contains":
        //            return "IN";

        //        case "Any":
        //        case "All":
        //            return methodName.ToUpperInvariant();

        //        default:
        //            throw new NotSupportedException(methodName + " isn't supported");
        //    }
        //}

        //public static BinaryExpression GetBinaryExpression(Expression expression)
        //{
        //    var binaryExpression = expression as BinaryExpression;
        //    var body = binaryExpression ?? Expression.MakeBinary(ExpressionType.Equal, expression, expression.NodeType == ExpressionType.Not ? Expression.Constant(false) : Expression.Constant(true));
        //    return body;
        //}

        //public static Func<PropertyInfo, bool> GetPrimitivePropertiesPredicate()
        //{
        //    return p => p.CanWrite && (p.PropertyType.IsValueType || p.PropertyType == typeof(string) || p.PropertyType == typeof(byte[]));
        //}

        //public static object GetValuesFromCollection(MethodCallExpression callExpr)
        //{
        //    var expr = callExpr.Object as MemberExpression;

        //    if (!(expr?.Expression is ConstantExpression))
        //        throw new NotSupportedException(callExpr.Method.Name + " isn't supported");

        //    var constExpr = (ConstantExpression)expr.Expression;

        //    var constExprType = constExpr.Value.GetType();
        //    return constExprType.GetField(expr.Member.Name).GetValue(constExpr.Value);
        //}


        //public static MemberExpression GetMemberExpression(Expression expression)
        //{
        //    switch (expression)
        //    {
        //        case MethodCallExpression expr:
        //            return (MemberExpression)expr.Arguments[0];

        //        case MemberExpression memberExpression:
        //            return memberExpression;

        //        case UnaryExpression unaryExpression:
        //            return (MemberExpression)unaryExpression.Operand;

        //        case BinaryExpression binaryExpression:
        //            var binaryExpr = binaryExpression;

        //            if (binaryExpr.Left is UnaryExpression left)
        //                return (MemberExpression)left.Operand;

        //            //should we take care if right operation is memberaccess, not left?
        //            return (MemberExpression)binaryExpr.Left;

        //        case LambdaExpression expression1:
        //            var lambdaExpression = expression1;

        //            switch (lambdaExpression.Body)
        //            {
        //                case MemberExpression body:
        //                    return body;
        //                case UnaryExpression expressionBody:
        //                    return (MemberExpression)expressionBody.Operand;
        //            }
        //            break;
        //    }

        //    return null;
        //}

        ///// <summary>
        /////     Gets the name of the property.
        ///// </summary>
        ///// <param name="expr">The Expression.</param>
        ///// <param name="nested">Out. Is nested property.</param>
        ///// <returns>The property name for the property expression.</returns>
        //public static string GetPropertyNamePath(Expression expr, out bool nested)
        //{
        //    var path = new StringBuilder();
        //    var memberExpression = GetMemberExpression(expr);
        //    var count = 0;
        //    do
        //    {
        //        count++;
        //        if (path.Length > 0)
        //            path.Insert(0, "");
        //        path.Insert(0, memberExpression.Member.Name);
        //        memberExpression = GetMemberExpression(memberExpression.Expression);
        //    } while (memberExpression != null);

        //    if (count > 2)
        //        throw new ArgumentException("Only one degree of nesting is supported");

        //    nested = count == 2;

        //    return path.ToString();
        //}

        //public static string GetExpressionSql<TEntity>(Expression<Func<TEntity, bool>> expression, ref Dictionary<string, object> pms)
        //{
        //    return DealExpress(expression, ref pms);
        //}

        //private static string DealExpress(Expression exp, ref Dictionary<string, object> pms)
        //{
        //    if (exp is LambdaExpression)
        //    {
        //        LambdaExpression l_exp = exp as LambdaExpression;
        //        return DealExpress(l_exp.Body, ref pms);
        //    }
        //    if (exp is BinaryExpression)
        //    {
        //        return DealBinaryExpression(exp as BinaryExpression, ref pms);
        //    }
        //    if (exp is MemberExpression)
        //    {
        //        return DealMemberExpression(exp as MemberExpression, ref pms);
        //    }
        //    if (exp is ConstantExpression)
        //    {
        //        return DealConstantExpression(exp as ConstantExpression, ref pms);
        //    }
        //    if (exp is UnaryExpression)
        //    {
        //        return DealUnaryExpression(exp as UnaryExpression, ref pms);
        //    }
        //    if (exp is MethodCallExpression method)
        //    {
        //        return DealMethodCallExpression(method, ref pms);
        //    }
        //    return "";
        //}
        //private static string DealUnaryExpression(UnaryExpression exp, ref Dictionary<string, object> pms)
        //{
        //    return DealExpress(exp.Operand, ref pms);
        //}
        //private static string DealConstantExpression(ConstantExpression exp, ref Dictionary<string, object> pms)
        //{
        //    //object vaule = exp.Value;
        //    //string v_str = string.Empty;
        //    //if (vaule == null)
        //    //{
        //    //    return "NULL";
        //    //}
        //    //if (vaule is string)
        //    //{
        //    //    v_str = string.Format("'{0}'", vaule.ToString());
        //    //}
        //    //else if (vaule is DateTime)
        //    //{
        //    //    DateTime time = (DateTime)vaule;
        //    //    v_str = string.Format("'{0}'", time.ToString("yyyy-MM-dd HH:mm:ss"));
        //    //}
        //    //else
        //    //{
        //    //    v_str = vaule.ToString();
        //    //}
        //    //return v_str;
        //    var p = pms.NewParam();
        //    pms.Add(p, exp.Value);
        //    return "@" + p;
        //}
        //private static string DealBinaryExpression(BinaryExpression exp, ref Dictionary<string, object> pms)
        //{

        //    string left = DealExpress(exp.Left, ref pms);
        //    string oper = GetOperStr(exp.NodeType, ref pms);
        //    string right = DealExpress(exp.Right, ref pms);
        //    if (right == "NULL")
        //    {
        //        if (oper == "=")
        //        {
        //            oper = " is ";
        //        }
        //        else
        //        {
        //            oper = " is not ";
        //        }
        //    }
        //    return left + oper + right;
        //}
        //private static string DealMemberExpression(MemberExpression exp, ref Dictionary<string, object> pms)
        //{
        //    return exp.Member.Name;
        //}
        //private static string GetOperStr(ExpressionType e_type, ref Dictionary<string, object> pms)
        //{
        //    switch (e_type)
        //    {
        //        case ExpressionType.OrElse: return " OR ";
        //        case ExpressionType.Or: return "|";
        //        case ExpressionType.AndAlso: return " AND ";
        //        case ExpressionType.And: return "&";
        //        case ExpressionType.GreaterThan: return ">";
        //        case ExpressionType.GreaterThanOrEqual: return ">=";
        //        case ExpressionType.LessThan: return "<";
        //        case ExpressionType.LessThanOrEqual: return "<=";
        //        case ExpressionType.NotEqual: return "<>";
        //        case ExpressionType.Add: return "+";
        //        case ExpressionType.Subtract: return "-";
        //        case ExpressionType.Multiply: return "*";
        //        case ExpressionType.Divide: return "/";
        //        case ExpressionType.Modulo: return "%";
        //        case ExpressionType.Equal: return "=";
        //    }
        //    return "";
        //}
        //private static string DealMethodCallExpression(MethodCallExpression exp, ref Dictionary<string, object> pms)
        //{
        //    string sql = "";
        //    if (exp.Object is MemberExpression exp1)
        //    {
        //        sql += DealMemberExpression(exp1, ref pms);
        //    }

        //    if (exp.Method.Name.Equals("Equals"))
        //    {
        //        sql += "=";
        //    }
        //    if (exp.Arguments.Count > 0 && exp.Arguments[0] is ConstantExpression c)
        //    {
        //        var p = pms.NewParam();
        //        sql += "@" + p;
        //        pms.Add(p, c.Value);
        //    }
           
        //    return sql;
        //}

        //private static string NewParam(this Dictionary<string, object> pms)
        //{
        //    return "p_" + pms?.Count;
        //}


    }
}
