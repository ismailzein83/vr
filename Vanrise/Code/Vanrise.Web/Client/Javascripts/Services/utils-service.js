'use strict';

app.service('UtilsService', ['$q', 'LogEntryTypeEnum', 'LabelColorsEnum', function ($q, LogEntryTypeEnum, LabelColorsEnum) {

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
        getEnum:getEnum
    });

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
        if (logEntryType) {
            var enumObj = getEnum(LogEntryTypeEnum, 'value', logEntryType);
            if (enumObj) return enumObj.description;
        }
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
}]);