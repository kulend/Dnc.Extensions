# Vino.Core.Extensions.Layui

<p>
    如果您的.net core 2.0项目使用layui来渲染表单，那么您可以尝试使用这个扩展库可以帮你减少代码和工作量。
</p>

* 版本发布
    <p>[2017.08.28] 版本 2.0.2.0</p>

* 安装方法
PM> Install-Package Vino.Core.Extensions.Layui -Version 2.0.2
> dotnet add package Vino.Core.Extensions.Layui --version 2.0.2

* 使用方法
    1. 在Startup的ConfigureServices方法中
    ```c#
            //使用Layui
            services.AddLayui();
    ```

    2. _ViewImports.cshtml文件中添加
    ```c#
        @addTagHelper *, Vino.Core.Extensions.Layui
    ```

    3. 定义Model及其属性，例如：
    ```c#
    public class DemoDto
    {
        /// <summary>
        /// ID
        /// </summary>
        [DataType("hidden")]
        public long Id { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [Required, MaxLength(20)]
        [Display(Name = "名称")]
        public string Name { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        [MaxLength(256)]
        [Display(Name = "链接")]
        public string Url { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [Display(Name = "排序值")]
        public int OrderIndex { get; set; } = 0;

        /// <summary>
        /// 是否显示
        /// </summary>
        [Display(Name = "是否显示", Prompt = "是|否")]
        public bool IsShow { get; set; } = true;

        [Display(Name = "时间")]
        [DisplayFormat(DataFormatString = "yyyy-MM-dd HH:mm:ss")]
        [DataType(DataType.DateTime)]
        public DateTime? Date { set; get; }

        [Display(Name = "年份")]
        [DisplayFormat(DataFormatString = "yyyy年")]
        [DataType("year")]
        public int Year { set; get; }

        [Display(Name = "月份")]
        [DisplayFormat(DataFormatString = "现在是yyyy年MM月")]
        [DataType("month")]
        public string Month { set; get; }

        /// <summary>
        /// 性别
        /// </summary>
        [Display(Name = "性别")]
        public EmSex Sex { set; get; }
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

    4. 在form中使用@Html.LayuiInputFor，如
    ```c#
        @using (Html.LayuiBeginForm<DemoDto>("Save"))
        {
            @Html.LayuiInputFor(x => x.Id)
            @Html.LayuiInputFor(x => x.Name)
            @Html.LayuiInputFor(x => x.Url)
            @Html.LayuiInputFor(x => x.IsShow)
            @Html.LayuiInputFor(x => x.OrderIndex)
            @Html.LayuiInputFor(x => x.Date)
            @Html.LayuiInputFor(x => x.Year)
            @Html.LayuiInputFor(x => x.Month)
            @Html.LayuiInputFor(x => x.Sex)
            @Html.LayuiFormActionsForSubmitAndClose()
        }
    ```
    Ok, 是不是很简单？

* 其他说明
    1. 设定字段类型是int或short，这input标签会添加input-length-num样式css，如果是字符串字段，设定MaxLengthAttribute，长度>=50，添加样式"input-length-long",如果长度20~50，添加样式"input-length-middle"，如果长度小于20，则添加样式"input-length-short"。用户可在css文件中自定义文本框宽度。
    2. bool型字段默认会渲染成switch，lay-text需要这样设置：
    ```c#
        [Display(Name = "是否显示", Prompt = "是|否")]
    ```
    3. 除了字段类型，很多时候需要DataTypeAttribute来判断输入类型，默认为text，还可设置hidden，password，multilinetext，textarea，datetime，date，year，month，time等。multilinetext和textarea会渲染成Textarea。

    4. 关于FormActions按钮相关，
        1). @Html.LayuiFormActionsForSubmit(), 添加保存按钮。
        2). @Html.LayuiFormActionsForSubmitAndClose(), 添加保存和关闭按钮。
        3). @Html.LayuiFormActionsForSubmitAndReset(), 添加保存和重置按钮。
        4). @Html.LayuiFormActions(btns), 添加自定义按钮，例如：
    ```c#
        @Html.LayuiFormActions(
           new LayuiSubmitActionButton(),
           new LayuiCloseActionButton(),
           new LayuiResetActionButton { Text = "重置表单" },
           new LayuiActionButton { Id = "btn_aaa", Text = "自定义按钮", Css = "my-btn", OnClick = "alert(1);" }
        )
    ``` 
    5. 修改全局默认配置
    ```c#
            //使用Layui
            services.AddLayui(opt => {
                opt.SubmitButtonText = "保 存"; //默认提交按钮文字
                opt.CloseButtonOnClick = "关 闭";//默认关闭按钮文字
                opt.ResetButtonText = "重 置";//默认重置表单按钮文字
                opt.CloseButtonOnClick = "closeWindow()";//默认关闭按钮OnClick事件
            });
    ```
* 如有bug或其他建议，可以提交issue。也可邮件给我：kulend@qq.com

