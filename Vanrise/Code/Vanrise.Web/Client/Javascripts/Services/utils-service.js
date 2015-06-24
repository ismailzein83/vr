'use strict';

app.service('UtilsService', ['$q', function ($q) {

    return ({
        replaceAll: replaceAll,
        waitMultipleAsyncOperations: waitMultipleAsyncOperations,
        getItemIndexByVal: getItemIndexByVal,
        getItemByVal: getItemByVal,
        contains: contains,
        getPropValuesFromArray: getPropValuesFromArray,
        getPropMaxValueFromArray: getPropMaxValueFromArray
    });

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

            angular.forEach(array, function (val) {
                if (val === undefined)
                    max = val[propName];
                if (val[propName] > max)
                    max = val[propName];
            });
        }
        return max;
    }
}]);