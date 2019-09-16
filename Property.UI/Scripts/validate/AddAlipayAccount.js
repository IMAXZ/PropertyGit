$(document).ready(function ()
{
    $("#form").validate({
        rules: {
            AlipayNumber: {
                required: true,
                maxlength: 100
            },
            AlipayMerchantNo: {
                required: true,
                maxlength: 100
            },
            PrivatePath: {
                required:true
            },
            PublicPath: {
                required:true
            }
        },
        messages: {
            AlipayNumber: {
                required: "请输入支付宝账户",
                maxlength: "长度不能超过100位"
            },
            AlipayMerchantNo: {
                required: "请输入商户号",
                maxlength: "长度不能超过100位"
            },
            AlipayKey: {
                required: "请输入秘钥"
            },
            PrivatePath: {
                required: "请选择要导入的文件"
            },
            PublicPath: {
                required: "请选择要导入的文件"
            }
        }
    });
});


