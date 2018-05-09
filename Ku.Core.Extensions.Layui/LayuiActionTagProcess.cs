using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Vino.Core.Extensions.Ui;

namespace Vino.Core.Extensions.Layui
{
    public class LayuiActionTagProcess : IActionTagProcess
    {
        protected readonly LayuiOption _layuiOption;

        public LayuiActionTagProcess(IOptions<LayuiOption> layuiOption)
        {
            this._layuiOption = layuiOption.Value;
        }

        public async Task ProcessAsync(ActionTagHelper helper, TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "button";
            output.TagMode = TagMode.StartTagAndEndTag;
            var cls = "layui-btn";

            var hasThemeCss = !string.IsNullOrEmpty(helper.Css) && (helper.Css.Contains("layui-btn-primary") 
                || helper.Css.Contains("layui-btn-normal") || helper.Css.Contains("layui-btn-warm") 
                || helper.Css.Contains("layui-btn-danger") || helper.Css.Contains("layui-btn-disabled") 
                || helper.Css.Contains("layui-btn-default"));
            var hasSizeCss = !string.IsNullOrEmpty(helper.Css) && (helper.Css.Contains("layui-btn-lg")
                || helper.Css.Contains("layui-btn-sm") || helper.Css.Contains("layui-btn-xs"));

            if (!hasThemeCss && !string.IsNullOrEmpty(_layuiOption.ActionTagTheme))
            {
                cls += $" {_layuiOption.ActionTagTheme}";
            }
            if (!string.IsNullOrEmpty(_layuiOption.ActionTagSize) && !hasSizeCss)
            {
                cls += $" {_layuiOption.ActionTagSize}";
            }

            if (!string.IsNullOrEmpty(helper.Css))
            {
                cls += $" {helper.Css}";
            }
            var type = "button";
            if (context.AllAttributes.TryGetAttribute("type", out TagHelperAttribute attr))
            {
                type = attr.Value.ToString();
            }
            output.Attributes.SetAttribute("class", cls);
            output.Attributes.SetAttribute("type", type);
            output.Attributes.SetAttribute("title", helper.Title ?? helper.Text ?? "");

            var content = "";
            if (!string.IsNullOrEmpty(helper.Icon))
            {
                content = $"<i class=\"layui-icon\">{helper.Icon}</i>";
            }
            if (!string.IsNullOrEmpty(helper.Text))
            {
                content += helper.Text;
            }
            output.Content.SetHtmlContent(content);
        }
    }
}
