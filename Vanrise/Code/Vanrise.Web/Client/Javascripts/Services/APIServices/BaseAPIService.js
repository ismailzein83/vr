
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
app.service('BaseAPIService', ['$http', '$q', 'Sec_CookieService', '$location', '$rootScope', 'notify', 'DataRetrievalResultTypeEnum', '$injector', 'HttpStatusCodeEnum', 'UtilsService', 'VRModalService',
    function BaseAPIService($http, $q, Sec_CookieService, $location, $rootScope, notify, DataRetrievalResultTypeEnum, $injector, HttpStatusCodeEnum, UtilsService, VRModalService) {

    var loginURL = '/Security/Login';
    var paymnetURL = '/Security/Payment';

    function redirectToLoginPage() {
        if (location.pathname.indexOf('/Security/Login') < 0) {
            window.location.href = loginURL + '?redirectTo=' + encodeURIComponent(window.location.href);
        }
    }

    function redirectToPaymentPage() {
        if (location.pathname.indexOf('/Security/Payment') < 0) {
            window.location.href = paymnetURL;
        }
    }

    function setLoginURL(value) {
        if (value != undefined && value != '')
            loginURL = value;
    }

    var pendingWEBAPICallHandles = [];

    function get(url, params, options, originalDeferred) {
        var deferred = originalDeferred != undefined ? originalDeferred : $q.defer();
        var callHandle = createCallHandle();
        var urlParameters;
        if (params) 
            urlParameters = {
                params: params,
            };

        if (options != undefined) {
            if (options.responseTypeAsBufferArray != undefined)
                urlParameters.responseType = 'arraybuffer';
            if (options.useCache === true)
                urlParameters.cache = true;
        }
        
        $http.get(url, urlParameters)
            .success(function (response, status, headers, config) {
                removeCallHandle(callHandle);
                var returnedResponse;
                var headersTab = headers();
                $rootScope.clock = UtilsService.dateToServerFormat(headersTab.serverdate);
                if (options != undefined && options.returnAllResponseParameters) {
                    returnedResponse = {
                        data: response,
                        status: status,
                        headers: headers,
                        config: config
                    };
                }
                else
                    returnedResponse = response;
                deferred.resolve(returnedResponse);
            })
            .error(function (data, status, headers, config) {
                var recallWebAPIFunction = function () {
                    get(url, params, options, deferred);
                };
                handleAPIError(data, status, headers, config, deferred, callHandle, recallWebAPIFunction);
                removeCallHandle(callHandle);
            });
        return deferred.promise;
    }

    function post(url, dataToSend, options, originalDeferred) {
        var deferred = originalDeferred != undefined ? originalDeferred : $q.defer();
        var callHandle = createCallHandle();
        var data;
        if (dataToSend)
            data = UtilsService.convertDatePropertiesToString(dataToSend);
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
        };
        $http(req)
            .success(function (response, status, headers, config) {
                removeCallHandle(callHandle);
                var headersTab = headers();
                $rootScope.clock = UtilsService.dateToServerFormat(headersTab.serverdate);
                var returnedResponse;
                if (isExport || (options != undefined && options.returnAllResponseParameters)) {
                    returnedResponse = {
                        data: response,
                        status: status,
                        headers: headers,
                        config: config
                    };
                }
                else
                    returnedResponse = response;
                deferred.resolve(returnedResponse);
            })
            .error(function (data, status, headers, config) {
                var recallWebAPIFunction = function () {
                    post(url, dataToSend, options, deferred);
                };
                handleAPIError(data, status, headers, config, deferred, callHandle, recallWebAPIFunction);
                removeCallHandle(callHandle);
            });
        return deferred.promise;

    }

    function showErrorMessage(message)
    {
        notify({ message: message, classes: "alert alert-danger" });
    }

    function handleAPIError(data, status, headers, config, deferred, callHandle, recallWebAPIFunction) {
        if (HttpStatusCodeEnum.Unauthorized.value === status) {
            if (Sec_CookieService.isAccessCookieAvailable()) {
                askForLoginPassword(callHandle).then(function () {
                    recallWebAPIFunction();
                }).catch(function () {
                    redirectToLoginPage();
                    deferred.reject(data);
                });
            }
            else {
                redirectToLoginPage();
                deferred.reject(data);
            }
        }
        else if (HttpStatusCodeEnum.Forbidden.value === status) {
            console.log('Error Occured: ' + data);
            notify.closeAll();
            showErrorMessage(data);
            setTimeout(function () {
                notify.closeAll();
            }, 3000);
            deferred.reject(data);
        }
        else {
            if (HttpStatusCodeEnum.PaymentRequired.value === status) {
                redirectToPaymentPage();
            }
            else {
                console.log('');
                if (data != undefined)
                    console.log('Error Occured: ' + data.ExceptionMessage);
                console.log('');
                console.log(data);
                notify.closeAll();

                var exceptionMessage = headers("ExceptionMessage");
                if (exceptionMessage != undefined) {
                    showErrorMessage(exceptionMessage);
                } else {
                    showErrorMessage('Error Occured while posting data!');
                }
                setTimeout(function () {
                    notify.closeAll();
                }, 3000);
            }

            deferred.reject(data);
        }
    }

    function createCallHandle() {
        var callHandle = {};
        pendingWEBAPICallHandles.push(callHandle);
        if (isAskingForLoginPassword)
            pendingCallsAfterLastAskForLogin.push(callHandle);
        return callHandle;
    }

    function removeCallHandle(callHandle) {
        pendingWEBAPICallHandles.splice(pendingWEBAPICallHandles.indexOf(callHandle), 1);
        var indexInAfterLastAskForLogin = pendingCallsAfterLastAskForLogin.indexOf(callHandle);
        if (indexInAfterLastAskForLogin >= 0)
            pendingCallsAfterLastAskForLogin.splice(indexInAfterLastAskForLogin, 1);
    }

    var askForLoginPasswordDeferred;
    var pendingCallsAfterLastAskForLogin = [];
    var isAskingForLoginPassword;
    function askForLoginPassword(callHandle) {
        if (!isAskingForLoginPassword) {
            if (pendingCallsAfterLastAskForLogin.length > 0) {
                var quickDeferred = $q.defer();
                var callHandleIndex = pendingCallsAfterLastAskForLogin.indexOf(callHandle);
                if (callHandleIndex >= 0) {
                    pendingCallsAfterLastAskForLogin.splice(callHandleIndex, 1);
                    quickDeferred.resolve();
                }
                else {
                    quickDeferred.reject();
                }
                return quickDeferred.promise;
            }
            else {
                askForLoginPasswordDeferred = $q.defer();
                for (var i = 0; i < pendingWEBAPICallHandles.length; i++) {
                    pendingCallsAfterLastAskForLogin.push(pendingWEBAPICallHandles[i]);
                }

                var modalSettings = {};

                modalSettings.onScopeReady = function (modalScope) {
                    modalScope.onAuthenticated = function () {
                        isAskingForLoginPassword = false;
                        askForLoginPasswordDeferred.resolve();
                        askForLoginPasswordDeferred = undefined;
                    };
                };

                isAskingForLoginPassword = true;
                VRModalService.showModal('/Client/Javascripts/Services/APIServices/ReenterPasswordModal.html', null, modalSettings).finally(
                    function () {
                        isAskingForLoginPassword = false;
                        if (askForLoginPasswordDeferred != undefined) {
                            askForLoginPasswordDeferred.reject();
                            askForLoginPasswordDeferred = undefined;
                        }
                });

            }
        }
        pendingCallsAfterLastAskForLogin.splice(pendingCallsAfterLastAskForLogin.indexOf(callHandle), 1);
        return askForLoginPasswordDeferred.promise;
    }


    return ({
        get: get,
        post: post,
        setLoginURL: setLoginURL
    });

}]);