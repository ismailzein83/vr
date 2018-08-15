'use strict';

app.directive('vrCommonHttpconnectionEditor', ['UtilsService',
    function (UtilsService) {
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

                UtilsService.waitMultiplePromises([settingsGridAPIReadyPromiseDeferred.promise, headerGridAPIReadyPromiseDeferred.promise]).then(function () {
                    defineAPI();
                });
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined && payload.data != undefined) {
                        $scope.scopeModel.baseURL = payload.data.BaseURL;
                        if (payload.data.Headers != undefined)
                            $scope.scopeModel.headers = payload.data.Headers;

                        if (payload.data.WorkflowRetrySettings != undefined)
                            $scope.scopeModel.settings = payload.data.WorkflowRetrySettings;
                    }
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
                        Headers: headers,
                        WorkflowRetrySettings: settings
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }]);