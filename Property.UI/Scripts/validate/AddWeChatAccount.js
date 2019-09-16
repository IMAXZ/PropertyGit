$(document).ready(function ()
{
    $("#form").validate({
        rules: {
            WeChatNumber: {
                required: true,
                maxlength: 100
            },
            WeChatMerchantNo: {
                required: true,
                maxlength: 100
            },
            WeChatKey: {
                required: true
            }
        },
        messages: {
            WeChatNumber: {
                required: "请输入微信APP ID",
                maxlength: "长度不能超过100位"
            },
            WeChatMerchantNo: {
                required: "请输入商户号",
                maxlength: "长度不能超过100位"
            },
            WeChatKey: {
                required: "请输入秘钥"
            }
        }
    });
});