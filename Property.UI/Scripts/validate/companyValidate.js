$(document).ready(function ()
{
    $("#form").validate({
        rules: {
            Name: {
                required: true,
                maxlength: 50,
            },
            Address: {
                maxlength: 200
            },
            Tel: {
                maxlength: 30,
                TelCheck:true
            },
            UploadImg: {
                required:true
            }
        },
        messages: {
            Name: {
                required: "请输入物业公司名称",
                maxlength: "长度不能超过50位",
                remote:"物业公司已存在"
            },
            Address: {
                maxlength: "长度不能超过200位"
            },
            Tel: {
                maxlength: "长度不能超过30位",
                TelCheck: "请正确填写联系电话"
            },
            UploadImg: {
                required: "请选择图片"
            },
        }
    });
});