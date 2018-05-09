# Ku.Core.Extensions.Layui

<p>
    2018年5月9日，由原先的Vino.Core.Extensions.Layui更名为Ku.Core.Extensions.Layui
</p>

<p>
    如果您的.net core 2.0项目使用layui来渲染表单，那么您可以尝试使用这个扩展库可以帮你减少代码和工作量。
</p>

* 版本发布
	<p>[2018.05.09] 版本 2.2.0.0</p>
	<p>[2017.08.29] 版本 2.0.2.1</p>
    <p>[2017.08.28] 版本 2.0.2.0</p>

* 安装方法
<br/> 	先安装Ku.Core.Extensions.Ui
<br/> PM> Install-Package Ku.Core.Extensions.Ui -Version 2.2.0.0
<br/> dotnet add package Ku.Core.Extensions.Ui --version 2.2.0.0
	再安装Ku.Core.Extensions.Layui
<br/> PM> Install-Package Ku.Core.Extensions.Layui -Version 2.2.0.0
<br/> dotnet add package Ku.Core.Extensions.Layui --version 2.2.0.0

* 使用方法
    1. 在Startup的ConfigureServices方法中
    ```c#
        //使用Layui
        services.AddLayui();
    ```

    2. _ViewImports.cshtml文件中添加
    ```c#
        @addTagHelper *, Ku.Core.Extensions.Layui
    ```

    3. 定义Model及其属性，例如：
    ```c#
    public class DemoModel
    {
        /// <summary>
        /// ID
        /// </summary>
        [DataType("hidden")]
        public long Id { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [Required, MaxLength(20), MinLength(5)]
        [Display(Name = "名称", Description = "附加说明文字")]
        [RegularExpression(@"^[a-zA-Z''-'\s]{1,40}$", ErrorMessage = "输入的名称不符合规则")]
        public string Name { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [Required]
        [Display(Name = "手机号")]
        [DataType(DataType.PhoneNumber)] //也可以为[DataType("mobile")]
        public string Mobile { get; set; }

        /// <summary>
        /// 链接
        /// </summary>
        [MaxLength(256)]
        [Display(Name = "链接")]
        [DataType(DataType.Url)]
        public string Url { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        [MaxLength(256)]
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        /// <summary>
        /// 序号
        /// </summary>
        [Display(Name = "序号", Prompt ="0~9999")]
        public int OrderIndex { get; set; } = 0;

        /// <summary>
        /// 序号
        /// </summary>
        [Display(Name = "数字", Prompt = "0~9999")]
        [Range(-1, 999.05)]
        public decimal Dec { get; set; } = 0;

        /// <summary>
        /// 开关
        /// </summary>
        [Display(Name = "开关", Prompt = "开|关")]
        public bool Switch { get; set; } = true;

        [Display(Name = "时间")]
        [DisplayFormat(DataFormatString = "yyyy-MM-dd HH:mm:ss")] //定义显示格式
        [DataType(DataType.DateTime)] //也可以这样[DataType("datetime")]
        public DateTime? Date { set; get; }

        [Display(Name = "年份")]
        [DataType("year")]
        public int Year { set; get; }

        [Display(Name = "月份")]
        [DisplayFormat(DataFormatString = "现在是yyyy年MM月")]
        [DataType("month")]
        public string Month { set; get; }

        /// <summary>
        /// 枚举
        /// </summary>
        [Display(Name = "性别")]
        public EmSex Sex { set; get; }

        /// <summary>
        /// 多行文本
        /// </summary>
        [Display(Name = "备注", Prompt = "请输入您的备注信息...")]
        [MaxLength(500)]
        [DataType(DataType.MultilineText)] //或者[DataType("textarea")]
        public string Remark { set; get; }
    }

    /// <summary>
    /// 性别
    /// </summary>
    public enum EmSex : short
    {
        [Display(Name = "保密")]
        Secret = 0,

        [Display(Name = "男")]
        Male = 1,

        [Display(Name = "女")]
        Female = 2
    }
    ```

    4. 在form中使用@Html.InputFor，如
    ```c#
                @using (Html.BeginForm<DemoModel>("#"))
                {
                    @Html.InputFor(x => x.Id)
                    @Html.InputFor(x => x.Name, x => x.Mobile)
                    @Html.InputFor(x => x.Url, x => x.Email)
                    @Html.InputFor(x => x.OrderIndex, x => x.Dec)
                    @Html.InputFor(x => x.Sex)
                    @Html.InputFor(x => x.Switch, x => x.Date)
                    @Html.InputFor(x => x.Year, x => x.Month)
                    @Html.InputFor(x => x.Remark)
                    @Html.ActionsForSubmitAndReset()
                }
    ```
    5. 在form中使用@Html.ShowFor，如
    ```c#
                @using (Html.BeginForm<DemoModel>("#"))
                {
                    @Html.InputFor(x => x.Id)
                    @Html.ShowFor(x => x.Name, x => x.Mobile)
                    @Html.ShowFor(x => x.Url, x => x.Email)
                    @Html.ShowFor(x => x.OrderIndex, x => x.Dec)
                    @Html.ShowFor(x => x.Sex)
                    @Html.ShowFor(x => x.Switch, x => x.Date)
                    @Html.ShowFor(x => x.Year, x => x.Month)
                    @Html.ShowFor(x => x.Remark)
                }
    ```
	使用ShowFor需要在页面添加以下css代码：
    ```css
    <style>
        .layui-form-item .layui-form-label-show {
            float: left;
            display: block;
            padding: 9px 15px;
            font-weight: 400;
            text-align: left;
            color: #808080;
        }

        .layui-form-item .layui-input-block .layui-form-label-show {
            width: 85%;
        }
    </style>
    ```

	6.表单验证
	<br>    Model字段添加特定的特性或指定DataType即可实现表单自动验证。使用验证页面需引入form.verify.js，文件在Tests/Ku.Core.Extensions.Layui.Test/wwwroot/js目录下有。
	<br>Required特性：不能为空
	<br>MaxLength特性：最大长度验证
	<br>MinLength特性：最小长度验证
	<br>StringLength特性：最大和最小长度验证
	<br>Range特性：最大和最小值验证
	<br>DataType(DataType.PhoneNumber)或DataType("mobile")：手机号验证
	<br>DataType(DataType.Url)：网址验证
	<br>DataType(DataType.EmailAddress)或DataType("email")：Email验证
	<br>字段类型为int：整数验证
	<br>字段类型为decimal：数字验证
	<br>RegularExpression特性：自定义正则表达式验证，如果[RegularExpression(@"^[a-zA-Z''-'\s]{1,40}$", ErrorMessage = "输入的名称不符合规则")]

    Ok, 是不是很简单？

