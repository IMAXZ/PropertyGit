$(document).ready(function () {

    $('.i-checks').iCheck({
        radioClass: 'iradio_square-green',
    });

    $("#form").validate({
        rules: {
            Name: {
                required: true,
                maxlength: 100,
                remote: {
                    type: "POST",
                    url: "/PropertyPayment/RemoteCheckExist",
                    data: {
                        Name: function () { return $("#Name").val() },
                        Id: function () { return $("#ExpenseTypeId").val(); }
                    }
                }
            },
            Memo: {
                maxlength: 500
            },
        },
        messages: {
            Name: {
                required: "请输入类别名称",
                maxlength: "长度不能超过100位",
                remote: "该类别名称已存在"
            },
            Memo: {
                maxlength: "长度不能超过500位"
            },
        },
    });
});