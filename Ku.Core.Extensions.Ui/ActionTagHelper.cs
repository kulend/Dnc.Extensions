using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Ku.Core.Extensions.Ui
{
    public class ActionTagHelper : TagHelper
    {
        private IActionTagProcess _process;

        public ActionTagHelper(IActionTagProcess process)
        {
            this._process = process;
        }

        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        public string Text { set; get; } = "";
        public string Css { set; get; } = "";
        public string Icon { set; get; }
        public string Title { set; get; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            await _process.ProcessAsync(this, context, output);
        }
    }
}
