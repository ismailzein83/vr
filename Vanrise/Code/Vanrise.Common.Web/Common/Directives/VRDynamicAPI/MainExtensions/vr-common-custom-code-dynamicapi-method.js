"use strict";
app.directive("vrCommonCustomCodeDynamicapiMethod", ["UtilsService", "VRNotificationService", "VR_Dynamic_APIService", "VRCommon_VRDynamicAPIAPIService", "VRUIUtilsService", "VRCommon_ObjectTrackingService",
    function (UtilsService, VRNotificationService, VR_Dynamic_APIService, VRCommon_VRDynamicAPIAPIService, VRUIUtilsService, VRCommon_ObjectTrackingService) {

        var directiveDefinitionObject = {
            restrict: "E",
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var customCodeMethodsGrid = new CustomCodeMethodsGrid($scope, ctrl, $attrs);
                customCodeMethodsGrid.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: "/Client/Modules/Common/Directives/VRDynamicAPI/MainExtensions/Templates/CustomCodeDynamicAPIMethod.html"
        };


        function CustomCodeMethodsGrid($scope, ctrl) {

            var methodParametersGridApi;
            var methodParametersReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var methodTypeDirectiveApi;
            var methodTypeReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.isReturnTypeRequired = function () {
                    if (methodTypeDirectiveApi != undefined && methodTypeDirectiveApi.getSelectedIds() != undefined) {
                        if ($scope.scopeModel.returnType == undefined && methodTypeDirectiveApi.getSelectedIds() == 1)
                            return true;
                    }
                    return false;
                };

                $scope.scopeModel.onCustomCodeMethodParametersGridDirectiveReady = function (api) {

                    methodParametersGridApi = api;
                    methodParametersReadyPromiseDeferred.resolve();

                };

                $scope.scopeModel.onCustomCodeMethodTypeDirectiveReady = function (api) {

                    methodTypeDirectiveApi = api;
                    methodTypeReadyPromiseDeferred.resolve();
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function") {
                    ctrl.onReady(defineAPI());

                }


                function defineAPI() {

                    var api = {};

                    api.load = function (payload) {

                        if (payload != undefined && payload.vrDynamicAPIMethodSettingsEntity != undefined) {
                            $scope.scopeModel.isLoading = true;
                            $scope.scopeModel.returnType = payload.vrDynamicAPIMethodSettingsEntity.ReturnType;
                            $scope.scopeModel.body = payload.vrDynamicAPIMethodSettingsEntity.MethodBody;
                        }
                        function loadMethodTypeDirective() {

                            var promises = [];
                            var methodTypeDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

                            promises.push(methodTypeReadyPromiseDeferred.promise);
                            UtilsService.waitMultiplePromises(promises).then(function (response) {
                                var methodTypePayload;
                                if (payload != undefined && payload.vrDynamicAPIMethodSettingsEntity != undefined) {
                                    methodTypePayload = {
                                        selectedIds: payload.vrDynamicAPIMethodSettingsEntity.MethodType
                                    };
                                }
                                VRUIUtilsService.callDirectiveLoad(methodTypeDirectiveApi, methodTypePayload, methodTypeDirectiveLoadDeferred);
                            });
                            return methodTypeDirectiveLoadDeferred.promise;
                        }
                        function loadMethodParametersDirective() {
                            var methodParametersDirectiveLoadDeferred = UtilsService.createPromiseDeferred();
                            methodParametersReadyPromiseDeferred.promise.then(function (response) {
                                var methodParametersPayload = {
                                    context: getContext()
                                };
                                if (payload != undefined && payload.vrDynamicAPIMethodSettingsEntity != undefined) {
                                    methodParametersPayload.InParameters=payload.vrDynamicAPIMethodSettingsEntity.InParameters;
                                }
                                VRUIUtilsService.callDirectiveLoad(methodParametersGridApi, methodParametersPayload, methodParametersDirectiveLoadDeferred);
                            });
                            return methodParametersDirectiveLoadDeferred.promise;
                        }

                        var promises = [];

                        promises.push(loadMethodTypeDirective());
                        promises.push(loadMethodParametersDirective());

                        return UtilsService.waitMultiplePromises(promises).then(function (response) {
                            $scope.scopeModel.isLoading = false;
                        });
                    };

                    api.getData = function () {

                        return {
                            $type: "Vanrise.Common.MainExtensions.VRDynamicAPI.VRCustomCodeDynamicAPIMethod,Vanrise.Common.MainExtensions",
                            MethodBody: $scope.scopeModel.body,
                            MethodType: methodTypeDirectiveApi.getSelectedIds(),
                            InParameters: methodParametersGridApi.getData(),
                            ReturnType: $scope.scopeModel.returnType
                        };
                    };
                    if (ctrl.onReady != null)
                        ctrl.onReady(api);
                    return api;
                }

                function getContext() {
                    return {
                        getMethodType: function () {
                            return methodTypeDirectiveApi != undefined ? methodTypeDirectiveApi.getSelectedIds() : undefined;
                        }
                    };
                }

            }
        }

        return directiveDefinitionObject;
    }]);
