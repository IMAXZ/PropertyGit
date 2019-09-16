$(document).ready(function () {

    $("#form").validate({
        rules: {
            PointName: {
                required: true,
                maxlength: 200,
                //remote: {
                //    type: "POST",
                //    url: "/Inspection/PointCheckExist",
                //    data: {
                //        PointId: function () { return $("#PointId").val(); },
                //        PointName: function () { return $("#PointName").val(); },
                //        CategoryId: function () {return $("#CategoryId").val(); },
                //    }
                //}
            },
            CategoryId:{
                required: true
            },
            Memo: {
                maxlength: 1000
            }
        },
        messages: {
            PointName: {
                required: "请输入巡检点名称",
                maxlength: "长度不能超过200位",
                //remote: "该巡检点名称已存在"
            },
            CategoryId: {
                required: "请选择巡检类别"
            },
            Memo: {
                maxlength: "长度不能超过1000位"
            }
        }
    });
});