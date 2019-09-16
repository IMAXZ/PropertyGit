$(document).ready(function () {
    $("#form").validate({
        rules: {
            Name: {
                required: true,
                maxlength: 50,
                remote: {
                    type: "POST",
                    url: "/PropertyCompany/RemoteCheck",
                    date: {
                        Id: function () { return $("#Id"); },
                        Name: function () { return $("#Name"); }
                    }
                }
            },
            Address: {
                maxlength: 200
            },
            Tel: {
                maxlength: 30,
                TelCheck: true
            }
        },
        messages: {
            Name: {
                required: "请输入物业公司名称",
                maxlength: "长度不能超过50位",
                remote: "物业公司已存在"
            },
            Address: {
                maxlength: "长度不能超过200位"
            },
            Tel: {
                maxlength: "长度不能超过30位",
                TelCheck: "请正确填写联系电话"
            },
        }
    });
});