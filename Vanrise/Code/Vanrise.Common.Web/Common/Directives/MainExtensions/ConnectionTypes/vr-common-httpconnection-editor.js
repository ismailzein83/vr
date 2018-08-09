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

            var gridAPI;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.datasource = [];

                $scope.scopeModel.onHttpHeaderGridReady = function (api) {
                    gridAPI = api;
                };

                $scope.scopeModel.addHttpHeader = function () {
                    var dataItem;
                    dataItem = {
                        Name: "",
                        Value: ""
                    };
                    $scope.scopeModel.datasource.push(dataItem);
                };

                $scope.scopeModel.isNameValid = function () {
                    if ($scope.scopeModel.datasource.length == 0)
                        return;

                    for (var i = 0; i < $scope.scopeModel.datasource.length; i++) {
                        var currentItem = $scope.scopeModel.datasource[i];
                        for (var j = i + 1; j < $scope.scopeModel.datasource.length; j++) {
                            var dataItem = $scope.scopeModel.datasource[j];
                            if (dataItem.Name === currentItem.Name)
                                return 'This name already exists';
                        }
                    }
                    return null;
                };

                $scope.scopeModel.removeHttpHeader = function (dataItem) {
                    $scope.scopeModel.datasource.splice($scope.scopeModel.datasource.indexOf(dataItem), 1);
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var headers;

                    if (payload != undefined && payload.data != undefined) {
                        $scope.scopeModel.baseURL = payload.data.BaseURL;
                        $scope.scopeModel.datasource = payload.data.Headers;
                    }
                };

                api.getData = function () {
                    var headers = [];
                    angular.forEach($scope.scopeModel.datasource, function (item) {
                        var dataItem = {
                            Name: item.Name,
                            Value: item.Value
                        };
                        headers.push(dataItem);
                    });

                    return {
                        $type: "Vanrise.Common.Business.VRHttpConnection, Vanrise.Common.Business",
                        BaseURL: $scope.scopeModel.baseURL,
                        Headers: headers
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }]);