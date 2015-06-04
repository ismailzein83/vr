'use strict';

app.service('UtilsService', ['$q', function ($q) {

    return ({
        replaceAll: replaceAll,
        waitMultipleAsyncOperations: waitMultipleAsyncOperations,
        getItemIndexByVal: getItemIndexByVal,
        getItemByVal: getItemByVal
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
            operation().then(function () {
                if (isRejected)
                    return;
                pendingOperations--;

                if (pendingOperations == 0)
                    deferred.resolve();
            }).catch(function (error) {
                deferred.reject(error);
                isRejected = true;
            });
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
}]);