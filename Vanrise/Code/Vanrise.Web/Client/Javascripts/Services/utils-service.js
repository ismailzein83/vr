﻿

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

    function getEnumPropertyAsArray(enumObj, propertyFilter) {
        var array = [];

        for (var item in enumObj) {
            if (enumObj.hasOwnProperty(item)) {
                array.push(enumObj[item][propertyFilter]);
            }
        }
        return array;
    }

    function fillArray(array, data) {
        for (var i = 0; i < data.length; i++) {
            array.push(data[i]);
        }
    }

    function getLogEntryType() {
        return getArrayEnum(LogEntryTypeEnum);
    }

    function getLogEntryTypeDescription(logEntryType) {
        return getEnumDescription(LogEntryTypeEnum, logEntryType);
    }

    function getEnumDescription(dataEnum, value)
    {
        var enumObj = getEnum(dataEnum, 'value', value);
        if (enumObj) return enumObj.description;
        return undefined;
    }

    function getLogEntryTypeColor(logEntryType) {

        var color = undefined;

        switch(logEntryType)
        {
            case LogEntryTypeEnum.Information.value:
                color = LabelColorsEnum.Info.color;
                break;
            case LogEntryTypeEnum.Warning.value:
                color = LabelColorsEnum.Warning.color;
                break;
            case LogEntryTypeEnum.Error.value:
                color = LabelColorsEnum.Error.color;
                break;
            case LogEntryTypeEnum.Verbose.value:
                color = LabelColorsEnum.Primary.color;
                break;
        }

        return color;
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

    function waitMultiplePromises(promises)    {
        var deferred = createPromiseDeferred();
        var pendingPromises = promises.length;
        var isRejected = false;
        if (pendingPromises == 0)
            deferred.resolve();
        angular.forEach(promises, function (promise) {           

                promise.then(function () {
                    if (isRejected)
                        return;
                    pendingPromises--;

                    if (pendingPromises == 0)
                        deferred.resolve();
                }).catch(function (error) {
                    deferred.reject(error);
                    isRejected = true;
                });            
        });
        return deferred.promise;
    }

    function PromiseClass() {
        var deferred = $q.defer();

        //var deferredPromise = deferred.promise;

        //this.promise2 = {
        //    then: deferredPromise.then,
        //    catch: deferredPromise.catch,
        //    finally: deferredPromise.finally
        //};
        this.promise = deferred.promise;
        this.resolve = deferred.resolve;
        this.reject = deferred.reject;
    }
    
    function createPromiseDeferred() {
        return new PromiseClass();
    }

    function convertToPromiseIfUndefined(promiseOrUndefined) {
        if (promiseOrUndefined != undefined)
            return promiseOrUndefined;
        else {//if value is undefined, create promise and resolve it
            var promiseDeferred = createPromiseDeferred();
            promiseDeferred.resolve();
            return promiseDeferred.promise;
        }
    }

    function getItemIndexByVal(array, value, attname) {
        for (var i = 0; i < array.length; i++) {
            if (eval('array[' + i + '].' + attname) == value) {
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
    function cloneDateTime(date) {
        return new Date(date).toUTCString().replace(' GMT', '');
    }

    function getPeriod(periodType) {
        switch (periodType) {
            case PeriodEnum.LastYear.value: return PeriodEnum.LastYear.getInterval();
            case PeriodEnum.LastMonth.value: return PeriodEnum.LastMonth.getInterval();
            case PeriodEnum.LastWeek.value: return PeriodEnum.LastWeek.getInterval();
            case PeriodEnum.Yesterday.value: return PeriodEnum.Yesterday.getInterval();
            case PeriodEnum.Today.value: return PeriodEnum.Today.getInterval();
            case PeriodEnum.CurrentWeek.value: return PeriodEnum.CurrentWeek.getInterval();
            case PeriodEnum.CurrentMonth.value: return PeriodEnum.CurrentMonth.getInterval();
            case PeriodEnum.CurrentYear.value: return PeriodEnum.CurrentYear.getInterval();
        }
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
    function validateDates(fromDate, toDate) {
        if (fromDate == undefined || toDate == undefined)
            return null;
        var from = new Date(fromDate);
        var to = new Date(toDate);
        if (from.getTime() > to.getTime())
            return "Start should be before end";
        else
            return null;
    }
    
    function getServiceURL(moduleName, controllerName, actionName) {
        return '/api/' + moduleName + '/' + controllerName + '/' + actionName;
    }

    function cloneObject(obj, withoutHashKey) {        
        var newObj = internalClone(obj, withoutHashKey);      
        return newObj;
    }

    function internalClone(obj, withoutHashKey) {
        if (obj === null || typeof (obj) !== 'object' || 'isActiveClone' in obj)
            return obj;

        var temp = obj.constructor(); // changed

        for (var key in obj) {
            if (key == '$$hashKey' && withoutHashKey)
                continue;
            if (Object.prototype.hasOwnProperty.call(obj, key)) {
                obj['isActiveClone'] = null;
                temp[key] = internalClone(obj[key], withoutHashKey);
                delete obj['isActiveClone'];
            }
        }

        return temp;
    }
    function guid() {
        function s4() {
            return Math.floor((1 + Math.random()) * 0x10000)
              .toString(16)
              .substring(1);
        }
        return s4() + s4() + '-' + s4() + '-' + s4() + '-' +
          s4() + '-' + s4() + s4() + s4();
    }


    function buildTitleForAddEditor(entityType) {
        return "New " + entityType;
    }

    function buildTitleForUpdateEditor(entityTitle, entityType) {
        var title = "Edit "
        if (entityType!=undefined)
            title += entityType + ": " + entityTitle;
        else
            title += entityTitle
        return title;
    }

    function escapeRegExp(string) {
        return string.replace(/([.*+?^=!:${}()|\[\]\/\\])/g, "\\$1");
    }

    function serializetoJson(obj)
    {
        return angular.toJson(obj);
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
        getArrayEnum: getArrayEnum,
        validateDates: validateDates,
        getEnumDescription: getEnumDescription,
        getEnumPropertyAsArray: getEnumPropertyAsArray,
        fillArray: fillArray,
        cloneDateTime: cloneDateTime,
        getServiceURL: getServiceURL,
        cloneObject: cloneObject,
        guid: guid,
        escapeRegExp: escapeRegExp,
        buildTitleForAddEditor: buildTitleForAddEditor,
        buildTitleForUpdateEditor: buildTitleForUpdateEditor,
        waitMultiplePromises: waitMultiplePromises,
        createPromiseDeferred: createPromiseDeferred,
        convertToPromiseIfUndefined: convertToPromiseIfUndefined,
        serializetoJson: serializetoJson
    });

}]);