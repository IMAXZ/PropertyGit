// 判断浮点数value是否大于0
jQuery.validator.addMethod("isFloatGtZero", function (value, element) {
    value = parseFloat(value);
    return this.optional(element) || value > 0;
}, "浮点数必须大于0");

//验证联系电话
jQuery.validator.addMethod("TelCheck", function (value, element) {
    var length = value.length;
    var tel1 = /^\d{3,4}-\d{7,9}$/;
    var tel2 = /^\d{7,9}$/;
    var mobile = /^1[34578]\d{9}$/;
    return this.optional(element) || tel1.test(value) || tel2.test(value) || mobile.test(value);
}, "请正确填写您的联系电话");

//验证身份证号
jQuery.validator.addMethod("CardCheck", function (value, element) {
    return this.optional(element) || isIdCardNo(value);
}, "请正确输入身份证号码");


//验证手机号
jQuery.validator.addMethod("PhoneCheck", function (value, element) {
    var length = value.length;
    var mobile = /^1[34578]\d{9}$/;
    return this.optional(element) || mobile.test(value);
}, "请正确填写您的手机号");

//字母或数字
jQuery.validator.addMethod("LetterOrNumber", function (value, element) {
    var score = /^[A-Za-z0-9]+$/;
    return this.optional(element) || score.test(value);
}, "请输入大小写字母、数字");


//请输入至少一个字母和数字
jQuery.validator.addMethod("LetterAndNumber", function (value, element) {
    var score = /^(?![^a-zA-Z]+$)(?!\D+$).{2,}$/;
    return this.optional(element) || score.test(value);
}, "密码必须包含字母和数字");


//判断浮点数value做多输入两位小数
jQuery.validator.addMethod("isOneOrTwoDecimal", function (value, element) {
    //value = parseFloat(value);
    var source =/^[0-9]+(.[0-9]{1,2})?$/;
    return this.optional(element) || source.test(value);
}, "小数点后最多输入两位小数");

//验证身份证支持类方法
function isIdCardNo(num) {
    var factorArr = new Array(7, 9, 10, 5, 8, 4, 2, 1, 6, 3, 7, 9, 10, 5, 8, 4, 2, 1);
    var parityBit = new Array("1", "0", "X", "9", "8", "7", "6", "5", "4", "3", "2");
    var varArray = new Array();
    var intValue;
    var lngProduct = 0;
    var intCheckDigit;
    var intStrLen = num.length;
    var idNumber = num;
    // initialize
    if ((intStrLen != 15) && (intStrLen != 18)) {
        return false;
    }
    // check and set value
    for (i = 0; i < intStrLen; i++) {
        varArray[i] = idNumber.charAt(i);
        if ((varArray[i] < '0' || varArray[i] > '9') && (i != 17)) {
            return false;
        } else if (i < 17) {
            varArray[i] = varArray[i] * factorArr[i];
        }
    }
    if (intStrLen == 18) {
        //check date
        var date8 = idNumber.substring(6, 14);
        if (isDate8(date8) == false) {
            return false;
        }
        // calculate the sum of the products
        for (i = 0; i < 17; i++) {
            lngProduct = lngProduct + varArray[i];
        }
        // calculate the check digit
        intCheckDigit = parityBit[lngProduct % 11];
        // check last digit
        if (varArray[17] != intCheckDigit) {
            return false;
        }
    }
    else {        //length is 15
        //check date
        var date6 = idNumber.substring(6, 12);
        if (isDate6(date6) == false) {
            return false;
        }
    }
    return true;
}

function isDate6(sDate) {
    if (!/^[0-9]{6}$/.test(sDate)) {
        return false;
    }
    var year, month, day;
    year = sDate.substring(0, 4);
    month = sDate.substring(4, 6);
    if (year < 1700 || year > 2500) return false
    if (month < 1 || month > 12) return false
    return true
}

function isDate8(sDate) {
    if (!/^[0-9]{8}$/.test(sDate)) {
        return false;
    }
    var year, month, day;
    year = sDate.substring(0, 4);
    month = sDate.substring(4, 6);
    day = sDate.substring(6, 8);
    var iaMonthDays = [31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31]
    if (year < 1700 || year > 2500) return false
    if (((year % 4 == 0) && (year % 100 != 0)) || (year % 400 == 0)) iaMonthDays[1] = 29;
    if (month < 1 || month > 12) return false
    if (day < 1 || day > iaMonthDays[month - 1]) return false
    return true
}

