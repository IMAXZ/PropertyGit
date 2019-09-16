$(document).ready(function () {
    $("#form").validate({
        rules: {
            Name: {
                required: true,
                maxlength: 30,
                remote: {
                    type: "POST",
                    url: "/BuildCompany/RemoteCheckExist",
                    data: {
                        Id: function () { return $("#UserId").val(); },
                        UserName: function () { return $("#UserName").val(); }
                    }
                }
            },
            Phone: {
                required: true,
                maxlength: 15,
                TelCheck: true
            },
            
            Desc: {
                
                //required: true,
                maxlength: 500
            },
            PayDesc: {
                //required: true,
                maxlength: 500
            },
            ServiceDesc: {
               // required: true,
                maxlength: 500.000
            }
        },
        messages: {
            Name: {
                required: "请输入单位名称",
                maxlength: "长度不能超过30位",
                remote:"该单位名称已经存在"
            },
            Phone: {
                required: "请输入电话",
                maxlength: "长度不能超过15位",
                TelCheck: "请正确填写手机"
            },

            Desc: {
                required: "请输入业主备注",
                maxlength: "长度不能超过500位"
            },
            PayDesc: {
                //required: "请输入缴费备注",
                maxlength: "长度不能超过500位"
            },
            ServiceDesc: {
                //required: "请输入服务备注",
                maxlength: "长度不能超过500位"
            }
        }
    });
});

