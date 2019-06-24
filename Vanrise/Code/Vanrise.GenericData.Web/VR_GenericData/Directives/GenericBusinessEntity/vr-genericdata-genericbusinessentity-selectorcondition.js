"use strict";
app.directive("vrGenericdataGenericbusinessentitySelectorcondition", ["UtilsService", "VRUIUtilsService", 'VR_GenericData_GenericBusinessEntityAPIService',
    function (UtilsService, VRUIUtilsService, VR_GenericData_GenericBusinessEntityAPIService) {

        var directiveDefinitionObject = {
            restrict: "E",
            scope: {
                onReady: "=",
                isrequired: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new GenericBusinessEntitySelectorFilterCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/VR_GenericData/Directives/GenericBusinessEntity/Templates/GenericBusinessEntitySelectorCondition.html"
        };

        function GenericBusinessEntitySelectorFilterCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var beDefinitionId;

            var selectorAPI;

            var directiveAPI;
            var directiveReadyDeferred;

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.templateConfigs = [];

                $scope.scopeModel.onSelectorReady = function (api) {
                    selectorAPI = api;
                    defineAPI();
                };

                $scope.scopeModel.onDirectiveReady = function (api) {
                    directiveAPI = api;
                    var setLoader = function (value) {
                        $scope.scopeModel.isLoadingDirective = value;
                    };
                    var directivePayload = {
                        beDefinitionId: beDefinitionId
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveAPI, directivePayload, setLoader, directiveReadyDeferred);
                };
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    selectorAPI.clearDataSource();

                    var promises = [];
                    var genericBESelectorCondition;

                    if (payload != undefined) {
                        beDefinitionId = payload.beDefinitionId;
                        genericBESelectorCondition = payload.genericBESelectorCondition;
                    }

                    var loadDirectivePromiseDeferred = undefined;
                    if (genericBESelectorCondition != undefined) {
                        directiveReadyDeferred = UtilsService.createPromiseDeferred();
                        loadDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                        promises.push(loadDirectivePromiseDeferred.promise);
                    }

                    var getGenericBESelectorConditionConfigsPromise = GetGenericBESelectorConditionConfigs();
                    promises.push(getGenericBESelectorConditionConfigsPromise);

                    getGenericBESelectorConditionConfigsPromise.then(function () {
                        if (genericBESelectorCondition != undefined) {
                            var loadDirectivePromise = loadDirective();
                            loadDirectivePromise.then(function () { loadDirectivePromiseDeferred.resolve(); });
                        }
                    });

                    function GetGenericBESelectorConditionConfigs() {
                        return VR_GenericData_GenericBusinessEntityAPIService.GetGenericBESelectorConditionConfigs().then(function (response) {
                            if (response != undefined) {
                                for (var i = 0; i < response.length; i++)
                                    $scope.scopeModel.templateConfigs.push(response[i]);
                            }

                            if (genericBESelectorCondition != undefined)
                                $scope.scopeModel.selectedTemplateConfig = UtilsService.getItemByVal($scope.scopeModel.templateConfigs, genericBESelectorCondition.ConfigId, 'ExtensionConfigurationId');
                        });
                    }

                    function loadDirective() {
                        var loadDirectiveDeferred = UtilsService.createPromiseDeferred();
                        directiveReadyDeferred.promise.then(function () {
                            directiveReadyDeferred = undefined;
                            var directivePayload = {
                                beDefinitionId: beDefinitionId,
                                genericBESelectorCondition: genericBESelectorCondition
                            };

                            VRUIUtilsService.callDirectiveLoad(directiveAPI, directivePayload, loadDirectiveDeferred);
                        });

                        return loadDirectiveDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var data;
                    if ($scope.scopeModel.selectedTemplateConfig != undefined && directiveAPI != undefined) {

                        data = directiveAPI.getData();
                        if (data != undefined)
                            data.ConfigId = $scope.scopeModel.selectedTemplateConfig.ExtensionConfigurationId;
                    }
                    return data;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
        return directiveDefinitionObject;
    }
]);