

app.service('UtilsService', ['$q', 'LogEntryTypeEnum', 'LabelColorsEnum','PeriodEnum', function ($q, LogEntryTypeEnum, LabelColorsEnum, PeriodEnum) {

    "use strict";

    var dateFormat = function () {
        var token = /d{1,4}|m{1,4}|yy(?:yy)?|([HhMsTt])\1?|[LloSZ]|"[^"]*"|'[^']*'/g,
            timezone = /\b(?:[PMCEA][SDP]T|(?:Pacific|Mountain|Central|Eastern|Atlantic) (?:Standard|Daylight|Prevailing) Time|(?:GMT|UTC)(?:[-+]\d{4})?)\b/g,
            timezoneClip = /[^-+\dA-Z]/g,
            pad = function (val, len) {
                val = String(val);
                len = len || 2;
                while (val.length < len) val = "0" + val;
                return val;
            };

        // Regexes and supporting functions are cached through closure
        return function (date, mask, utc) {
            var dF = dateFormat;

            // You can't provide utc if you skip other args (use the "UTC:" mask prefix)
            if (arguments.length == 1 && Object.prototype.toString.call(date) == "[object String]" && !/\d/.test(date)) {
                mask = date;
                date = undefined;
            }

            // Passing date through Date applies Date.parse, if necessary
            date = date ? new Date(date) : new Date;
            if (isNaN(date)) throw SyntaxError("invalid date");

            mask = String(dF.masks[mask] || mask || dF.masks["default"]);

            // Allow setting the utc argument via the mask
            if (mask.slice(0, 4) == "UTC:") {
                mask = mask.slice(4);
                utc = true;
            }

            var _ = utc ? "getUTC" : "get",
                d = date[_ + "Date"](),
                D = date[_ + "Day"](),
                m = date[_ + "Month"](),
                y = date[_ + "FullYear"](),
                H = date[_ + "Hours"](),
                M = date[_ + "Minutes"](),
                s = date[_ + "Seconds"](),
                L = date[_ + "Milliseconds"](),
                o = utc ? 0 : date.getTimezoneOffset(),
                flags = {
                    d: d,
                    dd: pad(d),
                    ddd: dF.i18n.dayNames[D],
                    dddd: dF.i18n.dayNames[D + 7],
                    m: m + 1,
                    mm: pad(m + 1),
                    mmm: dF.i18n.monthNames[m],
                    mmmm: dF.i18n.monthNames[m + 12],
                    yy: String(y).slice(2),
                    yyyy: y,
                    h: H % 12 || 12,
                    hh: pad(H % 12 || 12),
                    H: H,
                    HH: pad(H),
                    M: M,
                    MM: pad(M),
                    s: s,
                    ss: pad(s),
                    l: pad(L, 3),
                    L: pad(L > 99 ? Math.round(L / 10) : L),
                    t: H < 12 ? "a" : "p",
                    tt: H < 12 ? "am" : "pm",
                    T: H < 12 ? "A" : "P",
                    TT: H < 12 ? "AM" : "PM",
                    Z: utc ? "UTC" : (String(date).match(timezone) || [""]).pop().replace(timezoneClip, ""),
                    o: (o > 0 ? "-" : "+") + pad(Math.floor(Math.abs(o) / 60) * 100 + Math.abs(o) % 60, 4),
                    S: ["th", "st", "nd", "rd"][d % 10 > 3 ? 0 : (d % 100 - d % 10 != 10) * d % 10]
                };

            return mask.replace(token, function ($0) {
                return $0 in flags ? flags[$0] : $0.slice(1, $0.length - 1);
            });
        };
    }();

    // Some common format strings
    dateFormat.masks = {
        "default": "ddd mmm dd yyyy HH:MM:ss",
        shortDate: "m/d/yy",
        mediumDate: "mmm d, yyyy",
        longDate: "mmmm d, yyyy",
        fullDate: "dddd, mmmm d, yyyy",
        shortTime: "h:MM TT",
        mediumTime: "h:MM:ss TT",
        longTime: "h:MM:ss TT Z",
        isoDate: "yyyy-mm-dd",
        isoTime: "HH:MM:ss",
        isoDateTime: "yyyy-mm-dd'T'HH:MM:ss",
        isoUtcDateTime: "UTC:yyyy-mm-dd'T'HH:MM:ss'Z'"
    };

    // Internationalization strings
    dateFormat.i18n = {
        dayNames: [
            "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat",
            "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"
        ],
        monthNames: [
            "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec",
            "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"
        ]
    };

    

    function getEnum(enumObj,propertyFilter, valueFilter) {
        for (var item in enumObj) {
            if (enumObj.hasOwnProperty(item)) {

                var enumItem = enumObj[item];

                if (enumItem[propertyFilter] === valueFilter)
                    return enumItem;
            }
        }
        return undefined;
    }

    function getArrayEnum(enumObj) {
        var array = [];

        for (var item in enumObj) {
            if (enumObj.hasOwnProperty(item)) {
                array.push(enumObj[item]);
            }
        }
        return array;
    }

    function getLogEntryType() {
        return getArrayEnum(LogEntryTypeEnum);
    }

    function getLogEntryTypeDescription(logEntryType) {
        var enumObj = getEnum(LogEntryTypeEnum, 'value', logEntryType);
        if (enumObj) return enumObj.description;
        return undefined;
    }

    function getLogEntryTypeColor(logEntryType) {

        if (logEntryType === LogEntryTypeEnum.Information.value) return LabelColorsEnum.Info.Color;
        if (logEntryType === LogEntryTypeEnum.Warning.value) return LabelColorsEnum.Warning.Color;
        if (logEntryType === LogEntryTypeEnum.Error.value) return LabelColorsEnum.Error.Color;
        if (logEntryType === LogEntryTypeEnum.Verbose.value) return LabelColorsEnum.Primary.Color;

        return LabelColorsEnum.Info.Color;
    };

    function replaceAll(string, find, replace) {
        return string.replace(new RegExp(escapeRegExp(find), 'g'), replace);
    }

    function escapeRegExp(string) {
        return string.replace(/([.*+?^=!:${}()|\[\]\/\\])/g, "\\$1");
    }

    function waitMultipleAsyncOperations(operations) {
        var deferred = $q.defer();
        var pendingOperations = operations.length;
        var isRejected = false;
        if (pendingOperations == 0)
            deferred.resolve();
        angular.forEach(operations, function (operation) {
            var promise = operation();//the operation is expected to return a promise

            if(promise != undefined)
            {
                promise.then(function () {
                    if (isRejected)
                        return;
                    pendingOperations--;

                    if (pendingOperations == 0)
                        deferred.resolve();
                }).catch(function (error) {
                    deferred.reject(error);
                    isRejected = true;
                });
            }
            else {
                pendingOperations--;

                if (pendingOperations == 0)
                    deferred.resolve();
            }                
        });
        return deferred.promise;
    }

    function getItemIndexByVal(array, value, attname) {
        for (var i = 0; i < array.length; i++) {
            if (array[i][attname] == value) {
                return i
            }
        }
        return -1;
    }

    function getItemByVal(array, value, attname) {
        for (var i = 0; i < array.length; i++) {
            if (array[i][attname] == value) {
                return array[i];
            }
        }
        return null;
    }

    function contains(array, obj) {
        for (var i = 0; i < array.length; i++) {
            if (array[i] === obj) {
                return true;
            }
        }
        return false;
    }

    function getPropValuesFromArray(array, propName) {
        var propValues;
        if (array.length > 0) {
            propValues = [];
            angular.forEach(array, function (val) {
                propValues.push(val[propName]);
            });
        }
        return propValues;
    }

    function getPropMaxValueFromArray(array, propName) {
        var max = undefined;
        
        if (array.length > 0) {

            for (var i = 0, len = array.length; i < len; i++) {
                if (max === undefined)
                    max = array[i][propName];
                if (array[i][propName] > max)
                    max = array[i][propName];
            }
        }
        
        return max;
    }

    function getPropMinValueFromArray(array, propName) {
        var min = undefined;

        if (array.length > 0) {

            for (var i = 0, len = array.length; i < len; i++) {
                if (min === undefined)
                    min = array[i][propName];
                if (array[i][propName] < min)
                    min = array[i][propName];
            }
        }

        return min;
    }



    function downloadFile(data, headers) {
    
        //data = new ArrayBuffer(data.length);
        //console.log(data);
        //return;
        var octetStreamMime = 'application/octet-stream';
        var success = false;
        headers = headers();
        var matcher = /filename[^;=\n]*=((['"]).*?\2|[^;\n]*)/i;
        var results = headers['content-disposition'].match(matcher);
        var filename = results[1];
        var contentType = headers['content-type'] || octetStreamMime;
        try {
            var blob = new Blob([data], { type: contentType });
            if (navigator.msSaveBlob)
                navigator.msSaveBlob(blob, filename);
            else {
                var saveBlob = navigator.webkitSaveBlob || navigator.mozSaveBlob || navigator.saveBlob;
                if (saveBlob === undefined) throw "Not supported";
                saveBlob(blob, filename);
            }
            success = true;
        } catch (ex) {
            console.log(ex);
        }

        if (!success) {
            var urlCreator = window.URL || window.webkitURL || window.mozURL || window.msURL;
            if (urlCreator) {
                var link = document.createElement('a');
                if ('download' in link) {
                    try {
                        var blob = new Blob([data], { type: contentType });
                        var url = urlCreator.createObjectURL(blob);
                        console.log(url);
                        link.setAttribute('href', url);
                        link.setAttribute("download", filename);
                        var event = document.createEvent('MouseEvents');
                        event.initMouseEvent('click', true, true, window, 1, 0, 0, 0, 0, false, false, false, false, 0, null);
                        link.dispatchEvent(event);
                        success = true;
                    } catch (ex) {
                        console.log("Download link method with simulated click failed with the following exception:");
                        console.log(ex);
                    }
                }

                if (!success) {
                    try {
                        var blob = new Blob([data], { type: octetStreamMime });
                        var url = urlCreator.createObjectURL(blob);
                        window.location = url;
                        success = true;
                    } catch (ex) {
                        console.log("Download link method with window.location failed with the following exception:");
                        console.log(ex);
                    }
                }

            }
        }

        if (!success) {
            window.open(httpPath, '_blank', '');
        }
    }

   

   

    function dateToServerFormat(date) {
        if (date instanceof Date)
            return dateFormat(date, dateFormat.masks.isoDateTime);
        else
            return date;
    }


    function getPeriod(periodType) {
        switch (periodType) {
            case PeriodEnum.LastYear.value: return getLastYearInterval();
            case PeriodEnum.LastMonth.value: return getLastMonthInterval();
            case PeriodEnum.LastWeek.value: return getLastWeekInterval();
            case PeriodEnum.Yesterday.value: return getYesterdayInterval();
            case PeriodEnum.Today.value: return getTodayInterval();
            case PeriodEnum.CurrentWeek.value: return getCurrentWeekInterval();
            case PeriodEnum.CurrentMonth.value: return getCurrentMonthInterval();
            case PeriodEnum.CurrentYear.value: return getCurrentYearInterval();
        }
    }
    function getCurrentYearInterval() {
        var date = new Date();
        var interval = {
            from: new Date(date.getFullYear(), 0, 1),
            to: new Date(),
        }
        return interval;
    }
    function getCurrentWeekInterval() {
        var thisWeek = new Date(new Date().getTime() - 60 * 60 * 24 * 1000)
        var day = thisWeek.getDay();
        var LastMonday;
        if (day === 0) {
            LastMonday = new Date();
        }
        else {
            var diffToMonday = thisWeek.getDate() - day + (day === 0 ? -6 : 1);
            var LastMonday = new Date(thisWeek.setDate(diffToMonday));
        }


        var interval = {
            from: LastMonday,
            to: new Date(),
        }
        return interval;
    }
    function getLastWeekInterval() {
        var beforeOneWeek = new Date(new Date().getTime() - 60 * 60 * 24 * 7 * 1000)
        var day = beforeOneWeek.getDay();

        var diffToMonday = beforeOneWeek.getDate() - day + (day === 0 ? -6 : 1);
        var beforeLastMonday = new Date(beforeOneWeek.setDate(diffToMonday));
        var lastSunday = new Date(beforeOneWeek.setDate(diffToMonday + 6));
        var interval = {
            from: beforeLastMonday,
            to: lastSunday,
        }
        return interval;
    }
    function getCurrentMonthInterval() {
        var date = new Date();
        var interval = {
            from: new Date(date.getFullYear(), date.getMonth(), 1),
            to: new Date(),
        }
        return interval;
    }
    function getTodayInterval() {
        var date = new Date();
        var interval = {
            from: date,
            to: date
        }
        return interval;
    }
    function getYesterdayInterval() {
        var date = new Date();
        date.setDate(date.getDate() - 1);
        var interval = {
            from: date,
            to: date,
        }
        return interval;
    }
    function getLastMonthInterval() {
        var date = new Date();
        var interval = {
            from: new Date(date.getFullYear(), date.getMonth() - 1, 1),
            to: new Date(date.getFullYear(), date.getMonth(), 0),
        }
        return interval;
    }
    function getLastYearInterval() {
        var date = new Date();
        var interval = {
            from: new Date(date.getFullYear() - 1, 0, 1),
            to: new Date(date.getFullYear() - 1, 11, 31)
        }
        return interval;
    }
    function getShortDate (date) {
        var dateString = '';
        if (date) {

            var day = "" + (parseInt(date.getDate()));
            if (day.length == 1)
                dateString += "0" + day;
            else
                dateString += day;
            var month = "" + (parseInt(date.getMonth()) + 1);
            if (month.length == 1)
                dateString += "/0" + month;
            else
                dateString += "/" + month;
            dateString += "/" + date.getFullYear();
        }
        return dateString;
    }

    return ({
        replaceAll: replaceAll,
        waitMultipleAsyncOperations: waitMultipleAsyncOperations,
        getItemIndexByVal: getItemIndexByVal,
        getItemByVal: getItemByVal,
        contains: contains,
        getPropValuesFromArray: getPropValuesFromArray,
        getPropMaxValueFromArray: getPropMaxValueFromArray,
        downloadFile: downloadFile,
        getLogEntryTypeDescription: getLogEntryTypeDescription,
        getLogEntryTypeColor: getLogEntryTypeColor,
        getLogEntryType: getLogEntryType,
        getEnum: getEnum,
        dateToServerFormat: dateToServerFormat,
        getPeriod: getPeriod,
        getPropMinValueFromArray: getPropMinValueFromArray,
        getShortDate: getShortDate,
        getArrayEnum: getArrayEnum
    });

}]);