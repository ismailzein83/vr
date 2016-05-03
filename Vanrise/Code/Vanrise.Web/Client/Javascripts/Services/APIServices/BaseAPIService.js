﻿
app.config(['$httpProvider', function ($httpProvider) {
    $httpProvider.interceptors.push(function ($q, $injector) {
        return {
            'request': function (config) {
                config.headers['Auth-Token'] = $injector.get('SecurityService').getUserToken();
                return config;
            }
        };
    });
}]);
app.service('BaseAPIService', function ($http, $q, $location, $rootScope, notify, DataRetrievalResultTypeEnum, $injector) {

    var loginURL = '/Security/Login';

    function redirectToLoginPage() {
        if (location.pathname.indexOf('/Security/Login') < 0) {
            window.location.href = loginURL + '?redirectTo=' + encodeURIComponent(window.location.href);
        }
    }

    return ({
        get: get,
        post: post,
        setLoginURL: setLoginURL
    });

    function setLoginURL(value) {
        if (value != undefined && value != '')
            loginURL = value;
    }


    function get(url, params, options) {
        var deferred = $q.defer();

        var urlParameters;
        if (params) 
            urlParameters = {
                params: params,
            };

        if (options != undefined) {
            if (options.responseTypeAsBufferArray != undefined)
                urlParameters.responseType = 'arraybuffer';
            if (options.useCache != undefined && options.useCache === true) {}
                urlParameters.cache = true;
        }
        
        $http.get(url, urlParameters)
            .success(function (response, status, headers, config) {
                var returnedResponse;
                if (options != undefined && options.returnAllResponseParameters) {
                    returnedResponse = {
                        data: response,
                        status: status,
                        headers: headers,
                        config: config
                    }
                }
                else
                    returnedResponse = response;
                deferred.resolve(returnedResponse);
            })
            .error(function (data, status, headers, config) {
                if (status === 401)
                    redirectToLoginPage();

                console.log(''); 
                console.log('Error Occured: ' + data.ExceptionMessage);
                console.log('');
                console.log(data);
                notify.closeAll();
                notify({ message: 'Error Occured while getting data!', classes: "alert alert-danger" });
                setTimeout(function () {
                    notify.closeAll();
                }, 3000);
                deferred.reject(data);
            });
        return deferred.promise;
    }

    function post(url, dataToSend, options) {
        var deferred = $q.defer();
        var data;
        if (dataToSend)
            data = dataToSend
        var responseType = '';
        var ContentType = 'application/json;charset=utf-8';
        var isExport = dataToSend != undefined && dataToSend.DataRetrievalResultType != undefined && dataToSend.DataRetrievalResultType == DataRetrievalResultTypeEnum.Excel.value;
        if (isExport || (options != undefined && options.responseTypeAsBufferArray))
            responseType = 'arraybuffer';

        var req = {
            method: 'POST',
            url: url,
            responseType: responseType,
            headers: {
                'Content-Type': ContentType
            },
            data: data
        }
        $http(req)
            .success(function (response, status, headers, config) {
                var returnedResponse;
                if (isExport || (options != undefined && options.returnAllResponseParameters)) {
                    returnedResponse = {
                        data: response,
                        status: status,
                        headers: headers,
                        config: config
                    }
                }
                else
                    returnedResponse = response;
                deferred.resolve(returnedResponse);
            })
            .error(function (data, status, headers, config) {
              
                if (status === 401)
                    redirectToLoginPage();

                console.log('');
                console.log('Error Occured: ' + data.ExceptionMessage);
                console.log('');
                console.log(data);
                notify.closeAll();

                var exceptionMessage = headers("ExceptionMessage");
                if (exceptionMessage != undefined) {
                    showErrorMessage(exceptionMessage)
                } else {
                    showErrorMessage('Error Occured while posting data!')
                }
                setTimeout(function () {
                    notify.closeAll();
                }, 3000);
                deferred.reject(data);
            });
        return deferred.promise;

    }

    function showErrorMessage(message)
    {
        notify({ message: message, classes: "alert alert-danger" });
    }
});