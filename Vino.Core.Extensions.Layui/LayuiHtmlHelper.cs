using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.Encodings.Web;
using Vino.Core.Extensions.Layui;

namespace Microsoft.AspNetCore.Mvc.Rendering
{
    public class LayuiHtmlHelper<TModel> : HtmlHelper<TModel>
    {
        protected readonly IHtmlGenerator _htmlGenerator;
        protected readonly HtmlEncoder _htmlEncoder;
        protected readonly LayuiOption _layuiOption;

        public LayuiHtmlHelper(IHtmlGenerator htmlGenerator,
            ICompositeViewEngine viewEngine,
            IModelMetadataProvider metadataProvider,
            IViewBufferScope bufferScope,
            HtmlEncoder htmlEncoder,
            UrlEncoder urlEncoder,
            ExpressionTextCache expressionTextCache,
            IOptions<LayuiOption> layuiOption) : 
            base(htmlGenerator, viewEngine, metadataProvider, bufferScope, htmlEncoder, urlEncoder, expressionTextCache)
        {
            this._htmlGenerator = htmlGenerator;
            this._htmlEncoder = htmlEncoder;
            this._layuiOption = layuiOption.Value;
        }

        public MvcForm LayuiBeginForm(string action, FormMethod method = FormMethod.Post)
        {
            ViewContext.FormContext = new FormContext
            {
                CanRenderAtEndOfForm = true
            };

            var tagBuilder = new TagBuilder("form");
            tagBuilder.GenerateId("inputForm", "");
            tagBuilder.AddCssClass("layui-form");
            tagBuilder.MergeAttribute("action", action);
            tagBuilder.MergeAttribute("method", method.ToString(), replaceExisting: true);

            tagBuilder.TagRenderMode = TagRenderMode.StartTag;
            tagBuilder.WriteTo(ViewContext.Writer, _htmlEncoder);

            return new MvcForm(ViewContext, _htmlEncoder);
        }

        public IHtmlContent LayuiTextBoxFor<TResult>(Expression<Func<TModel, TResult>> expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            var modelExplorer = GetModelExplorer(expression);
            var metadata = modelExplorer.Metadata;
            var name = GetExpressionName(expression);
            return RenderTextBox(name, modelExplorer, metadata);
        }

        public IHtmlContent LayuiSwitchFor<TResult>(Expression<Func<TModel, TResult>> expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            var modelExplorer = GetModelExplorer(expression);
            var metadata = modelExplorer.Metadata;
            var name = GetExpressionName(expression);

            return RenderSwitch(name, modelExplorer, metadata);
        }

        public IHtmlContent LayuiTextAreaFor<TResult>(Expression<Func<TModel, TResult>> expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            var modelExplorer = GetModelExplorer(expression);
            var metadata = modelExplorer.Metadata;
            var name = GetExpressionName(expression);

            return RenderTextarea(name, modelExplorer, metadata);
        }

        public IHtmlContent LayuiHiddenFor<TResult>(Expression<Func<TModel, TResult>> expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            var modelExplorer = GetModelExplorer(expression);
            var metadata = modelExplorer.Metadata;
            var name = GetExpressionName(expression);

            return RenderHidden(name, modelExplorer, metadata);
        }

        public IHtmlContent LayuiPasswordFor<TResult>(Expression<Func<TModel, TResult>> expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            var modelExplorer = GetModelExplorer(expression);
            var metadata = modelExplorer.Metadata;
            var name = GetExpressionName(expression);

            return RenderPassword(name, modelExplorer, metadata);
        }

        public IHtmlContent LayuiEnumRadioFor<TResult>(Expression<Func<TModel, TResult>> expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            var modelExplorer = GetModelExplorer(expression);
            var metadata = modelExplorer.Metadata;
            var name = GetExpressionName(expression);

            return RenderEnumRadio(name, modelExplorer, metadata);
        }

