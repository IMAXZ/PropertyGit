$(document).ready(function () {

    //省份下拉框发生变化
    $("#ProvinceId").change(function () {

        var provinceId = $("#ProvinceId").val();
        var reqUrl = "";
        if (provinceId != "" && provinceId != undefined) {

            reqUrl = "/Common/GetCityList?provinceId=" + provinceId;
        }
        else {
            $('#CityId').empty();
            $('#CityId').append("<option value=\"\">请选择城市</option>");
            $('#CountyId').empty();
            $('#CountyId').append("<option value=\"\">请选择区县</option>");
            return;
        }
        $.ajax({
            type: "post",
            url: reqUrl,
            contentType: "application/json;charset=utf-8",
            dataType: "json",
            success: function (data) {
                $('#CityId').empty();
                $('#CityId').append("<option value=\"\">请选择城市</option>");
                $.each(data, function (index, entry) {
                    $('#CityId').append("<option value=\"" + entry["Value"] + "\">" + entry["Text"] + "</option>");
                });
                $('#CityId').change();
            }, error: function (error) {
                swal({
                    title: "选择省城市列表加载失败",
                    type: "error",
                    text: ""
                });
            }
        });
    });

    //城市下拉框发生变化
    $("#CityId").change(function () {

        var cityId = $("#CityId").val();
        var reqUrl = "";
        if (cityId != "" && cityId != undefined) {
            reqUrl = "/Common/GetCountyList?cityId=" + cityId;
        } else {
            $('#CountyId').empty();
            $('#CountyId').append("<option value=\"\">请选择区县</option>");
            return;
        }

        $.ajax({
            type: "post",
            url: reqUrl,
            contentType: "application/json;charset=utf-8",
            dataType: "json",
            success: function (data) {
                $('#CountyId').empty();
                $('#CountyId').append("<option value=\"\">请选择区县</option>");
                $.each(data, function (index, entry) {
                    $('#CountyId').append("<option value=\"" + entry["Value"] + "\">" + entry["Text"] + "</option>");
                });
                $('#CountyId').change();
            }, error: function (error) {
                swal({
                    title: "选择城市县区列表加载失败",
                    type: "error",
                    text: ""
                });
            }
        });
    })
});