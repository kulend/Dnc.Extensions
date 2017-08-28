using System;
using System.Collections.Generic;
using System.Text;

namespace Vino.Core.Extensions.Layui
{
    public class LayuiOption
    {
        public string SubmitButtonText { set; get; } = "保 存";

        public string CloseButtonText { set; get; } = "关 闭";

        public string ResetButtonText { set; get; } = "重 置";

        public string CloseButtonOnClick { set; get; } = "closeWindow()";
    }
}
