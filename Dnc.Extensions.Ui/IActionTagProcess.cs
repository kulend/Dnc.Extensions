using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Dnc.Extensions.Ui.Layui
{
    public interface IActionTagProcess
    {
        Task ProcessAsync(ActionTagHelper helper, TagHelperContext context, TagHelperOutput output);
    }
}
