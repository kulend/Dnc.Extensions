using Microsoft.AspNetCore.Html;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Microsoft.AspNetCore.Mvc.Rendering
{
    public static class HtmlHelperExtensions
    {
        public static MvcForm BeginForm<TModel>(this IHtmlHelper htmlHelper, string action)
        {
            if (htmlHelper == null)
            {
                throw new ArgumentNullException(nameof(htmlHelper));
            }
            var helper = htmlHelper as LayuiHtmlHelper<TModel>;
            if (helper == null)
            {
                throw new Exception("请在Startup.cs中添加service.AddLayui()");
            }

            return helper.LayuiBeginForm(
                actionName: action,
                controllerName: null,
                routeValues: null,
                method: FormMethod.Post,
                antiforgery: null,
                htmlAttributes: null);
        }

        public static MvcForm BeginForm<TModel>(this IHtmlHelper htmlHelper, string action, object htmlAttributes)
        {
            if (htmlHelper == null)
            {
                throw new ArgumentNullException(nameof(htmlHelper));
            }
            var helper = htmlHelper as LayuiHtmlHelper<TModel>;
            if (helper == null)
            {
                throw new Exception("请在Startup.cs中添加service.AddLayui()");
            }
            return helper.LayuiBeginForm(
                actionName: action,
                controllerName: null,
                routeValues: null,
                method: FormMethod.Post,
                antiforgery: null,
                htmlAttributes: htmlAttributes);
        }

        public static IHtmlContent InputFor<TModel, TResult>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TResult>> expression, object htmlAttributes = null)
        {
            if (htmlHelper == null)
            {
                throw new ArgumentNullException(nameof(htmlHelper));
            }

            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }
            var helper = htmlHelper as LayuiHtmlHelper<TModel>;
            if (helper == null)
            {
                throw new Exception("请在Startup.cs中添加service.AddLayui()");
            }
            return helper.LayuiInputFor(expression, htmlAttributes);
        }

        public static IHtmlContent ShowFor<TModel, TResult>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TResult>> expression)
        {
            if (htmlHelper == null)
            {
                throw new ArgumentNullException(nameof(htmlHelper));
            }

            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }
            var helper = htmlHelper as LayuiHtmlHelper<TModel>;
            if (helper == null)
            {
                throw new Exception("请在Startup.cs中添加service.AddLayui()");
            }
            return helper.LayuiShowFor(expression);
        }

        #region 操作按钮

        public static HtmlString ActionsFor<TModel>(this IHtmlHelper<TModel> htmlHelper, params ActionButton[] buttons)
        {
            if (htmlHelper == null)
            {
                throw new ArgumentNullException(nameof(htmlHelper));
            }

            var helper = htmlHelper as LayuiHtmlHelper<TModel>;
            if (helper == null)
            {
                throw new Exception("请在Startup.cs中添加service.AddLayui()");
            }

            return helper.LayuiActionsFor(buttons);
        }

        public static HtmlString ActionsForSubmit<TModel>(this IHtmlHelper<TModel> htmlHelper)
        {
            return htmlHelper.ActionsFor(SubmitButton.DEFAULT);
        }

        public static HtmlString ActionsForClose<TModel>(this IHtmlHelper<TModel> htmlHelper)
        {
            return htmlHelper.ActionsFor(CloseButton.DEFAULT);
        }

        public static HtmlString ActionsForSubmitAndClose<TModel>(this IHtmlHelper<TModel> htmlHelper)
        {
            return htmlHelper.ActionsFor(SubmitButton.DEFAULT, CloseButton.DEFAULT);
        }

        public static HtmlString ActionsForSubmitAndReset<TModel>(this IHtmlHelper<TModel> htmlHelper)
        {
            return htmlHelper.ActionsFor(SubmitButton.DEFAULT, ResetButton.DEFAULT);
        }

        #endregion
    }
}
