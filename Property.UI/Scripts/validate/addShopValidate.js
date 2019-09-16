$(document).ready(function () {
    $("#form").validate({
        rules: {
            ShopName: {
                required: true,
                maxlength: 50
            },
            Type: {
                required: true
            },
            ProvinceId: {
                required: true
            },
            CityId: {
                required: true
            },
            Tel: {
                maxlength: 20,
                TelCheck: true
            },
            Address: {
                maxlength: 200
            },
            MainSale: {
                required: true,
                maxlength: 300
            }
        },
        messages: {
            ShopName: {
                required: "请输入门店名称",
                maxlength: "长度不能超过50位"
            },
            Type: {
                required: "请选择门店类型"
            },
            ProvinceId: {
                required: "请选择所属省"
            },
            CityId: {
                required: "请选择所属市"
            },
            Tel: {
                maxlength: "长度不能超过20位"
            },
            Address: {
                maxlength: "长度不能超过200位"
            },
            MainSale: {
                required: "请输入主营介绍",
                maxlength: "长度不能超过300位"
            }
        }
    });
});
