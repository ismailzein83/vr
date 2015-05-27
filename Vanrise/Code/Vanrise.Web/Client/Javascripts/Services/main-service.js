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
        if (!angular.isObject(response.data) || !response.data.Message) {
            return ($q.reject("An unknown error occurred."));
        }
        return ($q.reject(response.data.Message));
    }

    function handleSuccess(response) {
        return (response.data);
    }

});

app.service('HttpService', function ($http, MainService) {

    return ({
        get: get,
        post:post
    });

    function get(url, params) {

        var request = $http({
            method: "get",
            url: url,
            params: params
        });
        return (request.then(MainService.handleSuccess, MainService.handleError));
    }

    function post(url, data) {

        var request = $http({
            method: "post",
            url: url,
            data: data
        });
        return (request.then(MainService.handleSuccess, MainService.handleError));
    }

});