* 其他说明
    1. 去掉之前版本中的和长度相关css。

    2. bool型字段默认会渲染成switch，lay-text需要这样设置：
    ```c#
        [Display(Name = "开关", Prompt = "开|关")]
    ```

    3. 除了字段类型，很多时候需要DataTypeAttribute来判断渲染方式，默认为text，还可设置hidden，password，multilinetext，textarea，datetime，date，year，month，time等。multilinetext和textarea会渲染成Textarea。


    4. 关于Action按钮，
        <br/> 1). @Html.ActionsForSubmit(), 添加保存按钮（提交表单）。
        <br/> 2). @Html.ActionsForSubmitAndClose(), 添加保存和关闭按钮。
        <br/> 3). @Html.ActionsForSubmitAndReset(), 添加保存和重置按钮。
        <br/> 4). @Html.ActionsFor(btns), 添加自定义按钮，例如：
    ```c#
        @Html.ActionsFor(
                new SubmitButton(),
                new CloseButton(),
                new ResetButton { Text = "重置表单" },
                new ActionButton { Id = "btn_test", Text = "自定义按钮", Css = "btn-test", OnClick = "alert(1);" }
            )
    ``` 

    5. 修改全局默认配置
    ```c#
            //使用Layui
            services.AddLayui(opt => {
                opt.ActionsInFormItem = false;
                opt.ActionTagTheme = "layui-btn-primary";
                opt.ActionTagSize = "layui-btn-sm";
            });
    ```

    6. 关于laydate组件，需要在页面添加以下js脚本：
    ```c#
    layui.use(['laydate'], function () {
        var $ = layui.jquery
        , laydate = layui.laydate;

        $(".layui-input.laydate").each(function () {
            var self = $(this);
            var type = self.data("type") || 'date';
            var format = self.data("format") || 'yyyy-MM-dd';
            laydate.render({
                elem: self[0],
                type: type,
                format: format
            });
        });
    });
    ```


* 如有bug或其他建议，可以提交issue。也可邮件给我：kulend@qq.com

