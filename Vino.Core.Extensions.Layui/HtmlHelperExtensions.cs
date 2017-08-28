using Microsoft.AspNetCore.Html;
using System;
using System.Linq.Expressions;
using System.Text;

namespace Microsoft.AspNetCore.Mvc.Rendering
{
    public static class HtmlHelperExtensions
    {
        public static MvcForm LayuiBeginForm<TModel>(this IHtmlHelper htmlHelper, string action)
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

        public static IHtmlContent LayuiTextBoxFor<TModel, TResult>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TResult>> expression)
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
            return helper.LayuiTextBoxFor(expression);
        }

        public static IHtmlContent LayuiSwitchFor<TModel, TResult>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TResult>> expression)
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
            return helper.LayuiSwitchFor(expression);
        }

        public static IHtmlContent LayuiTextAreaFor<TModel, TResult>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TResult>> expression)
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
            return helper.LayuiTextAreaFor(expression);
        }

        public static IHtmlContent LayuiHiddenFor<TModel, TResult>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TResult>> expression)
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
            return helper.LayuiHiddenFor(expression);
        }

        public static IHtmlContent LayuiPasswordFor<TModel, TResult>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TResult>> expression)
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
            return helper.LayuiPasswordFor(expression);
        }

        public static IHtmlContent LayuiEnumRadioFor<TModel, TResult>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TResult>> expression)
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
            return helper.LayuiEnumRadioFor(expression);
        }

        public static HtmlString LayuiFormActions<TModel>(this IHtmlHelper<TModel> htmlHelper, params LayuiActionButton[] buttons)
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

        public static HtmlString LayuiFormActionsForSubmit<TModel>(this IHtmlHelper<TModel> htmlHelper)
        {
            return htmlHelper.LayuiFormActions(new LayuiSubmitActionButton());
        }

        public static HtmlString LayuiFormActionsForSubmitAndClose<TModel>(this IHtmlHelper<TModel> htmlHelper)
        {
            return htmlHelper.LayuiFormActions(new LayuiSubmitActionButton(), new LayuiCloseActionButton());
        }

        public static HtmlString LayuiFormActionsForSubmitAndReset<TModel>(this IHtmlHelper<TModel> htmlHelper)
        {
            return htmlHelper.LayuiFormActions(new LayuiSubmitActionButton(), new LayuiResetActionButton());
        }

        public static IHtmlContent LayuiInputFor<TModel, TResult>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TResult>> expression)
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
