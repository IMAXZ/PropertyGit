
$(document).ready(function () {
    $("#form").validate({
        rules: {
            TrueName: {
                maxlength: 30
            },
            Email: {
                required: true,
                maxlength: 50,
                email: true
            },
            Tel: {
                maxlength: 30,
                TelCheck: true
            },
            Phone: {
                maxlength: 15,
                PhoneCheck: true
            },
            UserMemo: {
                maxlength: 200
            }
        },
        messages: {
            TrueName: {
                maxlength: "长度不能超过30位"
            },
            Email: {
                required: "请输入电子邮箱",
                maxlength: "长度不能超过50位",
                email: "请输入格式正确的邮箱地址"
            },
            Tel: {
                maxlength: "长度不能超过30位"
            },
            Phone: {
                maxlength: "长度不能超过15位"
            },
            UserMemo: {
                maxlength: "长度不能超过200位"
            }
        }
    });
});

