
function placeValidate(url) {
    $("#form").validate({
        rules: {
            PlaceName: {
                required: true,
                maxlength: 50,
                remote: {
                    type: "POST",
                    url: url,
                    data: {
                        PlaceId: function () { return $("#PlaceId").val(); },
                        PlaceName: function () { return $("#PlaceName").val(); },
                    }
                }
            },
            CompanyId: {
                required: true
            },
            ProvinceId: {
                required: true
            },
            CityId: {
                required: true
            },
            Address: {
                maxlength: 200
            },
            Tel: {
                maxlength: 30,
                TelCheck: true
            },
            Longitude: {
                number: true
            },
            Latitude: {
                number: true
            }
        },
        messages: {
            PlaceName: {
                required: "请输入物业小区名称",
                maxlength: "长度不能超过50位",
                remote: "物业小区已存在"
            },
            CompanyId: {
                required: "请选择所属物业公司"
            },
            ProvinceId: {
                required: "请选择省"
            },
            CityId: {
                required: "请选择城市"
            },
            Address: {
                maxlength: "长度不能超过200位"
            },
            Tel: {
                maxlength: "长度不能超过30位",
                TelCheck: "请正确填写联系电话"
            },
            Longitude: {
                number: "请输入数字"
            },
            Latitude: {
                number: "请输入数字"
            },
        }
    });
}
