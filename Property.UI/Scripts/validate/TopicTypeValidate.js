$(document).ready(function () {

    $("#form").validate({
        rules: {
            CategoryName: {
                required: true,
                maxlength: 20,
                remote: {
                    type: "POST",
                    url: "/TopicType/CategoryCheckExist",
                    data: {
                        categoryId: function () { return $("#CategoryId").val(); },
                        name: function () { return $("#CategoryName").val(); }
                    }
                }
            }
        },
        messages: {
            CategoryName: {
                required: "请输入沟通类别名称",
                maxlength: "长度不能超过20位",
                remote: "该类别名称已存在"
            }
        }
    });
});