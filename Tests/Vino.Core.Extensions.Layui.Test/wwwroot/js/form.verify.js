layui.use(['form'], function () {
    var form = layui.form;
    var $ = layui.jquery;

    form.verify({
        username: function (value, item) { //value：表单的值、item：表单的DOM对象
            if (!new RegExp("^[a-zA-Z0-9_\u4e00-\u9fa5\\s·]+$").test(value)) {
                return '用户名不能有特殊字符';
            }
            if (/(^\_)|(\__)|(\_+$)/.test(value)) {
                return '用户名首尾不能出现下划线\'_\'';
            }
            if (/^\d+\d+\d$/.test(value)) {
                return '用户名不能全为数字';
            }
        }

        //我们既支持上述函数式的方式，也支持下述数组的形式
        //数组的两个值分别代表：[正则匹配、匹配不符时的提示文字]
        , pass: [
            /^[\S]{6,12}$/
            , '密码必须6到12位，且不能出现空格'
        ],

        //邮箱地址
        email: function (value, item) {
            if (value && !/^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$/.test(value)) {
                return '邮箱格式不正确';
            }
        },
        //链接地址
        url: function (value, item) {
            if (value && !/(^#)|(^http(s*):\/\/[^\s]+\.[^\s]+)/.test(value)) {
                return '链接格式不正确';
            }
        },
        //手机号码
        phone: function (value, item) {
            if (value && !/^(((13[0-9]{1})|(14[0-9]{1})|(15[0-9]{1})|(17[0-9]{1})|(18[0-9]{1}))+\d{8})$/.test(value)) {
                return '手机号码格式不正确';
            }
        },
        //整数
        integer: function (value, item) {
            if (value) {
                if (!/^[-+]?\d*$/.test(value)) {
                    return '数据格式不正确';
                }
                var v = parseInt(value);
                var min = $(item).data("val-range-min");
                if (min != undefined && v < min) {
                    return `输入的值不能小于${min}`;
                }
                var max = $(item).data("val-range-max");
                if (max != undefined && v > max) {
                    return `输入的值不能大于${max}`;
                }
            }
        },
        //数值
        number: function (value, item) {
            if (value) {
                if (isNaN(value)) {
                    return '数据格式不正确';
                }
                var v = parseFloat(value);
                var min = $(item).data("val-range-min");
                if (min != undefined && v < min) {
                    return `输入的值不能小于${min}`;
                }
                var max = $(item).data("val-range-max");
                if (max != undefined && v > max) {
                    return `输入的值不能大于${max}`;
                }
            }
        },
        //长度
        stringlength: function (value, item) {
            if (value) {
                var min = $(item).data("val-length-min");
                if (min != undefined && value.length < min) {
                    return `输入的值长度不能小于${min}，当前长度${value.length}`;
                }
                var max = $(item).data("val-length-max");
                if (max != undefined && value.length > max) {
                    return `输入的值长度不能大于${max}，当前长度${value.length}`;
                }
            }
        },
        //正则表达式
        regular: function (value, item) {
            if (value) {
                var pattern = $(item).data("val-regular-pattern");
                var msg = $(item).data("val-regular-msg");
                if (pattern != undefined && pattern.length > 0) {
                    if (pattern.indexOf("custom") == 0) {
                        let js = pattern.substring(7);
                        if (!eval(js)) {
                            return msg || "输入的值不符合规则";
                        }
                    } else {
                        if (!new RegExp(pattern).test(value)) {
                            return msg || "输入的值不符合规则";
                        }
                    }
                }
            }
        }
    });

});