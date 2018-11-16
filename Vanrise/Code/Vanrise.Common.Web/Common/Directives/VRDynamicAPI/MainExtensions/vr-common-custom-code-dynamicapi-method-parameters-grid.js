"use strict"
app.directive("vrCommonCustomCodeDynamicapiMethodParametersGrid", ["UtilsService", "VRNotificationService", "VR_Dynamic_APIService", "VRCommon_VRDynamicAPIAPIService", "VRUIUtilsService", "VRCommon_ObjectTrackingService",
    function (UtilsService, VRNotificationService, VR_Dynamic_APIService, VRCommon_VRDynamicAPIAPIService, VRUIUtilsService, VRCommon_ObjectTrackingService) {

        var directiveDefinitionObject = {
            restrict: "E",
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var methodParametersGrid = new MethodParametersGrid($scope, ctrl, $attrs);
                methodParametersGrid.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: "/Client/Modules/Common/Directives/VRDynamicAPI/MainExtensions/Templates/CustomCodeDynamicAPIMethodParametersGrid.html"
        };

        function MethodParametersGrid($scope, ctrl) {

            var gridApi;
            var methodTypeValue;
            this.initializeController = initializeController;

            function initializeController() {

                $scope.scopeModel = {};

                $scope.scopeModel.parameters = [];

                $scope.scopeModel.disableAddingParameters = function () {

                    if (methodTypeValue == undefined || methodTypeValue.getMethodType() == undefined) return true;
                    if (methodTypeValue.getMethodType() == 2 && $scope.scopeModel.parameters.length >= 1) return true;
                    return false;
                };

                $scope.scopeModel.validateNumberOfParameters = function () {

                    if (methodTypeValue != undefined && methodTypeValue.getMethodType() == 2 && $scope.scopeModel.parameters.length > 1)
                        return 'Only One Parameter Is Allowed In Post Method!';
                    return null;
                };

                $scope.scopeModel.onGridReady = function (api) {

                    gridApi = api;

                    if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function") {
                        ctrl.onReady(getDirectiveApi());
                    }

                    function getDirectiveApi() {

                        var directiveApi = {};

                        directiveApi.load = function (payload) {
                            if (payload != undefined) {
                                if (payload.InParameters != undefined && payload.InParameters.length != 0) {
                                    for (var j = 0; j < payload.InParameters.length; j++) {
                                        $scope.scopeModel.parameters.push({ parameterName: payload.InParameters[j].ParameterName, parameterType: payload.InParameters[j].ParameterType });
                                    }
                                }
                                if (payload.context != undefined) {
                                    methodTypeValue = payload.context;
                                }
                            }
                        };

                        directiveApi.getData = function () {

                            var inParameters = [];
                            for (var j = 0; j < $scope.scopeModel.parameters.length; j++) {
                                inParameters.push({ ParameterName: $scope.scopeModel.parameters[j].parameterName, ParameterType: $scope.scopeModel.parameters[j].parameterType });
                            }
                            return inParameters;
                        };

                        return directiveApi;
                    }
                };

                $scope.scopeModel.addMethodParameter = function () {
                    $scope.scopeModel.parameters.push({});
                };

                $scope.scopeModel.deleteParameter = function (parameter) {

                    var index = $scope.scopeModel.parameters.indexOf(parameter);
                    if (index > -1) {
                        $scope.scopeModel.parameters.splice(index, 1);
                    }
                };
            }

        }
        return directiveDefinitionObject;
    }]);
