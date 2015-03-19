'use strict';

app.service('MainService', function ($q) {

    return ({
        getBaseURL: getBaseURL,
        handleError: handleError,
        handleSuccess: handleSuccess
    });

    function getBaseURL() {
        var pathArray = location.href.split('/');
        return pathArray[0] + '//' + pathArray[2];
    }

    function handleError(response) {
        if (!angular.isObject(response.data) || !response.data.message) {
            return ($q.reject("An unknown error occurred."));
        }
        return ($q.reject(response.data.message));
    }

    function handleSuccess(response) {
        return (response.data);
    }

});