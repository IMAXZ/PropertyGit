$(document).ready(function () {

    $("#form").validate({
        errorPlacement : function(error, element) { 
            if (element.parent().parent().parent().hasClass("i-checks")) {
                element.parent().parent().parent().before(error);
            } else if (element.parent().parent().hasClass("col-md-5")) {
                element.parent().before(error);
            } else if (element.parent().hasClass("input-group date")) {
                element.parent().before(error);
            }else
                element.before(error)
        },
        rules: {
            PlanName: {
                required: true,
                maxlength: 200,
                remote: {
                    type: "POST",
                    url: "/Inspection/PlanCheckExist",
                    data: {
                        PlanId: function () { return $("#PlanId").val(); },
                        PlanName: function () { return $("#PlanName").val(); }
                    }
                }
            },
            BeginDate: {
                required: true,
                StartDateCheck: true
            },
            EndDate: {
                required: true,
                EndDateCheck: true
            },
            RandomNum: {
                NotNull: true,
                digits: true,
                min: 1,
                DayNumCheck: true,
                WeekNumCheck: true,
                MonthNumCheck: true
            },
            EndHourNums: {
                DayPlanCheck: true
            },
            StartWeekNums: {
                WeekPlanCheck: true
            },
            StartDayNums: {
                MonthPlanCheck: true
            },
            PointIds: {
                required: true
            },
            Memo: {
                maxlength: 1000
            }
        },
        messages: {
            PlanName: {
                required: "请输入巡检任务名称",
                maxlength: "长度不能超过200位",
                remote: "该巡检任务名称已经存在"
            },
            BeginDate: {
                required: "巡检开始时间不能为空"
            },
            EndDate: {
                required: "巡检结束时间不能为空"
            },
            RandomNum: {
                digits: "必须为大于0的整数",
                min: "必须为大于0的整数"
            },
            PointIds: {
                required: "请选择巡检点"
            },
            Memo: {
                maxlength: "长度不能超过1000位"
            }
        }
    });
});
jQuery.validator.addMethod("StartDateCheck", function (value, element) {

    var flag = true;

    var beginArr = value.split("-");
    var beginDate = new Date(beginArr[0], beginArr[1] - 1, beginArr[2]);
    var WeekDay = beginDate.getDay();
    //周巡检
    if ($("#Type").val() == 1 && WeekDay != 1) {
        flag = false;
    //月巡检
    } else if ($("#Type").val() == 2 && beginDate.getDate() != 1) {
        flag = false;
    }
    return flag;
}, "巡检开始时间设置不合理");

jQuery.validator.addMethod("EndDateCheck", function (value, element) {

    var flag = true;

    var beginArr = $("#BeginDate").val().split("-");
    var beginDate = new Date(beginArr[0], beginArr[1] - 1, beginArr[2]);
    var endArr = $("#EndDate").val().split("-");
    var endDate = new Date(endArr[0], endArr[1] - 1, endArr[2]);

    var day = new Date(endDate.getFullYear(), endDate.getMonth() + 1, 0);
    //周巡检
    if ($("#Type").val() == 1 && endDate.getDay() != 0) {
        flag = false;
        //月巡检
    } else if ($("#Type").val() == 2 && endDate.getDate() != day.getDate()) {
        flag = false;
    }
    if (endDate.getTime() < beginDate.getTime())
    {
        flag = false;
    }
    return flag;
}, "巡检结束时间设置不合理");

jQuery.validator.addMethod("NotNull", function (value, element) {
    var flag = true;
    if ($("#IsRandom").val() == 1) {
        if ($("#RandomNum").val() == "") {
            flag = false;
        }
    }
    return flag;
}, "请输入随机巡检次数");

jQuery.validator.addMethod("DayNumCheck", function (value, element) {
    var flag = true;
    if ($("#IsRandom").val() == 1) {
        if ($("#Type").val() == 0 && parseInt($("#RandomNum").val()) > 10) {
            flag = false;
        }
    }
    return flag;
}, "日巡检次数不能多于10次");

jQuery.validator.addMethod("WeekNumCheck", function (value, element) {
    var flag = true;
    if ($("#IsRandom").val() == 1) {
        if ($("#Type").val() == 1 && parseInt($("#RandomNum").val()) > 7) {
            flag = false;
        }
    }
    return flag;
}, "周巡检次数不能多于7次");

jQuery.validator.addMethod("MonthNumCheck", function (value, element) {
    var flag = true;
    if ($("#IsRandom").val() == 1) {
        if ($("#Type").val() == 2 && parseInt($("#RandomNum").val()) > 10) {
            flag = false;
        }
    }
    return flag;
}, "月巡检次数不能多于10次");

jQuery.validator.addMethod("DayPlanCheck", function (value, element) {

    if ($("#IsRandom").val() == 0 && $("#Type").val() == 0) {

        var num = $("#DayPlans").children("div:visible").length;

        for (var i = 1; i <= num; i++) {

            var LastStartHourNum = $("#DayPlan" + i).find("#StartHourNums").val();
            var LastEndHourNum = $("#DayPlan" + i).find("#EndHourNums").val();
            var BeforeEndHourNum;
            if (i > 1) {
                BeforeEndHourNum = $("#DayPlan" + (i - 1)).find("#EndHourNums").val();
            }
            if (LastStartHourNum == "" || LastEndHourNum == "") {
                return false;
            }
                //如果结束时间小于开始时间或第二次开始时间小于第一次结束时间
            else if (parseInt(LastStartHourNum) >= parseInt(LastEndHourNum)
                || (i > 1 && parseInt(LastStartHourNum) < parseInt(BeforeEndHourNum))) {
                return false;
            }
        }
    }
    return true;
}, "&nbsp;&nbsp;巡检时间安排存在问题,请先合理安排");

jQuery.validator.addMethod("WeekPlanCheck", function (value, element) {

    if ($("#IsRandom").val() == 0 && $("#Type").val() == 1) {

        var num = $("#WeekPlans").children("div:visible").length;
        for (var i = 1; i <= num; i++) {
            var LastStartWeekNum = $("#WeekPlan" + i).find("#StartWeekNums").val();
            var BeforeWeekNum;
            if (i > 1) {
                BeforeWeekNum = $("#WeekPlan" + (i - 1)).find("#StartWeekNums").val();
            }
            if (LastStartWeekNum == "" || (i > 1 && parseInt(LastStartWeekNum) <= parseInt(BeforeWeekNum))) {
                return false;
            }
        }
    }
    return true;
}, "&nbsp;&nbsp;巡检时间安排存在问题,请先合理安排");

jQuery.validator.addMethod("MonthPlanCheck", function (value, element) {

    if ($("#IsRandom").val() == 0 && $("#Type").val() == 2) {
        var num = $("#MonthPlans").children("div:visible").length;
        for (var i = 1; i <= num; i++) {
            var LastStartDayNum = $("#MonthPlan" + i).find("#StartDayNums").val();
            var BeforeDayNum;
            if (i > 1) {
                BeforeDayNum = $("#MonthPlan" + (i - 1)).find("#StartDayNums").val();
            }
            if (LastStartDayNum == "" || (i > 1 && parseInt(LastStartDayNum) <= parseInt(BeforeDayNum))) {
                return false;
            }
        }
    }
    return true;
}, "&nbsp;&nbsp;巡检时间安排存在问题,请先合理安排");