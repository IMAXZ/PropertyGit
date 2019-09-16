function roleValidate(url) {

    $("#form").validate({
        rules: {
            RoleName: {
                required: true,
                maxlength: 50,
                remote: {
                    type: "POST",
                    url: url,             //角色名称是否存在
                    data: {
                        roleId: function () { return $("#RoleId").val(); },
                        roleName: function () { return $("#RoleName").val(); }
                    }
                }
            },
            RoleMemo: {
                maxlength: 200
            }
        },
        messages: {
            RoleName: {
                required: "请输入角色名称",
                maxlength: "长度不能超过50位",
                remote: "该角色名称已经存在"
            },
            RoleMemo: {
                maxlength: "长度不能超过200位"
            }
        }
    });
}
