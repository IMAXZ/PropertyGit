$(function () {
    $("#side-menu li[id]").click(function () {
        var alink = $(this).find("a:first");
        var menuID = $(this).attr("id");
        if (alink != undefined) {
            var href = alink.attr("href");
            if (href != "#") {
                if (menuID.indexOf("L1") != -1) {
                    $.session.set('L1ActiveMenuID', menuID);
                    $.session.remove('L2ActiveMenuID');
                    $.session.remove('L3ActiveMenuID');
                }
                if (menuID.indexOf("L2") != -1) {
                    var l1menuID = $(this).parent().parent().attr("id");
                    $.session.set('L1ActiveMenuID', l1menuID)
                    $.session.set('L2ActiveMenuID', menuID)
                    $.session.remove('L3ActiveMenuID');
                }
                if (menuID.indexOf("L3") != -1) {
                    var l1menuID = $(this).parent().parent().parent().parent().attr("id");
                    $.session.set('L1ActiveMenuID', l1menuID)
                    var l2menuID = $(this).parent().parent().attr("id");
                    $.session.set('L2ActiveMenuID', l2menuID)
                    $.session.set('L3ActiveMenuID', menuID)
                }
            }
        }
    })

    var L1menuID = $.session.get('L1ActiveMenuID');
    if (L1menuID != undefined) {
        $("#" + L1menuID).attr("class", "active");
    }
    var L2menuID = $.session.get('L2ActiveMenuID');
    if (L2menuID != undefined) {
        var li = $("#" + L2menuID);
        li.parent().addClass("in")
        li.attr("class", "active");
    }
    var L3menuID = $.session.get('L3ActiveMenuID');
    if (L3menuID != undefined) {
        var li = $("#" + L3menuID);
        li.parent().addClass("in")
        li.attr("class", "active");
    }
})

$(document).keydown(function (e) {
    var target = e.target;
    var tag = e.target.tagName.toUpperCase();
    if (e.keyCode == 8) {
        if ((tag == 'INPUT' && !$(target).attr("readonly")) || (tag == 'TEXTAREA' && !$(target).attr("readonly"))) {
            if ((target.type.toUpperCase() == "RADIO") || (target.type.toUpperCase() == "CHECKBOX")) {
                return false;
            } else {
                return true;
            }
        } else {
            return false;
        }
    }
});