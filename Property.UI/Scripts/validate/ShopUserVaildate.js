
$(document).ready(function () {
    $("#form").validate({
        rules: {
            UserName: {
                required: true,
                maxlength: 50,
                LetterOrNumber: true,
                remote: {
                    type: "POST",
                    url: "/ShopUser/RemoteCheckExist",
                    data: {
                        Id: function () { return $("#Id").val(); },
                        userName: function () { return $("#UserName").val(); }
                    }
                }
            },
            Email: {
                required: true,
                maxlength: 50,
                email: true
            },
            Password: {
                required: true,
                LetterAndNumber: true,
                maxlength: 32,
                minlength: 6
            },
            ConfirmPassword: {
                required: true,
                LetterAndNumber: true,
                maxlength: 32,
                minlength: 6,
                equalTo: "#Password"
            },
            Phone: {
                required: true,
                maxlength: 15,
                PhoneCheck: true
            },
            Memo: {
                maxlength: 200
            }
        },
        messages: {
            UserName: {
                required: "请输入用户名称",
                maxlength: "长度不能超过50位",
                remote: "该用户名已存在"
            },
            Password: {
                required: "请输入登录密码",
                maxlength: "长度不能超过32位",
                minlength: "长度不能小于6位"
            },
            ConfirmPassword: {
                required: "请输入确认密码",
                maxlength: "长度不能超过32位",
                minlength: "长度不能小于6位",
                equalTo: "密码前后不一致"
            },
            Email: {
                required: "请输入电子邮箱",
                maxlength: "长度不能超过50位",
                email: "请输入格式正确的邮箱地址"
            },
            Phone: {
                required: "请输入手机号码",
                maxlength: "长度不能超过15位"
            },
            Memo: {
                maxlength: "长度不能超过200位"
            }
        }
    });
});


