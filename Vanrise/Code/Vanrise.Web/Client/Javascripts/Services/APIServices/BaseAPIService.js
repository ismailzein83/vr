﻿
app.config(['$httpProvider', function ($httpProvider) {
    $httpProvider.interceptors.push(function ($q, $cookies) {
        return {
            'request': function (config) {
                var userInfoCookie = $cookies['TOne_LoginTokenCookie'];
                if (userInfoCookie != undefined)
                {
                    var userInfo = JSON.parse(userInfoCookie);
                    config.headers['Auth-Token'] = userInfo.Token;
                }
                else
                {
                    config.headers['Auth-Token'] = "";
                }
                
                return config;
            }
        };
    });
}]);

app.service('BaseAPIService', function ($http, $q, $rootScope, notify) {

    return ({
        get: get,
        post: post
    });

    function get(url, params, options) {
        var deferred = $q.defer();

        var urlParameters;
        if (params)
            urlParameters = {
                params: params,
            };
        if (options != undefined && options.responseTypeAsBufferArray) 
            urlParameters.responseType = 'arraybuffer';
        
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
        if (options != undefined && options.responseTypeAsBufferArray)
            responseType = 'arraybuffer';

        var req = {
            method: 'POST',
            url: url,
            responseType:responseType,
            headers: {
                'Content-Type': ContentType
            },
            data: data
        }
        $http(req)
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
                console.log(data);
                notify.closeAll();
                notify({ message: 'Error Occured while posting data!', classes: "alert alert-danger" });
                setTimeout(function () {
                    notify.closeAll();
                }, 3000);
                deferred.reject(data);
            });
        return deferred.promise;

    }
});