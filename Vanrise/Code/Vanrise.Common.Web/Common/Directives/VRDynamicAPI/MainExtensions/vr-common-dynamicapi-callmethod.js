"use strict";
app.directive("vrCommonDynamicapiCallmethod", ["UtilsService", "VRUIUtilsService","VRCommon_CustomCodeDynamicAPIMethodTypeEnum",
    function (UtilsService, VRUIUtilsService, VRCommon_CustomCodeDynamicAPIMethodTypeEnum) {

        var directiveDefinitionObject = {
            restrict: "E",
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new CallMethodCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: "/Client/Modules/Common/Directives/VRDynamicAPI/MainExtensions/Templates/CallMethodDynamicAPITemplate.html"
        };


        function CallMethodCtor($scope, ctrl) {

            var namespaceSelectorAPI;
            var namespaceSelectorReadyDeferred = UtilsService.createPromiseDeferred();
            var namespaceSelectedPromiseDeferred;

            var classSelectorAPI;
            var classSelectorReadyDeferred = UtilsService.createPromiseDeferred();
            var classSelectedPromiseDeferred;

            var methodSelectorAPI;
            var methodSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var vrDynamicAPIMethodSettingsEntity;
            var vrNamespaceId;


            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.methodTypes = UtilsService.getArrayEnum(VRCommon_CustomCodeDynamicAPIMethodTypeEnum);

                $scope.scopeModel.onNamespaceSelectorReady = function (api) {
                    namespaceSelectorAPI = api;
                    namespaceSelectorReadyDeferred.resolve();
                };

                $scope.scopeModel.onClassSelectorReady = function (api) {
                    classSelectorAPI = api;
                    classSelectorReadyDeferred.resolve();
                };

                $scope.scopeModel.onMethodSelectorReady = function (api) {
                    methodSelectorAPI = api;
                    methodSelectorReadyDeferred.resolve();
                };


                $scope.scopeModel.onNamespaceSelectionChange = function (vrNamespace) {
                    if (vrNamespace != undefined) {
                        vrNamespaceId = vrNamespace.VRNamespaceId;
                        var setLoader = function (value) {
                            $scope.scopeModel.isLoading = value;
                        };
                        classSelectorReadyDeferred.promise.then(function () {
                             
                            var classSelectorPayload = {
                                vrNamespaceId: vrNamespaceId
                            };
                            VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, classSelectorAPI, classSelectorPayload, setLoader, namespaceSelectedPromiseDeferred);
                        });
                        methodSelectorReadyDeferred.promise.then(function () {
                            methodSelectorAPI.clearDataSource();
                        });
                    }
                };

             
                $scope.scopeModel.onClassSelectionChange = function (vrClass) {
                    if (vrClass != undefined) {
                        methodSelectorReadyDeferred.promise.then(function () {
                            var setLoader = function (value) {
                                $scope.scopeModel.isLoading = value;
                            };
                            var payload = {
                                vrNamespaceId: vrNamespaceId,
                                className: vrClass.Name
                            };
                            VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, methodSelectorAPI, payload, setLoader, classSelectedPromiseDeferred);
                        });
                    }
                };
              
              

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function") {
                    ctrl.onReady(defineAPI());

                }

                function defineAPI() {

                    var api = {};

                    api.load = function (payload) {
                        var promises = [];
                        vrDynamicAPIMethodSettingsEntity = undefined;
                        $scope.scopeModel.isLoading = true;

                        if (payload != undefined && payload.vrDynamicAPIMethodSettingsEntity != undefined) {

                            $scope.scopeModel.selectedMethodType = UtilsService.getItemByVal($scope.scopeModel.methodTypes, payload.vrDynamicAPIMethodSettingsEntity.MethodType, 'value');
                            namespaceSelectedPromiseDeferred = UtilsService.createPromiseDeferred();
                            classSelectedPromiseDeferred = UtilsService.createPromiseDeferred();
                            vrDynamicAPIMethodSettingsEntity = payload.vrDynamicAPIMethodSettingsEntity;

                            promises.push(loadClassSelector());
                            promises.push(loadMethodSelector());

                        }
                        function loadClassSelector() {
                            var classSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                            UtilsService.waitMultiplePromises([classSelectorReadyDeferred.promise, namespaceSelectedPromiseDeferred.promise]).then(function () {
                                var classSelectorPayload = {
                                    selectedIds: vrDynamicAPIMethodSettingsEntity != undefined ? vrDynamicAPIMethodSettingsEntity.ClassName : undefined,
                                    vrNamespaceId: vrDynamicAPIMethodSettingsEntity != undefined ? vrDynamicAPIMethodSettingsEntity.NamespaceId : undefined
                                };
                                VRUIUtilsService.callDirectiveLoad(classSelectorAPI, classSelectorPayload, classSelectorLoadDeferred);
                            });
                            return classSelectorLoadDeferred.promise;
                        }

                        function loadMethodSelector() {
                            var methodSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                            UtilsService.waitMultiplePromises([methodSelectorReadyDeferred.promise, classSelectedPromiseDeferred.promise]).then(function () {
                                var methodSelectorPayload = {
                                    selectedIds: vrDynamicAPIMethodSettingsEntity != undefined ? vrDynamicAPIMethodSettingsEntity.MethodName : undefined,
                                    vrNamespaceId: vrDynamicAPIMethodSettingsEntity != undefined ? vrDynamicAPIMethodSettingsEntity.NamespaceId : undefined,
                                    className: vrDynamicAPIMethodSettingsEntity != undefined ? vrDynamicAPIMethodSettingsEntity.ClassName : undefined,
                                };
                                VRUIUtilsService.callDirectiveLoad(methodSelectorAPI, methodSelectorPayload, methodSelectorLoadDeferred);
                            });
                            return methodSelectorLoadDeferred.promise;
                        }

                        function loadNamespaceSelector() {
                            var namespaceSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                            namespaceSelectorReadyDeferred.promise.then(function () {
                                var namespacePayload = {
                                    selectedIds: vrDynamicAPIMethodSettingsEntity != undefined ? vrDynamicAPIMethodSettingsEntity.NamespaceId : undefined
                                };
                                VRUIUtilsService.callDirectiveLoad(namespaceSelectorAPI, namespacePayload, namespaceSelectorLoadDeferred);
                            });
                            return namespaceSelectorLoadDeferred.promise;
                        }

                        promises.push(loadNamespaceSelector());

                        return UtilsService.waitMultiplePromises(promises).then(function (response) {
                            namespaceSelectedPromiseDeferred = undefined;
                            classSelectedPromiseDeferred = undefined;
                            vrDynamicAPIMethodSettingsEntity = undefined;
                        }).finally(function () {
                            $scope.scopeModel.isLoading = false;
                        });
                    };

                    api.getData = function () {
                        return {
                            $type: "Vanrise.Common.MainExtensions.VRDynamicAPI.VRCallMethodDynamicAPI,Vanrise.Common.MainExtensions",
                            NamespaceId: namespaceSelectorAPI.getSelectedIds(),
                            ClassName: classSelectorAPI.getSelectedIds(),
                            MethodName: methodSelectorAPI.getSelectedIds(),
                            MethodType: $scope.scopeModel.selectedMethodType.value
                        };
                    };
                    if (ctrl.onReady != null)
                        ctrl.onReady(api);
                    return api;
                }

            }
        }

        return directiveDefinitionObject;
    }]);
