'use strict';

app.directive('businessprocessVrWorkflowVariabletypes', ['UtilsService', 'VRUIUtilsService', 'BusinessProcess_VRWorkflowAPIService',
    function (UtilsService, VRUIUtilsService, BusinessProcess_VRWorkflowAPIService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                selectedvalues: '=',
                onselectionchanged: '=',
                onselectitem: '=',
                ondeselectitem: '=',
                isrequired: '=',
                normalColNum: '@'
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;
                ctrl.datasource = [];
                ctrl.selectedvalues;

                var ctor = new VariableTypeCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: "/Client/Modules/BusinessProcess/Directives/VRWorkflow/Templates/VRWorkflowVariableTypesTemplate.html"
        };

        function VariableTypeCtor(ctrl, $scope, attrs) {
            this.initializeController = initializeController;

            var selectorAPI;

            var directiveAPI;
            var directiveReadyDeferred;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.templateConfigs = [];
                $scope.scopeModel.selectedTemplateConfig;
                $scope.scopeModel.showVariableTypeSelector = true;

                $scope.scopeModel.onSelectorReady = function (api) {
                    selectorAPI = api;
                    defineAPI();
                };

                $scope.scopeModel.onDirectiveReady = function (api) {
                    directiveAPI = api;
                    var setLoader = function (value) {
                        $scope.scopeModel.isLoadingDirective = value;
                    };
                    var directivePayload;
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveAPI, directivePayload, setLoader, directiveReadyDeferred);
                };
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    selectorAPI.clearDataSource();

                    var selectIfSingleItem;
                    var variableType;

                    if (payload != undefined) {
                        selectIfSingleItem = payload.selectIfSingleItem;
                        variableType = payload.variableType != undefined ? payload.variableType : undefined;
                    }

                    var promises = [];

                    var variableTypeExtensionConfigsLoadPromise = getVariableTypeExtensionConfigs();
                    promises.push(variableTypeExtensionConfigsLoadPromise);

                    if (variableType != undefined) {
                        var loadDirectivePromise = loadDirective();
                        promises.push(loadDirectivePromise);
                    }

                    function getVariableTypeExtensionConfigs() {
                        return BusinessProcess_VRWorkflowAPIService.GetVRWorkflowVariableTypeExtensionConfigs().then(function (response) {
                            if (response != null) {
                                for (var i = 0; i < response.length; i++) {
                                    $scope.scopeModel.templateConfigs.push(response[i]);
                                }

                                if (variableType != undefined) {
                                    $scope.scopeModel.selectedTemplateConfig = UtilsService.getItemByVal($scope.scopeModel.templateConfigs, variableType.ConfigId, 'ExtensionConfigurationId');
                                }
                                else if (selectIfSingleItem == true) {
                                    selectorAPI.selectIfSingleItem();
                                }
                                if ($scope.scopeModel.selectedTemplateConfig == undefined) {
                                    $scope.scopeModel.selectedTemplateConfig = UtilsService.getItemByVal($scope.scopeModel.templateConfigs, 'VRWorkflow_ArgumentVariableType_GenericVariableType', 'Name');
                                }

                                if (selectIfSingleItem == true && $scope.scopeModel.templateConfigs.length == 1) {
                                    $scope.scopeModel.showVariableTypeSelector = false;
                                }
                            }
                        });
                    }

                    function loadDirective() {
                        directiveReadyDeferred = UtilsService.createPromiseDeferred();

                        var directiveLoadDeferred = UtilsService.createPromiseDeferred();

                        directiveReadyDeferred.promise.then(function () {
                            directiveReadyDeferred = undefined;

                            var directivePayload;
                            if (payload != undefined) {
                                directivePayload = payload.variableType;
                            }
                            VRUIUtilsService.callDirectiveLoad(directiveAPI, directivePayload, directiveLoadDeferred);
                        });

                        return directiveLoadDeferred.promise;
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
    }]);