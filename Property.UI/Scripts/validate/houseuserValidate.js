$(document).ready(function ()
{
    $("#form").validate({
        rules: {
            Name: {
                required: true,
                maxlength: 30
            },
            Phone: {
                required: true,
                PhoneCheck: true,
                maxlength: 15
            },
            BuildId: {
                required: true
            },
            UnitId: {
                required: true
            },
            DoorId: {
                required: true,
                remote: {
                    type: "POST",
                    url: "/HouseUser/RemoteCheckExist",
                    data: {
                        doorId: function () { return $("#DoorId").val(); },
                        id: function () { return $("#Id").val(); },
                    }
                }
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
                //required: true,
                maxlength: 500
            }
        },
        messages: {
            Name: {
                required: "请输入业主姓名",
                maxlength: "长度不能超过30位",
            },
            Phone: {
                required: "请输入手机",
                maxlength: "长度不能超过15位",
                PhoneCheck: "请正确填写手机"
            },
            BuildId: {
                required: "请选择楼座",
            },
            UnitId: {
                required: "请选择单元",
            },
            DoorId: {
                required: "请选择单元户",
                remote: "该单元户业主已存在"

            },
            Desc: {
                //required: "请输入业主备注",
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
