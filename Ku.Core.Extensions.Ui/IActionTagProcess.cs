using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Vino.Core.Extensions.Ui
{
    public interface IActionTagProcess
    {
        Task ProcessAsync(ActionTagHelper helper, TagHelperContext context, TagHelperOutput output);
    }
}
