$(document).ready(function () {
    $("#form").validate({
        rules: {
            UserName: {
                required: true,
                maxlength: 50,
                CorrectUserNameAndPasswordFormat: UserName.value,
            },
            TrueName: {
                required: true,
                maxlength: 30
            },
            Phone: {
                maxlength: 15,
                PhoneCheck: Phone.value
            },
            Tel:{
                maxlength: 30,
                TelCheck: Tel.value
            },
            Email: {
                required: true,
                maxlength: 50,
                email: true
            },
            Password: {
                required: true,
                maxlength: 32,
                CorrectUserNameAndPasswordFormat: Password.value
            },
            UserMemo: {
                maxlength: 200
            }
        },
        messages:{
            UserName:{
                required: "请输入用户名称",
                maxlength: "长度不能超过50位",

            },
            TrueName: {
                required: "请输入姓名",
                maxlength: "长度不能超过30位"
            },
            Phone: {
                maxlength: "长度不能超过15位"
            },
            Tel: {
                maxlength: "长度不能超过30位"
            },
            Email: {
                required: "请输入电子邮箱",
                maxlength: "长度不能超过50位",
                email: "请输入格式正确的邮箱地址"
            },
            Password: {
                required: "请输入登录密码",
                maxlength: "长度不能超过32位"
            },
            Memo: {
                maxlength: "长度不能超过200位"
            }
        }
    });
});
$.validator.addMethod("CorrectPhoneNumber", function (value, element) {
    var score = /\d{3}-\d{8}|\d{4}-\d{7}/;
    return score.test(value);
}, "请输入正确格式的电话号码 如0532-1234567");

$.validator.addMethod("CorrectTelNumber", function (value, element) {
    var score = /^(13[0-9]|14[5|7]|15[0|1|2|3|5|6|7|8|9]|18[0|1|2|3|5|6|7|8|9])\d{8}$/;
    return score.test(value);
}, "请输入正确格式的手机号码 如139666888");

$.validator.addMethod("CorrectUserNameAndPasswordFormat", function (value, element) {
    var score = /^[A-Za-z0-9]+$/;
    return score.test(value);
}, "只允许输入大小写字母、数字");

$.validator.addMethod("CorrectTrueNameFormat", function (value, element) {
    var score = /^[\u4e00-\u9fa5]{0,}$/;
    return score.test(value);
}, "只允许输入汉字");
