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

        public HtmlString LayuiActionsFor(params ActionButton[] buttons)
        {
            StringBuilder sb = new StringBuilder();
            if (_layuiOption.ActionsInFormItem)
            {
                sb.AppendLine("<div class=\"layui-form-item\">");
                sb.AppendLine("<div class=\"layui-input-block\">");
            }
            foreach (var btn in buttons)
            {
                switch (btn)
                {
                    case SubmitButton submit:
                        if (submit.IsDefault)
                        {
                            submit.Text = _layuiOption.Submit.Text;
                            submit.Css  = _layuiOption.Submit.Css;
                            submit.Action = _layuiOption.Submit.Action;
                            submit.Icon = _layuiOption.Submit.Icon;
                        }
                        sb.AppendLine($"<button class=\"layui-btn {btn.Css}\" lay-submit>{btn.Text}</button>");
                        break;
                    case CloseButton close:
                        if (close.IsDefault)
                        {
                            close.Text = _layuiOption.Close.Text;
                            close.Css = _layuiOption.Close.Css;
                            close.Action = _layuiOption.Close.Action;
                            close.Icon = _layuiOption.Close.Icon;
                        }
                        sb.AppendLine($"<button type=\"button\" class=\"layui-btn {btn.Css}\" Action=\"{btn.Action}\">{btn.Text}</button>");
                        break;
                    case ResetButton reset:
                        if (reset.IsDefault)
                        {
                            reset.Text = _layuiOption.Reset.Text;
                            reset.Css = _layuiOption.Reset.Css;
                            reset.Action = _layuiOption.Reset.Action;
                            reset.Icon = _layuiOption.Reset.Icon;
                        }
                        sb.AppendLine($"<button type=\"reset\" class=\"layui-btn {btn.Css}\">{btn.Text}</button>");
                        break;
                    default:
                        sb.AppendLine($"<button type=\"button\" class=\"layui-btn {btn.Css}\" Action=\"{btn.Action}\">{btn.Text}</button>");
                        break;
                }
            }

            if (_layuiOption.ActionsInFormItem)
            {
                sb.AppendLine("</div></div>");
            }
            return new HtmlString(sb.ToString());
        }

        public IHtmlContent LayuiInputFor<TResult>(Expression<Func<TModel, TResult>> expression, params KeyValuePair<string, string>[] attrs)
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
                    content = RenderSwitch(name, modelExplorer, metadata, attrs);
                    break;
                case "multilinetext":
                case "textarea":
                    content = RenderTextarea(name, modelExplorer, metadata);
                    break;
                case "enum_radio":
                    content = RenderEnumRadio(name, modelExplorer, metadata, attrs);
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

        public IHtmlContent LayuiShowFor<TResult>(Expression<Func<TModel, TResult>> expression)
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
                    content = RenderLable(name, modelExplorer, metadata);
                    break;
                case "hidden":
                    content = RenderHidden(name, modelExplorer, metadata);
                    break;
                case "password":
                    content = RenderPassword(name, modelExplorer, metadata);
                    break;
                case "switch":
                    content = RenderSwitchShow(name, modelExplorer, metadata);
                    break;
                case "multilinetext":
                case "textarea":
                    content = RenderTextarea(name, modelExplorer, metadata);
                    break;
                case "enum_radio":
                    content = RenderEnumRadioShow(name, modelExplorer, metadata);
                    break;
                case "datetime":
                case "date":
                case "year":
                case "month":
                case "time":
                    content = RenderDateTimeShow(name, modelExplorer, metadata);
                    break;
                case "image":
                case "imageurl":
                    content = RenderImageShow(name, modelExplorer, metadata);
                    break;
            }

            return content;
        }

        #region Input

        private IHtmlContent RenderTextBox(string name, ModelExplorer modelExplorer, ModelMetadata metadata, params KeyValuePair<string, string>[] attrs)
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
            var length = "";
            if (isNumber)
            {
                length = "num";
            }
            else if (maxLength >= 50)
            {
                length = "long";
            }
            else if (maxLength >= 20)
            {
                length = "middle";
            }
            else
            {
                length = "short";
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

            if (attrs != null && attrs.Length > 0)
            {
                foreach (var attr in attrs)
                {
                    input.MergeAttribute(attr.Key, attr.Value);
                }
            }

            tag.InnerHtml.AppendHtml($"<label class=\"layui-form-label\">{displayName}</label>");
            tag.InnerHtml.AppendHtml($"<div class=\"layui-input-inline {length}\">");
            tag.InnerHtml.AppendHtml(input);
            tag.InnerHtml.AppendHtml("</div>");
            if (!string.IsNullOrEmpty(metadata.Description))
            {
                tag.InnerHtml.AppendHtml($"<div class=\"layui-form-mid layui-word-aux\">{metadata.Description}</div>");
            }
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

        private IHtmlContent RenderSwitch(string name, ModelExplorer modelExplorer, ModelMetadata metadata, params KeyValuePair<string, string>[] attrs)
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

            if (attrs != null && attrs.Length > 0)
            {
                foreach (var attr in attrs)
                {
                    input.MergeAttribute(attr.Key, attr.Value);
                }
            }

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

        private IHtmlContent RenderEnumRadio(string name, ModelExplorer modelExplorer, ModelMetadata metadata, params KeyValuePair<string, string>[] attrs)
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
                if (attrs != null && attrs.Length > 0)
                {
                    foreach (var attr in attrs)
                    {
                        input.MergeAttribute(attr.Key, attr.Value);
                    }
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

            Type type = GetRealType(modelExplorer.ModelType);
            var dataTypeName = metadata.DataTypeName;
            if (string.IsNullOrEmpty(dataTypeName))
            {
                dataTypeName = "datetime";
            }
            dataTypeName = dataTypeName.ToLower();
            var defaultFormatStr = "yyyy-MM-dd HH:mm:ss";
            switch (dataTypeName)
            {
                case "datetime":
                    defaultFormatStr = "yyyy-MM-dd HH:mm:ss";
                    break;
                case "date":
                    defaultFormatStr = "yyyy-MM-dd";
                    break;
                case "year":
                    defaultFormatStr = "yyyy";
                    break;
                case "month":
                    defaultFormatStr = "yyyy-MM";
                    break;
                case "time":
                    defaultFormatStr = "HH:mm:ss";
                    break;
            }

            var formatString = metadata.DisplayFormatString ?? defaultFormatStr;

            if (!string.IsNullOrEmpty(placeholder))
            {
                input.MergeAttribute("lay-text", placeholder);
            }
            input.MergeAttribute("data-format", formatString);
            if (!string.IsNullOrEmpty(metadata.DataTypeName))
            {
                input.MergeAttribute("data-type", dataTypeName);
            }

            if (modelExplorer.Model != null)
            {
                if (type == typeof(DateTime))
                {
                    input.MergeAttribute("value", ((DateTime)modelExplorer.Model).ToString(formatString));
                }
                else
                {
                    input.MergeAttribute("value", modelExplorer.Model.ToString());
                }
            }

            tag.InnerHtml.AppendHtml($"<label class=\"layui-form-label\">{displayName}</label>");
            tag.InnerHtml.AppendHtml("<div class=\"layui-input-inline\">");
            tag.InnerHtml.AppendHtml(input);
            tag.InnerHtml.AppendHtml("</div>");

            return tag;
        }

        #endregion

        #region Show

        private IHtmlContent RenderLable(string name, ModelExplorer modelExplorer, ModelMetadata metadata)
        {
            var tag = new TagBuilder("div");
            tag.AddCssClass("layui-form-item");

            var displayName = metadata.GetDisplayName();

            var lable = new TagBuilder("label");
            lable.AddCssClass("layui-form-label-show");
            if (modelExplorer.Model != null)
            {
                lable.InnerHtml.Append(modelExplorer.Model.ToString());
            }

            tag.InnerHtml.AppendHtml($"<label class=\"layui-form-label\">{displayName}</label>");
            tag.InnerHtml.AppendHtml("<div class=\"layui-input-block\">");
            tag.InnerHtml.AppendHtml(lable);
            tag.InnerHtml.AppendHtml("</div>");
            return tag;
        }

        private IHtmlContent RenderDateTimeShow(string name, ModelExplorer modelExplorer, ModelMetadata metadata)
        {
            var tag = new TagBuilder("div");
            tag.AddCssClass("layui-form-item");

            var displayName = metadata.GetDisplayName();
            var placeholder = metadata.Placeholder;

            var label = new TagBuilder("label");
            label.AddCssClass("layui-form-label-show");


            metadata.DataTypeName.ToLower();

            if (modelExplorer.Model != null)
            {
                var formate = metadata.DisplayFormatString;
                Type type = GetRealType(modelExplorer.ModelType);
                if (type == typeof(DateTime))
                {
                    DateTime value = (DateTime)modelExplorer.Model;
                    if (string.IsNullOrEmpty(formate))
                    {
                        formate = "yyyy-MM-dd HH:mm:ss";
                    }
                    label.InnerHtml.Append(value.ToString(formate));
                }
            }

            tag.InnerHtml.AppendHtml($"<label class=\"layui-form-label\">{displayName}</label>");
            tag.InnerHtml.AppendHtml("<div class=\"layui-input-inline\">");
            tag.InnerHtml.AppendHtml(label);
            tag.InnerHtml.AppendHtml("</div>");

            return tag;
        }

        private IHtmlContent RenderEnumRadioShow(string name, ModelExplorer modelExplorer, ModelMetadata metadata)
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

            var currentEnumItem = displays.SingleOrDefault(x=>x.Value.Equals(value));

            var lable = new TagBuilder("label");
            lable.AddCssClass("layui-form-label-show");
            lable.InnerHtml.Append(currentEnumItem.Key.Name);

            tag.InnerHtml.AppendHtml(lable);
            tag.InnerHtml.AppendHtml("</div>");
            return tag;
        }

        private IHtmlContent RenderSwitchShow(string name, ModelExplorer modelExplorer, ModelMetadata metadata, params KeyValuePair<string, string>[] attrs)
        {
            var tag = new TagBuilder("div");
            tag.AddCssClass("layui-form-item");

            var displayName = metadata.GetDisplayName();
            var placeholder = metadata.Placeholder;

            if (string.IsNullOrEmpty(placeholder) || !placeholder.Contains("|"))
            {
                placeholder = "是|否";
            }
            var texts = placeholder.Split("|");

            bool modelChecked = false;
            if (modelExplorer.Model != null)
            {
                bool.TryParse(modelExplorer.Model.ToString(), out modelChecked);
            }

            var text = modelChecked ? texts[0] : texts[1];

            tag.InnerHtml.AppendHtml($"<label class=\"layui-form-label\">{displayName}</label>");
            tag.InnerHtml.AppendHtml("<div class=\"layui-input-block\">");

            var lable = new TagBuilder("label");
            lable.AddCssClass("layui-form-label-show");

            if (attrs != null && attrs.Length > 0)
            {
                foreach (var attr in attrs)
                {
                    lable.MergeAttribute(attr.Key, attr.Value);
                }
            }

            lable.InnerHtml.Append(text);
            tag.InnerHtml.AppendHtml(lable);
            tag.InnerHtml.AppendHtml("</div>");
            return tag;
        }

        private IHtmlContent RenderImageShow(string name, ModelExplorer modelExplorer, ModelMetadata metadata)
        {
            var tag = new TagBuilder("div");
            tag.AddCssClass("layui-form-item");

            var displayName = metadata.GetDisplayName();

            tag.InnerHtml.AppendHtml($"<label class=\"layui-form-label\">{displayName}</label>");
            tag.InnerHtml.AppendHtml("<div class=\"layui-input-block\">");

            var value = modelExplorer.Model != null ? modelExplorer.Model.ToString() : "";
            if (!string.IsNullOrEmpty(value))
            {
                var img = new TagBuilder("img");
                img.MergeAttribute("src", value);
                img.MergeAttribute("height", "64");
                img.MergeAttribute("width", "64");
                tag.InnerHtml.AppendHtml(img);
            }
            tag.InnerHtml.AppendHtml("</div>");
            return tag;
        }


        #endregion

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
        public bool IsDefault { set; get; } = false;

        public string Id { set; get; }

        public string Text { set; get; }

        public string Css { set; get; }

        public string Action { set; get; }

        public string Icon { set; get; }
    }

    public class SubmitButton : ActionButton
    {
        public static SubmitButton DEFAULT = new SubmitButton { IsDefault = true };
    }

    public class CloseButton : ActionButton
    {
        public static CloseButton DEFAULT = new CloseButton { IsDefault = true };
    }

    public class ResetButton : ActionButton
    {
        public static ResetButton DEFAULT = new ResetButton { IsDefault = true };
    }
}
