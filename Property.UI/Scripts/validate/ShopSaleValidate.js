$(document).ready(function () {
    $("#form").validate({
        errorPlacement: function (error, element) {
            if (element.parent().hasClass("input-group")) {
                element.parent().after(error);
            }
            else
                element.after(error)
        },
        rules: {
            GoodsCategoryId: {
                required: true
            },
            Title: {
                required: true,
                maxlength: 200
            },
            Content: {
                required: true
            },
            Price: {
                required: true,
                isFloatGtZero: true,
                isOneOrTwoDecimal: true,
                max: 999999999.99
            },
            RemainingAmout: {
                required: true,
                digits: true,
                min: 1,
                max: 999999999
            },
            Phone: {
                TelCheck: true
            }
        }, messages: {
            GoodsCategoryId: {
                required: '请选择商品分类'
            },
            Title: {
                required: '请输入商品名称',
                maxlength: '长度不能超过200位',
            },
            Content: {
                required: '请输入详细描述'
            },
            Price: {
                required: '请输入商品价格',
                isFloatGtZero: '必须输入大于0的数字',
                isOneOrTwoDecimal: '小数点后最多保留两位',
                max: '最大不能超过999999999.99'
            },
            RemainingAmout: {
                required: '请输入商品库存',
                digits: '必须输入大于0的整数',
                min: '必须输入大于0的整数',
                max: '最大不能超过999999999'
            },
        }
    });
})