        public HtmlString LayuiFormActions(params ActionButton[] buttons)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<div class=\"layui-form-item\">");
            sb.AppendLine("<div class=\"layui-input-block\">");
            foreach (var btn in buttons)
            {
                switch (btn)
                {
                    case SubmitButton submit:
                        if (string.IsNullOrEmpty(btn.Text))
                        {
                            btn.Text = _layuiOption.SubmitButtonText;
                        }
                        sb.AppendLine($"<button class=\"layui-btn {btn.Css}\" lay-submit>{btn.Text}</button>");
                        break;
                    case CloseButton close:
                        if (string.IsNullOrEmpty(btn.Text))
                        {
                            btn.Text = _layuiOption.CloseButtonText;
                        }
                        if (string.IsNullOrEmpty(btn.OnClick))
                        {
                            btn.OnClick = _layuiOption.CloseButtonOnClick;
                        }
                        sb.AppendLine($"<button type=\"button\" class=\"layui-btn {btn.Css}\" OnClick=\"{btn.OnClick}\">{btn.Text}</button>");
                        break;
                    case ResetButton reset:
                        if (string.IsNullOrEmpty(btn.Text))
                        {
                            btn.Text = _layuiOption.ResetButtonText;
                        }
                        sb.AppendLine($"<button type=\"reset\" class=\"layui-btn {btn.Css}\">{btn.Text}</button>");
                        break;
                    default:
                        sb.AppendLine($"<button type=\"button\" class=\"layui-btn {btn.Css}\" OnClick=\"{btn.OnClick}\">{btn.Text}</button>");
                        break;
                }
            }

            sb.AppendLine("</div></div>");

