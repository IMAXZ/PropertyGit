$(document).ready(function () {

    $("#form").validate({
        rules: {
            CategoryName: {
                required: true,
                maxlength: 200,
                remote: {
                    type: "POST",
                    url: "/Inspection/ContentCheckExist",
                    data: {
                        CategoryId: function () { return $("#CategoryId").val(); },
                        CategoryName: function () { return $("#CategoryName").val(); }
                    }
                }
            },
            Memo: {
                maxlength: 1000
            }
        },
        messages: {
            CategoryName: {
                required: "请输入巡检类别名称",
                maxlength: "长度不能超过200位",
                remote: "该巡检类别名称已经存在"
            },
            Memo: {
                maxlength: "长度不能超过1000位"
            }
        }
    });
});