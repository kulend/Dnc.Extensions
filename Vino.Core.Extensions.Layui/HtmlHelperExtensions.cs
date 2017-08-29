using Microsoft.AspNetCore.Html;
using System;
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
            return helper.LayuiBeginForm(action);
        }

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

            return helper.LayuiFormActions(buttons);
        }

        public static HtmlString ActionsForSubmit<TModel>(this IHtmlHelper<TModel> htmlHelper)
        {
            return htmlHelper.ActionsFor(new SubmitButton());
        }

        public static HtmlString ActionsForSubmitAndClose<TModel>(this IHtmlHelper<TModel> htmlHelper)
        {
            return htmlHelper.ActionsFor(new SubmitButton(), new CloseButton());
        }

        public static HtmlString ActionsForSubmitAndReset<TModel>(this IHtmlHelper<TModel> htmlHelper)
        {
            return htmlHelper.ActionsFor(new SubmitButton(), new ResetButton());
        }

        public static IHtmlContent InputFor<TModel, TResult>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TResult>> expression)
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
            return helper.LayuiInputFor(expression);
        }
    }
}