            return new HtmlString(sb.ToString());
        }

        public IHtmlContent LayuiInputFor<TResult>(Expression<Func<TModel, TResult>> expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            var modelExplorer = GetModelExplorer(expression);
            var metadata = modelExplorer.Metadata;
            var name = GetExpressionName(expression);

            Type type = GetRealType(modelExplorer.ModelType);

            IHtmlContent content = new HtmlString(name);
            var dataTypeName = metadata.DataTypeName;
            if (string.IsNullOrEmpty(dataTypeName))
            {
                if (type == typeof(bool))
                {
                    dataTypeName = "switch";
                }
                else if (metadata.IsEnum)
                {
                    dataTypeName = "enum_radio";
                }
                else if (type == typeof(DateTime))
                {
                    dataTypeName = "datetime";
                }
                else
                {
                    dataTypeName = "text";
                }
            }
            
            switch (dataTypeName.ToLower())
            {
                case "text":
                    content = RenderTextBox(name, modelExplorer, metadata);
                    break;
                case "hidden":
                    content = RenderHidden(name, modelExplorer, metadata);
                    break;
                case "password":
                    content = RenderPassword(name, modelExplorer, metadata);
                    break;
                case "switch":
                    content = RenderSwitch(name, modelExplorer, metadata);
                    break;
                case "multilinetext":
                case "textarea":
                    content = RenderTextarea(name, modelExplorer, metadata);
                    break;
                case "enum_radio":
                    content = RenderEnumRadio(name, modelExplorer, metadata);
                    break;
                case "datetime":
                case "date":
                case "year":
                case "month":
                case "time":
                    content = RenderDateTime(name, modelExplorer, metadata);
                    break;
            }

            return content;
        }

        private IHtmlContent RenderTextBox(string name, ModelExplorer modelExplorer, ModelMetadata metadata)
        {
            var tag = new TagBuilder("div");
            tag.AddCssClass("layui-form-item");

            var displayName = metadata.GetDisplayName();
            var isRequired = metadata.IsRequired;
            var placeholder = metadata.Placeholder;

            //取得最大长度
            var maxLength = 30;
            var maxLengthAttribute = metadata.ValidatorMetadata.SingleOrDefault(x => x.GetType() == typeof(MaxLengthAttribute));
            if (maxLengthAttribute != null)
            {
                maxLength = (maxLengthAttribute as MaxLengthAttribute).Length;
            }
            var isNumber = metadata.ModelType == typeof(Int32) || metadata.ModelType == typeof(Int16);
            var input = new TagBuilder("input");

            //长度标签
            if (isNumber)
            {
                input.AddCssClass("input-length-num");
            }
            else if (maxLength >= 50)
            {
                input.AddCssClass("input-length-long");
            }
            else if (maxLength >= 20)
            {
                input.AddCssClass("input-length-middle");
            }
            else
            {
                input.AddCssClass("input-length-short");
            }

            input.AddCssClass("layui-input");
            input.TagRenderMode = TagRenderMode.SelfClosing;
            input.MergeAttribute("type", InputType.Text.ToString());
            input.MergeAttribute("name", name, replaceExisting: true);
            if (!string.IsNullOrEmpty(placeholder))
            {
                input.MergeAttribute("placeholder", placeholder);
            }
            if (isRequired)
            {
                input.MergeAttribute("lay-verify", "required");
            }

            if (!isNumber)
            {
                input.MergeAttribute("maxlength", maxLength.ToString());
            }

            if (modelExplorer.Model != null)
            {
                input.MergeAttribute("value", modelExplorer.Model.ToString());
            }

            tag.InnerHtml.AppendHtml($"<label class=\"layui-form-label\">{displayName}</label>");
            tag.InnerHtml.AppendHtml("<div class=\"layui-input-block\">");
            tag.InnerHtml.AppendHtml(input);
            tag.InnerHtml.AppendHtml("</div>");
            return tag;
        }

        private IHtmlContent RenderHidden(string name, ModelExplorer modelExplorer, ModelMetadata metadata)
        {
            var input = new TagBuilder("input");
            input.GenerateId(name, "");
            input.TagRenderMode = TagRenderMode.SelfClosing;
            input.MergeAttribute("type", InputType.Hidden.ToString());
            input.MergeAttribute("name", name, replaceExisting: true);

            if (modelExplorer.Model != null)
            {
                input.MergeAttribute("value", modelExplorer.Model.ToString());
            }
            return input;
        }

        private IHtmlContent RenderSwitch(string name, ModelExplorer modelExplorer, ModelMetadata metadata)
        {
            var tag = new TagBuilder("div");
            tag.AddCssClass("layui-form-item");

            var displayName = metadata.GetDisplayName();
            var placeholder = metadata.Placeholder;

            var input = new TagBuilder("input");
            input.GenerateId(name, "");
            input.TagRenderMode = TagRenderMode.SelfClosing;
            input.MergeAttribute("type", InputType.CheckBox.ToString());
            input.MergeAttribute("name", name, replaceExisting: true);
            input.MergeAttribute("value", "true");
            input.MergeAttribute("lay-skin", "switch");
            if (!string.IsNullOrEmpty(placeholder))
            {
                input.MergeAttribute("lay-text", placeholder);
            }
            if (modelExplorer.Model != null)
            {
                if (bool.TryParse(modelExplorer.Model.ToString(), out bool modelChecked))
                {
                    if (modelChecked)
                    {
                        input.MergeAttribute("checked", "checked");
                    }
                }
            }

            tag.InnerHtml.AppendHtml($"<label class=\"layui-form-label\">{displayName}</label>");
            tag.InnerHtml.AppendHtml("<div class=\"layui-input-inline\">");
            tag.InnerHtml.AppendHtml(input);
            tag.InnerHtml.AppendHtml("</div>");

            return tag;
        }

        private IHtmlContent RenderTextarea(string name, ModelExplorer modelExplorer, ModelMetadata metadata)
        {
            var tag = new TagBuilder("div");
            tag.AddCssClass("layui-form-item");

            var displayName = metadata.GetDisplayName();
            var isRequired = metadata.IsRequired;
            var placeholder = metadata.Placeholder;

            //取得最大长度
            var maxLength = 100;
            var maxLengthAttribute = metadata.ValidatorMetadata.SingleOrDefault(x => x.GetType() == typeof(MaxLengthAttribute));
            if (maxLengthAttribute != null)
            {
                maxLength = (maxLengthAttribute as MaxLengthAttribute).Length;
            }
            var input = new TagBuilder("textarea");

            input.AddCssClass("layui-textarea");

            input.TagRenderMode = TagRenderMode.Normal;
            input.MergeAttribute("name", name, replaceExisting: true);
            if (!string.IsNullOrEmpty(placeholder))
            {
                input.MergeAttribute("placeholder", placeholder);
            }
            if (isRequired)
            {
                input.MergeAttribute("lay-verify", "required");
            }
            input.MergeAttribute("maxlength", maxLength.ToString());
            if (modelExplorer.Model != null)
            {
                input.InnerHtml.Append(modelExplorer.Model.ToString());
            }

            tag.InnerHtml.AppendHtml($"<label class=\"layui-form-label\">{displayName}</label>");
            tag.InnerHtml.AppendHtml("<div class=\"layui-input-block\">");
            tag.InnerHtml.AppendHtml(input);
            tag.InnerHtml.AppendHtml("</div>");
            return tag;
        }

        private IHtmlContent RenderPassword(string name, ModelExplorer modelExplorer, ModelMetadata metadata)
        {
            var tag = new TagBuilder("div");
            tag.AddCssClass("layui-form-item");

            var displayName = metadata.GetDisplayName();
            var isRequired = metadata.IsRequired;
            var placeholder = metadata.Placeholder;

            //取得最大长度
            var maxLength = 20;
            var maxLengthAttribute = metadata.ValidatorMetadata.SingleOrDefault(x => x.GetType() == typeof(MaxLengthAttribute));
            if (maxLengthAttribute != null)
            {
                maxLength = (maxLengthAttribute as MaxLengthAttribute).Length;
            }

            var input = new TagBuilder("input");

            //长度标签
            input.AddCssClass("input-length-short");

            input.AddCssClass("layui-input");
            input.TagRenderMode = TagRenderMode.SelfClosing;
            input.MergeAttribute("type", InputType.Password.ToString());
            input.MergeAttribute("name", name, replaceExisting: true);
            if (!string.IsNullOrEmpty(placeholder))
            {
                input.MergeAttribute("placeholder", placeholder);
            }
            if (isRequired)
            {
                input.MergeAttribute("lay-verify", "required");
            }

            input.MergeAttribute("maxlength", maxLength.ToString());

            if (modelExplorer.Model != null)
            {
                input.MergeAttribute("value", modelExplorer.Model.ToString());
            }

            tag.InnerHtml.AppendHtml($"<label class=\"layui-form-label\">{displayName}</label>");
            tag.InnerHtml.AppendHtml("<div class=\"layui-input-block\">");
            tag.InnerHtml.AppendHtml(input);
            tag.InnerHtml.AppendHtml("</div>");
            return tag;
        }

        private IHtmlContent RenderEnumRadio(string name, ModelExplorer modelExplorer, ModelMetadata metadata)
        {
            var tag = new TagBuilder("div");
            tag.AddCssClass("layui-form-item");

            if (!metadata.IsEnum)
            {
                throw new Exception("type is not enum.");
            }
            var displayName = metadata.GetDisplayName();

            tag.InnerHtml.AppendHtml($"<label class=\"layui-form-label\">{displayName}</label>");
            tag.InnerHtml.AppendHtml("<div class=\"layui-input-block\">");

            string value = "";
            if (modelExplorer.Model != null)
            {
                value = Convert.ToInt32(modelExplorer.Model).ToString();
            }

            var displays = metadata.EnumGroupedDisplayNamesAndValues ?? new List<KeyValuePair<EnumGroupAndName, string>>();
            foreach (var enumItem in displays)
            {
                var itemName = enumItem.Key.Name;
                var itemValue = enumItem.Value;
                var input = new TagBuilder("input");
                input.GenerateId(name + "_" + itemValue, "");
                input.TagRenderMode = TagRenderMode.SelfClosing;
                input.MergeAttribute("type", InputType.Radio.ToString());
                input.MergeAttribute("name", name, replaceExisting: true);
                input.MergeAttribute("value", itemValue);
                input.MergeAttribute("title", itemName);
                if (itemValue.Equals(value))
                {
                    input.MergeAttribute("checked", "checked");
                }
                tag.InnerHtml.AppendHtml(input);
            }

            tag.InnerHtml.AppendHtml("</div>");
            return tag;
        }

        private IHtmlContent RenderDateTime(string name, ModelExplorer modelExplorer, ModelMetadata metadata)
        {
            var tag = new TagBuilder("div");
            tag.AddCssClass("layui-form-item");

            var displayName = metadata.GetDisplayName();
            var placeholder = metadata.Placeholder;

            var input = new TagBuilder("input");
            input.AddCssClass("layui-input");
            input.AddCssClass("laydate");
            input.GenerateId(name, "");
            input.TagRenderMode = TagRenderMode.SelfClosing;
            input.MergeAttribute("type", InputType.Text.ToString());
            input.MergeAttribute("name", name, replaceExisting: true);
            if (!string.IsNullOrEmpty(placeholder))
            {
                input.MergeAttribute("lay-text", placeholder);
            }
            if (!string.IsNullOrEmpty(metadata.DisplayFormatString))
            {
                input.MergeAttribute("data-format", metadata.DisplayFormatString);
            }
            if (!string.IsNullOrEmpty(metadata.DataTypeName))
            {
                input.MergeAttribute("data-type", metadata.DataTypeName.ToLower());
            }

            if (modelExplorer.Model != null)
            {
                input.MergeAttribute("value", modelExplorer.Model.ToString());
            }

            tag.InnerHtml.AppendHtml($"<label class=\"layui-form-label\">{displayName}</label>");
            tag.InnerHtml.AppendHtml("<div class=\"layui-input-inline\">");
            tag.InnerHtml.AppendHtml(input);
            tag.InnerHtml.AppendHtml("</div>");

            return tag;
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

    public class ActionButton
    {
        public string Id { set; get; }

        public string Text { set; get; }

        public string Css { set; get; }

        public string OnClick { set; get; }
    }

    public class SubmitButton : ActionButton
    {
    }

    public class CloseButton : ActionButton
    {
        public CloseButton()
        {
            Css = "layui-btn-warm";
        }
    }

    public class ResetButton : ActionButton
    {
        public ResetButton()
        {
            Css = "layui-btn-primary";
        }
    }
}
