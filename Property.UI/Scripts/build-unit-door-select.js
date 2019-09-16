function buildUnitChange(controller) {
    //楼座下拉框发生变化
    $("#BuildId").change(function () {
        var buildId = $("#BuildId").val();
        var reqUrl = "";
        if (buildId != "" && buildId != undefined) {
            reqUrl = "/" + controller + "/GetUnitList?buildId=" + buildId;
        }
        else {
            $('#UnitId').empty();
            $('#UnitId').append("<option value=\"\">请选择单元</option>");
            $('#DoorId').empty();
            $('#DoorId').append("<option value=\"\">请选择单元户</option>");
            return;
        }
        $.ajax({
            type: "post",
            url: reqUrl,
            contentType: "application/json;charset=utf-8",
            dataType: "json",
            success: function (data) {
                $('#UnitId').empty();
                $('#UnitId').append("<option value=\"\">请选择单元</option>");
                $.each(data, function (index, entry) {
                    $('#UnitId').append("<option value=\"" + entry["Value"] + "\">" + entry["Text"] + "</option>");
                });
                $('#UnitId').change();
            }, error: function (error) {
                swal({
                    title: "选择楼座，单元列表加载失败",
                    type: "error",
                    text: ""
                });
            }
        });
    });

    //单元下拉框发生变化
    $("#UnitId").change(function () {

        var unitId = $("#UnitId").val();
        var reqUrl = "";
        if (unitId != "" && unitId != undefined) {
            reqUrl = "/" + controller + "/GetDoorList?unitId=" + unitId;
        } else {
            $('#DoorId').empty();
            $('#DoorId').append("<option value=\"\">请选择单元户</option>");
            return;
        }

        $.ajax({
            type: "post",
            url: reqUrl,
            contentType: "application/json;charset=utf-8",
            dataType: "json",
            success: function (data) {
                $('#DoorId').empty();
                $('#DoorId').append("<option value=\"\">请选择单元户</option>");
                $.each(data, function (index, entry) {
                    $('#DoorId').append("<option value=\"" + entry["Value"] + "\">" + entry["Text"] + "</option>");
                });
                $('#DoorId').change();
            }, error: function (error) {
                swal({
                    title: "选择单元，户列表加载失败",
                    type: "error",
                    text: ""
                });
            }
        });
    });
}