using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
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

        public MvcForm LayuiBeginForm(string actionName, string controllerName, object routeValues, FormMethod method, bool? antiforgery, object htmlAttributes)
        {
            ViewContext.FormContext = new FormContext
            {
                CanRenderAtEndOfForm = true
            };

            var tagBuilder = _htmlGenerator.GenerateForm(ViewContext, actionName, controllerName, routeValues,
                HtmlHelper.GetFormMethodString(method), htmlAttributes);
            if (tagBuilder != null)
            {
                tagBuilder.AddCssClass("layui-form");
                tagBuilder.MergeAttribute("auto-bind", "true", replaceExisting: false);
                tagBuilder.TagRenderMode = TagRenderMode.StartTag;
                tagBuilder.WriteTo(ViewContext.Writer, _htmlEncoder);
            }
            var shouldGenerateAntiforgery = antiforgery ?? method != FormMethod.Get;
            if (shouldGenerateAntiforgery)
            {
                ViewContext.FormContext.EndOfFormContent.Add(_htmlGenerator.GenerateAntiforgery(ViewContext));
            }

            return new MvcForm(ViewContext, _htmlEncoder);
        }

        //public IHtmlContent LayuiTextBoxFor<TResult>(Expression<Func<TModel, TResult>> expression)
        //{
        //    if (expression == null)
        //    {
        //        throw new ArgumentNullException(nameof(expression));
        //    }

        //    var modelExplorer = GetModelExplorer(expression);
        //    var metadata = modelExplorer.Metadata;
        //    var name = GetExpressionName(expression);
        //    return RenderTextBox(name, modelExplorer, metadata);
        //}

        //public IHtmlContent LayuiSwitchFor<TResult>(Expression<Func<TModel, TResult>> expression)
        //{
        //    if (expression == null)
        //    {
        //        throw new ArgumentNullException(nameof(expression));
        //    }

        //    var modelExplorer = GetModelExplorer(expression);
        //    var metadata = modelExplorer.Metadata;
        //    var name = GetExpressionName(expression);

        //    return RenderSwitch(name, modelExplorer, metadata);
        //}

        //public IHtmlContent LayuiTextAreaFor<TResult>(Expression<Func<TModel, TResult>> expression)
        //{
        //    if (expression == null)
        //    {
        //        throw new ArgumentNullException(nameof(expression));
        //    }

        //    var modelExplorer = GetModelExplorer(expression);
        //    var metadata = modelExplorer.Metadata;
        //    var name = GetExpressionName(expression);

        //    return RenderTextarea(name, modelExplorer, metadata);
        //}

        //public IHtmlContent LayuiHiddenFor<TResult>(Expression<Func<TModel, TResult>> expression)
        //{
        //    if (expression == null)
        //    {
        //        throw new ArgumentNullException(nameof(expression));
        //    }

        //    var modelExplorer = GetModelExplorer(expression);
        //    var metadata = modelExplorer.Metadata;
        //    var name = GetExpressionName(expression);

        //    return RenderHidden(name, modelExplorer, metadata);
        //}

        //public IHtmlContent LayuiPasswordFor<TResult>(Expression<Func<TModel, TResult>> expression)
        //{
        //    if (expression == null)
        //    {
        //        throw new ArgumentNullException(nameof(expression));
        //    }

        //    var modelExplorer = GetModelExplorer(expression);
        //    var metadata = modelExplorer.Metadata;
        //    var name = GetExpressionName(expression);

        //    return RenderPassword(name, modelExplorer, metadata);
        //}

        //public IHtmlContent LayuiEnumRadioFor<TResult>(Expression<Func<TModel, TResult>> expression)
        //{
        //    if (expression == null)
        //    {
        //        throw new ArgumentNullException(nameof(expression));
        //    }

        //    var modelExplorer = GetModelExplorer(expression);
        //    var metadata = modelExplorer.Metadata;
        //    var name = GetExpressionName(expression);

        //    return RenderEnumRadio(name, modelExplorer, metadata);
        //}

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
                            submit.Css = _layuiOption.Submit.Css;
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

        public IHtmlContent LayuiInputFor<TResult>(Expression<Func<TModel, TResult>> expression, object htmlAttributes)
        {
            return RenderItem(expression, false, null, htmlAttributes);
        }

        public IHtmlContent LayuiInputFor<TResult1, TResult2>(Expression<Func<TModel, TResult1>> expr1, Expression<Func<TModel, TResult2>> expr2)
        {
            var tag = new TagBuilder("div");
            tag.AddCssClass("layui-form-item");

            tag.InnerHtml.AppendHtml(RenderItem(expr1, true, null, null));
            tag.InnerHtml.AppendHtml(RenderItem(expr2, true, null, null));
            return tag;
        }

        public IHtmlContent LayuiShowFor<TResult>(Expression<Func<TModel, TResult>> expression, string dataType, object htmlAttributes)
        {
            return RenderItemShow(expression, false, dataType, htmlAttributes);
        }

        public IHtmlContent LayuiShowFor<TResult1, TResult2>(Expression<Func<TModel, TResult1>> expr1, Expression<Func<TModel, TResult2>> expr2)
        {
            var tag = new TagBuilder("div");
            tag.AddCssClass("layui-form-item");

            tag.InnerHtml.AppendHtml(RenderItemShow(expr1, true, null, null));
            tag.InnerHtml.AppendHtml(RenderItemShow(expr2, true, null, null));
            return tag;
        }

        #region Input

        private IHtmlContent RenderItem<TResult>(Expression<Func<TModel, TResult>> expression, bool inline, string dataType, object htmlAttributes)
        {
            if (expression == null)
            {
                return new HtmlString("");
            }

            var modelExplorer = GetModelExplorer(expression);
            var metadata = modelExplorer.Metadata;
            var name = GetExpressionName(expression);
            Type type = GetRealType(modelExplorer.ModelType);

            IHtmlContent content = new HtmlString(name);
            var dataTypeName = string.IsNullOrEmpty(dataType) ? GetDataTypeName(type, metadata) : dataType.ToLower();

            switch (dataTypeName)
            {
                case "text":
                    content = RenderTextBox(name, modelExplorer, dataTypeName, inline, htmlAttributes);
                    break;
                case "hidden":
                    content = RenderHidden(name, modelExplorer, metadata);
                    break;
                case "switch":
                    content = RenderSwitch(name, modelExplorer, metadata, inline, htmlAttributes);
                    break;
                case "multilinetext":
                case "textarea":
                    content = RenderTextarea(name, modelExplorer, metadata);
                    break;
                case "enum_radio":
                    content = RenderEnumRadio(name, modelExplorer, metadata, htmlAttributes);
                    break;
                case "datetime":
                case "date":
                case "year":
                case "month":
                case "time":
                    content = RenderDateTime(name, modelExplorer, inline, htmlAttributes);
                    break;
                case "url":
                case "password":
                case "email":
                case "emailaddress":
                default:
                    content = RenderTextBox(name, modelExplorer, dataTypeName, inline, htmlAttributes);
                    break;
            }

            return content;
        }

        private IHtmlContent RenderTextBox(string name, ModelExplorer modelExplorer, string dataType, bool inline, object htmlAttributes)
        {
            ModelMetadata metadata = modelExplorer.Metadata;

            var tag = new TagBuilder("div");
            tag.AddCssClass(inline ? "layui-inline" : "layui-form-item");

            var displayName = metadata.GetDisplayName();
            var isRequired = metadata.IsRequired;
            var placeholder = metadata.Placeholder;

            var isInteger = metadata.ModelType == typeof(Int32) || metadata.ModelType == typeof(Int16);
            var isDecimal = metadata.ModelType == typeof(decimal) || metadata.ModelType == typeof(float) || metadata.ModelType == typeof(double);
            var input = new TagBuilder("input");

            //长度标签
            //var length = "";
            //if (isNumber)
            //{
            //    length = "num";
            //}
            //else if (maxLength >= 50)
            //{
            //    length = "long";
            //}
            //else if (maxLength >= 20)
            //{
            //    length = "middle";
            //}
            //else
            //{
            //    length = "short";
            //}

            input.AddCssClass("layui-input");
            input.TagRenderMode = TagRenderMode.SelfClosing;
            if ("password".Equals(dataType))
            {
                input.MergeAttribute("type", InputType.Password.ToString());
            }
            else
            {
                input.MergeAttribute("type", InputType.Text.ToString());
            }
            input.MergeAttribute("name", name, replaceExisting: true);
            if (!string.IsNullOrEmpty(placeholder))
            {
                input.MergeAttribute("placeholder", placeholder);
            }

            //最大长度
            var maxLength = 0;
            var minLength = 0;
            var stringLengthAttribute = metadata.ValidatorMetadata.SingleOrDefault(x => x.GetType() == typeof(StringLengthAttribute));
            if (stringLengthAttribute != null)
            {
                maxLength = (stringLengthAttribute as StringLengthAttribute).MaximumLength;
                minLength = (stringLengthAttribute as StringLengthAttribute).MinimumLength;
            }
            var maxLengthAttribute = metadata.ValidatorMetadata.SingleOrDefault(x => x.GetType() == typeof(MaxLengthAttribute));
            if (maxLengthAttribute != null)
            {
                var vmax = (maxLengthAttribute as MaxLengthAttribute).Length;
                if (maxLength > vmax || maxLength == 0) maxLength = vmax;
            }
            var minLengthAttribute = metadata.ValidatorMetadata.SingleOrDefault(x => x.GetType() == typeof(MinLengthAttribute));
            if (minLengthAttribute != null)
            {
                var vmin = (minLengthAttribute as MinLengthAttribute).Length;
                if (minLength < vmin) minLength = vmin;
            }

            if (isInteger) maxLength = 11;
            if (isDecimal) maxLength = 32;
            if ("phonenumber".Equals(dataType) || "mobile".Equals(dataType)) maxLength = 11;
            input.MergeAttribute("maxlength", maxLength.ToString());

            //表单验证规则
            var verifys = new List<string>();
            if (isRequired && !isInteger && !isDecimal) verifys.Add("required");
            if (isInteger) verifys.Add("integer");
            if (isDecimal) verifys.Add("number");
            if ("phonenumber".Equals(dataType) || "mobile".Equals(dataType)) verifys.Add("phone");
            if ("url".Equals(dataType)) verifys.Add("url");
            if ("email".Equals(dataType) || "emailaddress".Equals(dataType)) verifys.Add("email");

            if (isInteger || isDecimal)
            {
                var rangeAttribute = metadata.ValidatorMetadata.SingleOrDefault(x => x.GetType() == typeof(RangeAttribute));
                if (rangeAttribute != null)
                {
                    input.MergeAttribute("data-val-range-min", (rangeAttribute as RangeAttribute).Minimum.ToString());
                    input.MergeAttribute("data-val-range-max", (rangeAttribute as RangeAttribute).Maximum.ToString());
                }
            }
            else
            {
                if (maxLength != 0 && minLength != 0)
                {
                    input.MergeAttribute("data-val-length-min", minLength.ToString());
                    input.MergeAttribute("data-val-length-max", maxLength.ToString());
                    verifys.Add("stringlength");
                }
            }
            //正则表达式验证
            var regularExpressionAttribute = metadata.ValidatorMetadata.SingleOrDefault(x => x.GetType() == typeof(RegularExpressionAttribute));
            if (regularExpressionAttribute != null)
            {
                var regular = regularExpressionAttribute as RegularExpressionAttribute;
                verifys.Add("regular");

                input.MergeAttribute("data-val-regular-pattern", regular.Pattern);
                input.MergeAttribute("data-val-regular-msg", regular.ErrorMessage);
            }

            if (verifys.Any())
            {
                input.MergeAttribute("lay-verify", string.Join('|', verifys.ToArray()));
            }

            if (modelExplorer.Model != null)
            {
                input.MergeAttribute("value", modelExplorer.Model.ToString());
            }

            MergeHtmlAttributes(input, htmlAttributes);

            tag.InnerHtml.AppendHtml($"<label class=\"layui-form-label\">{displayName}</label>");
            tag.InnerHtml.AppendHtml($"<div class=\"layui-input-inline\">");
            tag.InnerHtml.AppendHtml(input);
            tag.InnerHtml.AppendHtml("</div>");

            RenderDescription(tag, metadata);

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

        private IHtmlContent RenderSwitch(string name, ModelExplorer modelExplorer, ModelMetadata metadata, bool inline, object htmlAttributes)
        {
            var tag = new TagBuilder("div");
            tag.AddCssClass(inline ? "layui-inline" : "layui-form-item");

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

            MergeHtmlAttributes(input, htmlAttributes);

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
            if (!string.IsNullOrEmpty(metadata.Description))
            {
                tag.InnerHtml.AppendHtml($"<div class=\"layui-form-mid layui-word-aux\">{metadata.Description}</div>");
            }
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
            tag.InnerHtml.AppendHtml("<div class=\"layui-input-block long\">");
            tag.InnerHtml.AppendHtml(input);
            tag.InnerHtml.AppendHtml("</div>");
            return tag;
        }

        private IHtmlContent RenderEnumRadio(string name, ModelExplorer modelExplorer, ModelMetadata metadata, object htmlAttributes)
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
            var keyNames = metadata.EnumNamesAndValues;
            foreach (var enumItem in displays)
            {
                var itemName = enumItem.Key.Name;
                var itemValue = enumItem.Value;
                var itemKey = keyNames.SingleOrDefault(x => x.Value.Equals(itemValue)).Key;
                var input = new TagBuilder("input");
                input.GenerateId(name + "_" + itemValue, "");
                input.TagRenderMode = TagRenderMode.SelfClosing;
                input.MergeAttribute("type", InputType.Radio.ToString());
                input.MergeAttribute("name", name, replaceExisting: true);
                input.MergeAttribute("value", itemValue);
                input.MergeAttribute("title", itemName);
                input.MergeAttribute("key", itemKey);
                if (itemValue.Equals(value))
                {
                    input.MergeAttribute("checked", "checked");
                }
                MergeHtmlAttributes(input, htmlAttributes);
                tag.InnerHtml.AppendHtml(input);
            }

            tag.InnerHtml.AppendHtml("</div>");
            return tag;
        }

        private IHtmlContent RenderDateTime(string name, ModelExplorer modelExplorer, bool inline, object htmlAttributes)
        {
            ModelMetadata metadata = modelExplorer.Metadata;
            var tag = new TagBuilder("div");
            tag.AddCssClass(inline ? "layui-inline" : "layui-form-item");

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

            MergeHtmlAttributes(input, htmlAttributes);

            tag.InnerHtml.AppendHtml($"<label class=\"layui-form-label\">{displayName}</label>");
            tag.InnerHtml.AppendHtml("<div class=\"layui-input-inline\">");
            tag.InnerHtml.AppendHtml(input);
            tag.InnerHtml.AppendHtml("</div>");

            RenderDescription(tag, metadata);

            return tag;
        }

        #endregion

        #region Show

        private IHtmlContent RenderItemShow<TResult>(Expression<Func<TModel, TResult>> expression, bool inline, string dataType, object htmlAttributes)
        {
            if (expression == null)
            {
                return new HtmlString("");
            }

            var modelExplorer = GetModelExplorer(expression);
            var metadata = modelExplorer.Metadata;
            var name = GetExpressionName(expression);
            Type type = GetRealType(modelExplorer.ModelType);

            IHtmlContent content = null;
            var dataTypeName = string.IsNullOrEmpty(dataType) ? GetDataTypeName(type, metadata) : dataType.ToLower();

            switch (dataTypeName)
            {
                case "text":
                    content = RenderLable(modelExplorer, inline);
                    break;
                case "hidden":
                    content = new HtmlString("");
                    break;
                case "password":
                    content = new HtmlString("");
                    break;
                case "switch":
                    content = RenderSwitchShow(name, modelExplorer, metadata, inline, htmlAttributes);
                    break;
                case "multilinetext":
                case "textarea":
                    content = RenderTextareaShow(name, modelExplorer, metadata);
                    break;
                case "enum_radio":
                    content = RenderEnumRadioShow(modelExplorer, inline);
                    break;
                case "datetime":
                case "date":
                case "year":
                case "month":
                case "time":
                    content = RenderDateTimeShow(name, modelExplorer, metadata, inline);
                    break;
                case "image":
                case "imageurl":
                    content = RenderImageShow(name, modelExplorer, metadata);
                    break;
                default:
                    content = RenderLable(modelExplorer, inline);
                    break;
            }

            return content;
        }

        private IHtmlContent RenderLable(ModelExplorer modelExplorer, bool inline)
        {
            ModelMetadata metadata = modelExplorer.Metadata;

            var tag = new TagBuilder("div");
            tag.AddCssClass(inline ? "layui-inline" : "layui-form-item");

            var displayName = metadata.GetDisplayName();

            var label = new TagBuilder("label");
            label.AddCssClass("layui-form-label-show");
            if (modelExplorer.Model != null)
            {
                var formate = metadata.DisplayFormatString;
                if (!string.IsNullOrEmpty(formate))
                {
                    Type type = GetRealType(modelExplorer.ModelType);
                    switch (modelExplorer.Model)
                    {
                        case int v:
                            label.InnerHtml.Append(v.ToString(formate));
                            break;
                        case float v:
                            label.InnerHtml.Append(v.ToString(formate));
                            break;
                        case decimal v:
                            label.InnerHtml.Append(v.ToString(formate));
                            break;
                        default:
                            label.InnerHtml.Append(modelExplorer.Model.ToString());
                            break;
                    }
                }
                else
                {
                    label.InnerHtml.Append(modelExplorer.Model.ToString());
                }
            }

            tag.InnerHtml.AppendHtml($"<label class=\"layui-form-label\">{displayName}</label>");
            tag.InnerHtml.AppendHtml($"<div class=\"{(inline ? "layui-input-inline" : "layui-input-block")}\">");
            tag.InnerHtml.AppendHtml(label);
            tag.InnerHtml.AppendHtml("</div>");
            return tag;
        }

        private IHtmlContent RenderDateTimeShow(string name, ModelExplorer modelExplorer, ModelMetadata metadata, bool inline)
        {
            var tag = new TagBuilder("div");
            tag.AddCssClass(inline ? "layui-inline" : "layui-form-item");

            var displayName = metadata.GetDisplayName();
            var placeholder = metadata.Placeholder;

            var label = new TagBuilder("label");
            label.AddCssClass("layui-form-label-show");

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

        private IHtmlContent RenderEnumRadioShow(ModelExplorer modelExplorer, bool inline)
        {
            ModelMetadata metadata = modelExplorer.Metadata;

            var tag = new TagBuilder("div");
            tag.AddCssClass(inline ? "layui-inline" : "layui-form-item");

            if (!metadata.IsEnum)
            {
                throw new Exception("type is not enum.");
            }
            var displayName = metadata.GetDisplayName();

            tag.InnerHtml.AppendHtml($"<label class=\"layui-form-label\">{displayName}</label>");
            tag.InnerHtml.AppendHtml($"<div class=\"{(inline ? "layui-input-inline" : "layui-input-block")}\">");

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

        private IHtmlContent RenderSwitchShow(string name, ModelExplorer modelExplorer, ModelMetadata metadata, bool inline, object htmlAttributes)
        {
            var tag = new TagBuilder("div");
            tag.AddCssClass(inline ? "layui-inline" : "layui-form-item");

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
            tag.InnerHtml.AppendHtml($"<div class=\"{(inline ? "layui-input-inline" : "layui-input-block")}\">");

            var lable = new TagBuilder("label");
            lable.AddCssClass("layui-form-label-show");

            MergeHtmlAttributes(lable, htmlAttributes);

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

        private IHtmlContent RenderTextareaShow(string name, ModelExplorer modelExplorer, ModelMetadata metadata)
        {
            var tag = new TagBuilder("div");
            tag.AddCssClass("layui-form-item");

            var displayName = metadata.GetDisplayName();

            var lable = new TagBuilder("label");
            lable.AddCssClass("layui-form-label-show");
            if (modelExplorer.Model != null)
            {
                var ars = modelExplorer.Model.ToString().Split(new string[] { "\r\n", "\n"}, StringSplitOptions.None);
                foreach (var item in ars)
                {
                    lable.InnerHtml.Append(item);
                    lable.InnerHtml.AppendHtml("<br/>");
                }
            }

            tag.InnerHtml.AppendHtml($"<label class=\"layui-form-label\">{displayName}</label>");
            tag.InnerHtml.AppendHtml("<div class=\"layui-input-block\">");
            tag.InnerHtml.AppendHtml(lable);
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

        private string JsonSerialize(object obj)
        {
            if (obj == null)
            {
                return "";
            }
            return JsonConvert.SerializeObject(obj);
        }

        private void MergeHtmlAttributes(TagBuilder tag, object htmlAttributes)
        {
            if (tag == null || htmlAttributes == null)
            {
                return;
            }
            var attrs = AnonymousObjectToHtmlAttributes(htmlAttributes);
            foreach (var attr in attrs)
            {
                if (attr.Value == null)
                {
                    tag.MergeAttribute(attr.Key, null, replaceExisting: true);
                }
                else if (attr.Value is string s)
                {
                    tag.MergeAttribute(attr.Key, s, replaceExisting: true);
                }
                else if (attr.Value is int
                    || attr.Value is decimal
                    || attr.Value is float
                    || attr.Value is bool
                    || attr.Value is short)
                {
                    tag.MergeAttribute(attr.Key, attr.Value.ToString(), replaceExisting: true);
                }
                else
                {
                    tag.MergeAttribute(attr.Key, JsonSerialize(attr.Value), replaceExisting: true);
                }
            }
        }

        private string GetDataTypeName(Type type, ModelMetadata metadata)
        {
            string dataTypeName = metadata.DataTypeName;

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
            return dataTypeName.ToLower();
        }

        private void RenderDescription(TagBuilder tag, ModelMetadata metadata)
        {
            var description = metadata.Description;
            //if (string.IsNullOrEmpty(description) && metadata is ModelBinding.Metadata.DefaultModelMetadata md)
            //{
            //    var descAttribute = md.Attributes.Attributes.FirstOrDefault(x=>x.GetType() == typeof(System.ComponentModel.DescriptionAttribute));
            //    if (descAttribute != null)
            //    {
            //        description = ((System.ComponentModel.DescriptionAttribute)descAttribute).Description;
            //    }
            //}
            if (!string.IsNullOrEmpty(description))
            {
                tag.InnerHtml.AppendHtml($"<div class=\"layui-form-mid layui-word-aux\">{metadata.Description}</div>");
            }
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
