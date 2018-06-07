'use strict';
(function (app) {

    UtilsService.$inject = ['$q', 'LogEntryTypeEnum', 'LabelColorsEnum'];

    function UtilsService($q, LogEntryTypeEnum, LabelColorsEnum) {

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
                date = date ? new Date(date) : new Date();
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
            isoFullDateTime: "yyyy-mm-dd'T'HH:MM:ss.l",
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


        function isContextReadOnly(scope, ctrl) {
            while (scope != null) {
                if (typeof (scope.getReadOnly) == 'function') {
                    return scope.getReadOnly();
                }
                scope = scope.$parent;
            }
            return false;
        }

        function setContextReadOnly(scope) {
            scope.getReadOnly = function () {
                return true;
            };
        }

        function getEnum(enumObj, propertyFilter, valueFilter) {
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

        function getEnumDescription(dataEnum, value) {
            var enumObj = getEnum(dataEnum, 'value', value);
            if (enumObj) return enumObj.description;
            return undefined;
        }

        function validateTimeOffset(value) {

            if (value == undefined) return null;

            var offset = value.split(".");

            if (offset.length == 1) {
                var time = offset[0].split("-");

                if (time.length == 1 && validateTime(time[0]))
                    return null;

                else if (time.length == 2 && time[0].length == 0 && validateTime(time[1]))
                    return null;
            }
            else if (offset.length == 2) {
                var days = offset[0].split("-");

                if (days.length == 1 && validateInteger(days[0], 99) && validateTime(offset[1]))
                    return null;

                else if (days.length == 2 && days[0].length == 0 && validateInteger(days[1], 99) && validateTime(offset[1]))
                    return null;
            }

            return "Format: DD.HH:MM:SS";
        }

        function validateInteger(integer, maxValue) {
            var parsedInt = parseInt(integer);

            if (isNaN(parsedInt) || parsedInt < 0 || parsedInt > maxValue) return false;

            return true;
        }

        function validateTime(time) { // the valid time format is HH:MM:SS

            if (time.length != 8) return false;

            var timeArray = time.split(":");

            if (timeArray.length != 3 || timeArray[0].length != 2 || timeArray[1].length != 2 || timeArray[2].length != 2)
                return false;

            if (validateInteger(timeArray[0], 23) && validateInteger(timeArray[1], 59) && validateInteger(timeArray[2], 59))
                return true;

            return false;
        }

        function getLogEntryTypeColor(logEntryType) {

            var color = undefined;

            switch (logEntryType) {
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

                if (promise != undefined) {
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

        function waitMultiplePromises(promises) {
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

        function waitPromiseNode(promiseNode) {
            var resultPromiseDeferred = createPromiseDeferred();
            waitMultiplePromises(promiseNode.promises).then(function () {
                if (promiseNode.getChildNode != undefined && typeof (promiseNode.getChildNode) == 'function') {
                    var childNodePromise = waitPromiseNode(promiseNode.getChildNode());
                    linkExistingPromiseToPromiseDeferred(childNodePromise, resultPromiseDeferred);
                }
                else {
                    resultPromiseDeferred.resolve();
                }
            }).catch(function (error) {
                resultPromiseDeferred.reject(error);
            });
            return resultPromiseDeferred.promise;
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

        function linkExistingPromiseToPromiseDeferred(existingPromise, promiseDeferred) {
            existingPromise
                .then(function (response) {
                    promiseDeferred.resolve(response);
                })
                .catch(function (error) {
                    promiseDeferred.reject(error);
                });
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
                    return i;
                }
            }
            return -1;
        }

        //This function depends that the attribute to compare with is a string, that is why i did not reuse getItemIndexByVal that we can pass for it any type
        function getItemIndexByStringVal(array, value, attname, ignoreCase) {
            for (var i = 0; i < array.length; i++) {
                var valueFromArray = eval('array[' + i + '].' + attname);
                if ((ignoreCase == true && valueFromArray.toUpperCase() == value.toUpperCase()) || valueFromArray == value) {
                    return i;
                }
            }
            return -1;
        }

        function getItemByVal(array, value, attname) {
            for (var i = 0; i < array.length; i++) {
                if (eval('array[' + i + '].' + attname) == value) {
                    return array[i];
                }
            }
            return null;
        }

        function contains(array, obj) {

            if (typeof obj == "string") {
                for (var i = 0; i < array.length; i++) {
                    if (!isNaN(array[i])) {
                        if (array[i] === obj) {
                            return true;
                        }
                    }
                    else {
                        if (array[i].toLowerCase() === obj.toLowerCase()) {
                            return true;
                        }
                    }

                }
            }
            else {
                for (var i = 0; i < array.length; i++) {
                    if (array[i] === obj) {
                        return true;
                    }
                }
            }

            return false;
        }
        function getFilteredArrayFromArray(array, value, propName) {
            var filteredArray;
            if (array.length > 0) {
                filteredArray = [];
                angular.forEach(array, function (val) {
                    if (val[propName] == value)
                        filteredArray.push(val);
                });
            }
            return filteredArray;
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
            var octetStreamMime = 'application/octet-stream';
            var success = false;
            var headersTab = headers();
            var matcher = /filename[^;=\n]*=((['"]).*?\2|[^;\n]*)/i;

            var results = headersTab['content-disposition'].match(matcher);
            var name = replaceAll(results[1], "\"", ""); //for UI side fixing
            var filename = replaceAll(name, "%20", " ");
            var contentType = headersTab['content-type'] || octetStreamMime;
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
                return dateFormat(date, dateFormat.masks.isoFullDateTime);
            else
                return date;
        }

        function cloneDateTime(date) {
            return new Date(date).toUTCString().replace(' GMT', '');
        }

        function getShortDate(date) {
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
            var from = (fromDate instanceof Date) ? fromDate : createDateFromString(fromDate);
            var to = (toDate instanceof Date) ? toDate : createDateFromString(toDate);
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
        function buildTitleForUploadEditor(entityType) {
            return "Upload " + entityType;
        }

        function buildTitleForUpdateEditor(entityTitle, entityType, scope) {
            var title = (isContextReadOnly(scope)) ? "View " : "Edit ";
            if (entityType != undefined)
                title += entityType + ": " + entityTitle;
            else
                title += entityTitle;
            return title;
        }

        function escapeRegExp(string) {
            return string.replace(/([.*+?^=!:${}()|\[\]\/\\])/g, "\\$1");
        }

        function serializetoJson(obj) {
            if (obj != null)
                obj = convertDatePropertiesToString(obj);
            return angular.toJson(obj);
        }

        function convertDatePropertiesToString(obj) {
            if (obj == null || typeof obj != "object")
                return obj;
            for (var k in obj) {
                var propValue = obj[k];
                if (propValue == null)
                    continue;

                if (propValue instanceof Date)
                    obj[k] = dateToServerFormat(propValue);
                else if (isArray(propValue)) {
                    for (var i = 0; i < propValue.length; i++) {
                        convertDatePropertiesToString(propValue[i]);
                    }
                }
                else if (typeof propValue == "object")
                    convertDatePropertiesToString(propValue);
            }
            return obj;
        }

        function isArray(obj) {
            return Object.prototype.toString.call(obj) === '[object Array]';
        }

        function parseStringToJson(jsonString) {
            return angular.fromJson(jsonString);
        }

        function generateJSVariableName() {
            return 'dyn_' + replaceAll(guid(), '-', '');
        }

        function safeApply(scope, callBack) {
            //var phase = scope.$$phase;
            //if (phase == '$apply' || phase == '$digest') {
            //    if (callBack && (typeof (callBack) === 'function')) {
            //        callBack();
            //    }
            //} else {
            return scope.$apply(callBack);
            //}
        }

        function compareEqualsTimes(t1, t2) {
            if (t1.Hour == t2.Hour && t1.Minute == t2.Minute)
                return true;
            else
                return false;
        }

        function mergeObject(mainObj, input, replaceExisting) {
            for (var attrname in input) {
                if (mainObj[attrname] == undefined)
                    mainObj[attrname] = input[attrname];
                else if (replaceExisting)
                    mainObj[attrname] = input[attrname];
            }
            return mainObj;
        }

        function getSystemActionNames(moduleName, controllerName, methodNames) {
            var actionNames = '';
            if (methodNames != undefined && methodNames != null) {
                for (var i = 0; i < methodNames.length; i++) {
                    if (i > 0) { actionNames += '&'; }
                    actionNames += (moduleName + '/' + controllerName + '/' + methodNames[i]);
                }
            }
            return actionNames;
        }

        function areDateTimesEqual(first, second) {
            return (serializetoJson(first) == serializetoJson(second));
        }
        function getTimeOffset(date1, date2) {
            var one_day = 1000 * 60 * 60 * 24;

            // Convert both dates to milliseconds
            var date1_ms = date1.getTime();
            var date2_ms = date2.getTime();
            var sign = "";
            // Calculate the difference in milliseconds
            var difference_ms = date2_ms - date1_ms;
            if (difference_ms < 0)
                sign = "-";
            //take out milliseconds
            if (difference_ms < 0)
                difference_ms = -difference_ms;
            difference_ms = difference_ms / 1000;
            var seconds = Math.floor(difference_ms % 60);
            difference_ms = difference_ms / 60;
            var minutes = Math.floor(difference_ms % 60);
            difference_ms = difference_ms / 60;
            var hours = Math.floor(difference_ms % 24);
            var days = Math.floor(difference_ms / 24);

            var result = sign;
            if (hours < 10)
                result += "0" + hours;
            else
                result += hours;
            if (minutes < 10)
                result += ":0" + minutes;
            else
                result += ":" + minutes;

            if (seconds < 10)
                result += ":0" + seconds;
            else
                result += ":" + seconds;

            return result;
        }

        function trimRight(value, charlist) {
            if (charlist === undefined)
                charlist = "\s";

            return value.replace(new RegExp("[" + charlist + "]+$"), "");
        };

        function trimLeft(value, charlist) {
            if (charlist === undefined)
                charlist = "\s";

            return value.replace(new RegExp("^[" + charlist + "]+"), "");
        };

        function trim(value, charlist) {
            value = trimLeft(value, charlist);
            return trimRight(value, charlist);
        };
        function getUploadedFileName(fileName) {
            if (fileName != undefined) {
                var splitedFileName = fileName.split(".");
                if (splitedFileName != undefined && splitedFileName.length > 0) {
                    return splitedFileName[0];
                }
            }

        }

        function round(value, precision) {
            var multiplier = Math.pow(10, precision || 0);
            return Math.round(value * multiplier) / multiplier;
        }

        function diffDays(date1, date2) {
            date2.setHours(23);
            date2.setMinutes(59);
            date2.setSeconds(59);
            var timeDiff = Math.abs(date2.getTime() - date1.getTime());
            var diffDays = Math.ceil(timeDiff / (1000 * 3600 * 24));
            return diffDays;
        }

        var isShouldSubsctractOffsetfromDate;

        function shouldSubsctractOffsetfromDate() {
            if (isShouldSubsctractOffsetfromDate == undefined) {
                var convertedDate = new Date("2000-01-01T00:00:00");
                isShouldSubsctractOffsetfromDate = (convertedDate.getHours() != 0);
            }
            return isShouldSubsctractOffsetfromDate;
        }

        function createDateFromString(value) {
            var date = new Date(value);
            if (shouldSubsctractOffsetfromDate()) {
                var userTimezoneOffset = date.getTimezoneOffset() * 60000;
                return new Date(date.getTime() + userTimezoneOffset);
            }
            else {
                return date;
            }
        }

        function getDateFromDateTime(dateTime) {
            var date = new Date(dateTime);
            date.setHours(0, 0, 0, 0);
            return date;
        }

        function parseDateToString(date) {
            var dateAsString = '';
            if (date) {

                var day = "" + (parseInt(date.getDate()));
                if (day.length == 1)
                    dateAsString += "0" + day;
                else
                    dateAsString += day;
                var month = "" + (parseInt(date.getMonth()) + 1);
                if (month.length == 1)
                    dateAsString += "/0" + month;
                else
                    dateAsString += "/" + month;
                dateAsString += "/" + date.getFullYear();
            }
            return dateAsString;
        }

        function isNumericValue(value) {
            return !isNaN(value);
        }

        function isIntegerValue(value) {
            return !isNaN(parseFloat(value)) && isFinite(value) && Math.floor(value) === value;
        }

        function addFloats(f1, f2) {
            //Helper function to find the number of decimal places
            function findDec(f1) {
                var count;
                function isInt(n) {
                    return typeof n === 'number' &&
                       parseFloat(n) == parseInt(n, 10) && !isNaN(n);
                }
                var a = Math.abs(f1);
                f1 = a, count = 1;
                while (!isInt(f1) && isFinite(f1)) {
                    f1 = a * Math.pow(10, count++);
                }
                return count - 1;
            }
            //Determine the greatest number of decimal places
            var pf1 = parseFloat(f1);
            var pf2 = parseFloat(f2);
            var dec1 = findDec(pf1);
            var dec2 = findDec(pf2);
            var fixed = dec1 > dec2 ? dec1 : dec2;

            //do the math then do a toFixed, could do a toPrecision also
            var n = (pf1 + pf2).toFixed(fixed);
            return +n;
        }

        function getBaseUrlPrefix() {
            var pathArray = location.href.split('/');
            var protocol = pathArray[0];
            var host = pathArray[2];
            var baseurl = protocol + '//' + host + '/';
            return baseurl;
        }
        function areTimePeriodsOverlapped(p1From, p1To, p2From, p2To) {
            if ((isNaN(p1To) || p1To > p2From) && (isNaN(p2To) || p2To > p1From))
                return true;
            return false;
        }
        function getDateObject(date) {
            if (date instanceof Date)
                return date;
            else if (typeof (date) == 'string')
                return createDateFromString(date);
            else
                return undefined;
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
            serializetoJson: serializetoJson,
            convertDatePropertiesToString: convertDatePropertiesToString,
            generateJSVariableName: generateJSVariableName,
            buildTitleForUploadEditor: buildTitleForUploadEditor,
            safeApply: safeApply,
            compareEqualsTimes: compareEqualsTimes,
            mergeObject: mergeObject,
            getSystemActionNames: getSystemActionNames,
            areDateTimesEqual: areDateTimesEqual,
            validateTimeOffset: validateTimeOffset,
            getTimeOffset: getTimeOffset,
            getFilteredArrayFromArray: getFilteredArrayFromArray,
            trimRight: trimRight,
            trimLeft: trimLeft,
            trim: trim,
            getUploadedFileName: getUploadedFileName,
            getDateFromDateTime: getDateFromDateTime,
            round: round,
            diffDays: diffDays,
            parseStringToJson: parseStringToJson,
            getItemIndexByStringVal: getItemIndexByStringVal,
            isContextReadOnly: isContextReadOnly,
            setContextReadOnly: setContextReadOnly,
            createDateFromString: createDateFromString,
            parseDateToString: parseDateToString,
            isNumericValue: isNumericValue,
            isIntegerValue: isIntegerValue,
            addFloats: addFloats,
            getBaseUrlPrefix: getBaseUrlPrefix,
            areTimePeriodsOverlapped: areTimePeriodsOverlapped,
            linkExistingPromiseToPromiseDeferred: linkExistingPromiseToPromiseDeferred,
            waitPromiseNode: waitPromiseNode,
            getDateObject: getDateObject
        });
    }

    app.service('UtilsService', UtilsService);

})(app);