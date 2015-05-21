'use strict';

app.service('UtilsService', ['$q', function ($q) {

    return ({
        replaceAll: replaceAll,
        waitMultipleAsyncOperations: waitMultipleAsyncOperations
    });

    function replaceAll(string, find, replace) {
        return string.replace(new RegExp(escapeRegExp(find), 'g'), replace);
    }

    function escapeRegExp(string) {
        return string.replace(/([.*+?^=!:${}()|\[\]\/\\])/g, "\\$1");
    }

    function waitMultipleAsyncOperations(operations) {
        var deferred = $q.defer();
        var pendingOperations = operations.lenght;
        angular.forEach(operations, function (operation) {
            operation.then(function () {
                pendingOperations--;
                if (pendingOperations == 0)
                    deferred.resolve();
            }).catch(function (error) {
                deferred.reject(error);
            });
        });
        return deferred.promise;
    }
}]);