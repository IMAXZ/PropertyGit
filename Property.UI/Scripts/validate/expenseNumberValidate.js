function NumberValidate() {
    $("#form").validate({
        rules: {
            ExpenseNumber: {
                required: true,
                maxlength: 50,
                remote: {
                    type: "POST",
                    url: "/PaymentNo/RemoteCheckNumberExist",
                    data: {
                        Id: function () { return $("#Id").val(); },
                        Number: function () { return $("#ExpenseNumber").val(); },
                    }
                }
            },
            ExpenseTypeId: {
                required: true
            },
            DoorId: {
                NotNull:true
            },
            BuildCompanyId: {
                NotNull: true
            },
            Desc: {
                maxlength: 500
            }
        },
        messages: {
            ExpenseNumber: {
                required: "请输入缴费编号",
                maxlength: "长度不能超过50位",
                remote: "该编号已存在"
            },
            ExpenseTypeId: {
                required: "请选择缴费类型"
            },
            BuildCompanyId: {
                NotNull:"请选择所属办公楼单位"
            },
            Desc: {
                maxlength: "长度不能超过500位"
            }
        }
    });
}

jQuery.validator.addMethod("NotNull", function (value, element) {
    var flag = true;
    if ($("#PlaceType").val() == 0) {
        if ($("#DoorId").val() == "") {
            flag = false;
        }
    } else if ($("#PlaceType").val() == 1) {
        if ($("#BuildCompanyId").val() == "") {
            flag = false;
        }
    }
    return flag;
}, "请选择所属单元户");
