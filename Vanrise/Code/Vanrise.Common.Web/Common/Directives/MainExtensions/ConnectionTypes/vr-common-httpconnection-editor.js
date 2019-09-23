'use strict';

app.directive('vrCommonHttpconnectionEditor', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new HttpConnectionCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: "/Client/Modules/Common/Directives/MainExtensions/ConnectionTypes/Templates/HttpConnectionEditorTemplate.html"
        };

        function HttpConnectionCtor(ctrl, $scope, attrs) {
            this.initializeController = initializeController;

            var headerGridAPI;
            var headerGridAPIReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var settingsGridAPI;
            var settingsGridAPIReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var httpconnectioncallinterceptorSelectiveAPI;
            var httpconnectioncallinterceptorSelectiveReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.headers = [];
                $scope.scopeModel.settings = [];

                $scope.scopeModel.onHttpHeaderGridReady = function (api) {
                    headerGridAPI = api;
                    headerGridAPIReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onSettingGridReady = function (api) {
                    settingsGridAPI = api;
                    settingsGridAPIReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onHttpconnectioncallinterceptorSelectiveReady = function (api) {
                    httpconnectioncallinterceptorSelectiveAPI = api;
                    httpconnectioncallinterceptorSelectiveReadyDeferred.resolve();
                };

                $scope.scopeModel.addHttpHeader = function () {
                    var dataItem;
                    dataItem = {
                        Name: "",
                        Value: ""
                    };
                    $scope.scopeModel.headers.push(dataItem);
                };

                $scope.scopeModel.addSettings = function () {
                    var dataItem = {};
                    $scope.scopeModel.settings.push(dataItem);

                };

                $scope.scopeModel.isNameValid = function () {
                    if ($scope.scopeModel.headers.length == 0)
                        return;

                    for (var i = 0; i < $scope.scopeModel.headers.length; i++) {
                        var currentItem = $scope.scopeModel.headers[i];
                        for (var j = i + 1; j < $scope.scopeModel.headers.length; j++) {
                            var dataItem = $scope.scopeModel.headers[j];
                            if (dataItem.Name === currentItem.Name)
                                return 'This name already exists';
                        }
                    }
                    return null;
                };

                $scope.scopeModel.removeHttpHeader = function (dataItem) {
                    $scope.scopeModel.headers.splice($scope.scopeModel.headers.indexOf(dataItem), 1);
                };

                $scope.scopeModel.removeSetting = function (dataItem) {
                    $scope.scopeModel.settings.splice($scope.scopeModel.settings.indexOf(dataItem), 1);
                };

                $scope.scopeModel.onEnableLoggingValueChanged = function () {
                    if (!$scope.scopeModel.enableLogging) {
                        resetLoggingSwitches();
                    }
                };

                UtilsService.waitMultiplePromises([settingsGridAPIReadyPromiseDeferred.promise, headerGridAPIReadyPromiseDeferred.promise]).then(function () {
                    defineAPI();
                });
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined && payload.data != undefined) {
                        $scope.scopeModel.baseURL = payload.data.BaseURL;

                        $scope.scopeModel.enableLogging = payload.data.EnableLogging;
                        $scope.scopeModel.enableParametersLogging = payload.data.EnableParametersLogging;
                        $scope.scopeModel.enableRequestHeaderLogging = payload.data.EnableRequestHeaderLogging;
                        $scope.scopeModel.enableRequestLogging = payload.data.EnableRequestLogging;
                        $scope.scopeModel.enableResponseHeaderLogging = payload.data.EnableResponseHeaderLogging;
                        $scope.scopeModel.enableResponseLogging = payload.data.EnableResponseLogging;

                        if (payload.data.Headers != undefined)
                            $scope.scopeModel.headers = payload.data.Headers;

                        if (payload.data.WorkflowRetrySettings != undefined)
                            $scope.scopeModel.settings = payload.data.WorkflowRetrySettings;
                    }

                    var httpconnectioncallinterceptorSelectiveLoadDeferred = UtilsService.createPromiseDeferred();

                    httpconnectioncallinterceptorSelectiveReadyDeferred.promise.then(function () {
                        var interceptorPayload = {
                            Interceptor: (payload != undefined && payload.data != undefined) ? payload.data.Interceptor : null
                        };
                        VRUIUtilsService.callDirectiveLoad(httpconnectioncallinterceptorSelectiveAPI, interceptorPayload, httpconnectioncallinterceptorSelectiveLoadDeferred);
                    });

                };

                api.getData = function () {
                    var headers;
                    if ($scope.scopeModel.headers.length > 0) {
                        headers = [];
                        angular.forEach($scope.scopeModel.headers, function (item) {
                            var dataItem = {
                                Name: item.Name,
                                Value: item.Value
                            };
                            headers.push(dataItem);
                        });
                    }

                    var settings;
                    if ($scope.scopeModel.settings.length > 0) {
                        settings = [];
                        angular.forEach($scope.scopeModel.settings, function (item) {
                            var dataItem = {
                                MaxRetryCount: item.MaxRetryCount,
                                RetryInterval: item.RetryInterval
                            };
                            settings.push(dataItem);
                        });
                    }

                    return {
                        $type: "Vanrise.Common.Business.VRHttpConnection, Vanrise.Common.Business",
                        BaseURL: $scope.scopeModel.baseURL,

                        EnableLogging: $scope.scopeModel.enableLogging,
                        EnableParametersLogging: $scope.scopeModel.enableParametersLogging,
                        EnableRequestHeaderLogging: $scope.scopeModel.enableRequestHeaderLogging,
                        EnableRequestLogging: $scope.scopeModel.enableRequestLogging,
                        EnableResponseHeaderLogging: $scope.scopeModel.enableResponseHeaderLogging,
                        EnableResponseLogging: $scope.scopeModel.enableResponseLogging,

                        Headers: headers,
                        WorkflowRetrySettings: settings,
                        Interceptor: (httpconnectioncallinterceptorSelectiveAPI != null) ? httpconnectioncallinterceptorSelectiveAPI.getData() : null
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function resetLoggingSwitches() {
                $scope.scopeModel.enableParametersLogging = false;
                $scope.scopeModel.enableRequestHeaderLogging = false;
                $scope.scopeModel.enableRequestLogging = false;
                $scope.scopeModel.enableResponseHeaderLogging = false;
                $scope.scopeModel.enableResponseLogging = false;
            }
        }
    }]);