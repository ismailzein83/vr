
'use strict';

app.directive('vrAnalyticDataanalysisitemdefinitionSettings', ['VR_Analytic_DataAnalysisDefinitionAPIService', 'UtilsService', 'VRUIUtilsService', function (VR_Analytic_DataAnalysisDefinitionAPIService, UtilsService, VRUIUtilsService) {
    return {
        restrict: 'E',
        scope: {
            onReady: '=',
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var dataAnalysisItemDefinitionSettings = new DataAnalysisItemDefinitionSettings($scope, ctrl, $attrs);
            dataAnalysisItemDefinitionSettings.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        templateUrl: '/Client/Modules/VR_Notification/Directives/VRAlertRule/Templates/VRAlertRuleCriteriaTemplate.html'
    };

    function DataAnalysisItemDefinitionSettings($scope, ctrl, $attrs) {
        this.initializeController = initializeController;

        var directiveAPI;
        var directiveReadyDeferred = UtilsService.createPromiseDeferred();

        function initializeController() {

            $scope.scopeModel = {};

            $scope.scopeModel.onDirectiveReady = function (api) {
                directiveAPI = api;
                directiveReadyDeferred.resolve();
            };

            defineAPI();
        }
        function defineAPI() {
            var api = {};

            api.load = function (payload) {

                var promises = [];
                var dataAnalysisItemDefinition;
                var dataAnalysisDefinitionId;
                var itemDefinitionTypeId;
                var context;

                if (payload != undefined) {
                    dataAnalysisItemDefinition = payload.dataAnalysisItemDefinition;
                    dataAnalysisDefinitionId = payload.dataAnalysisDefinitionId;
                    itemDefinitionTypeId = payload.itemDefinitionTypeId;
                    context = payload.context;
                }

                var getDataAnalysisDefinitionSettingsPromise = getDataAnalysisDefinitionSettings();
                promises.push(getDataAnalysisDefinitionSettingsPromise);

                var directiveLoadDeferred = UtilsService.createPromiseDeferred();
                promises.push(directiveLoadDeferred.promise);

                UtilsService.waitMultiplePromises([getDataAnalysisDefinitionSettingsPromise, directiveReadyDeferred.promise]).then(function () {

                    var directivePayload = {};
                    directivePayload.context = buildContext();
                    if (dataAnalysisItemDefinition != undefined)
                        directivePayload.dataAnalysisItemDefinitionSettings = dataAnalysisItemDefinition.Settings;

                    VRUIUtilsService.callDirectiveLoad(directiveAPI, directivePayload, directiveLoadDeferred);
                });

                function getDataAnalysisDefinitionSettings() {

                    return VR_Analytic_DataAnalysisDefinitionAPIService.GetDataAnalysisDefinition(dataAnalysisDefinitionId).then(function (response) {

                        var dataAnalysisDefinitionEntity = response;
                        var itemsConfig = dataAnalysisDefinitionEntity.Settings.ItemsConfig;

                        for (var i = 0; i < itemsConfig.length; i++)
                            if (itemsConfig[i].TypeId == itemDefinitionTypeId) {
                                $scope.scopeModel.directiveEditor = itemsConfig[i].Editor;
                            }
                    });
                }
                function buildContext() {
                    return context;
                }

                return UtilsService.waitMultiplePromises(promises);
            };

            api.getData = function () {
                return directiveAPI.getData();
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }
}]);













//(function (app) {

//    'use strict';

//    VRAlertRuleTypeSettingsDirective.$inject = ['VR_Notification_VRAlertRuleAPIService', 'UtilsService', 'VRUIUtilsService'];

//    function VRAlertRuleTypeSettingsDirective(VR_Notification_VRAlertRuleAPIService, UtilsService, VRUIUtilsService) {
//        return {
//            restrict: "E",
//            scope: {
//                onReady: "=",
//                normalColNum: '@',
//                label: '@',
//                customvalidate: '='
//            },
//            controller: function ($scope, $element, $attrs) {
//                var ctrl = this;
//                var criteria = new VRAlertRuleTypeSettings($scope, ctrl, $attrs);
//                criteria.initializeController();
//            },
//            controllerAs: "ctrl",
//            bindToController: true,
//            templateUrl: '/Client/Modules/VR_Notification/Directives/VRAlertRule/Templates/VRAlertRuleCriteriaTemplate.html'
//        };

//        function VRAlertRuleTypeSettings($scope, ctrl, $attrs) {
//            this.initializeController = initializeController;

//            var selectorAPI;
//            var context;

//            var directiveAPI;
//            var directiveReadyDeferred;

//            function initializeController() {
//                $scope.scopeModel = {};
//                $scope.scopeModel.templateConfigs = [];
//                $scope.scopeModel.selectedTemplateConfig;

//                $scope.scopeModel.onSelectorReady = function (api) {
//                    selectorAPI = api;
//                    defineAPI();
//                };

//                $scope.scopeModel.onDirectiveReady = function (api) {
//                    directiveAPI = api;

//                    var directivePayload = { context: builContext() };
//                    var setLoader = function (value) {
//                        $scope.scopeModel.isLoadingDirective = value;
//                    };
//                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveAPI, directivePayload, setLoader, directiveReadyDeferred);
//                };
//            }
//            function defineAPI() {
//                var api = {};

//                api.load = function (payload) {
//                    selectorAPI.clearDataSource();

//                    var promises = [];
//                    var criteria;

//                    if (payload != undefined) {
//                        context = payload.context;
//                        criteria = payload.criteria;
//                    }

//                    var getVRAlertRuleCriteriaTemplateConfigsPromise = getVRAlertRuleCriteriaTemplateConfigs();
//                    promises.push(getVRAlertRuleCriteriaTemplateConfigsPromise);

//                    if (criteria != undefined) {
//                        var loadDirectivePromise = loadDirective();
//                        promises.push(loadDirectivePromise);
//                    }


//                    function getVRAlertRuleCriteriaTemplateConfigs() {
//                        return VR_Notification_VRAlertRuleAPIService.GetVRAlertRuleCriteriaExtensionConfigs().then(function (response) {

//                            if (response != null) {
//                                for (var i = 0; i < response.length; i++) {
//                                    $scope.scopeModel.templateConfigs.push(response[i]);
//                                }
//                                if (criteria != undefined) {
//                                    $scope.scopeModel.selectedTemplateConfig =
//                                        UtilsService.getItemByVal($scope.scopeModel.templateConfigs, criteria.ConfigId, 'ExtensionConfigurationId');
//                                }
//                            }
//                        });
//                    }
//                    function loadDirective() {
//                        directiveReadyDeferred = UtilsService.createPromiseDeferred();

//                        var directiveLoadDeferred = UtilsService.createPromiseDeferred();

//                        directiveReadyDeferred.promise.then(function () {
//                            directiveReadyDeferred = undefined;

//                            var directivePayload = {
//                                context: builContext(),
//                                criteria: criteria
//                            };
//                            VRUIUtilsService.callDirectiveLoad(directiveAPI, directivePayload, directiveLoadDeferred);
//                        });

//                        return directiveLoadDeferred.promise;
//                    }

//                    return UtilsService.waitMultiplePromises(promises);
//                };

//                api.getData = function () {
//                    var data;
//                    if ($scope.scopeModel.selectedTemplateConfig != undefined && directiveAPI != undefined) {

//                        data = directiveAPI.getData();
//                        if (data != undefined) {
//                            data.ConfigId = $scope.scopeModel.selectedTemplateConfig.ExtensionConfigurationId;
//                        }
//                    }
//                    return data;
//                };

//                if (ctrl.onReady != null) {
//                    ctrl.onReady(api);
//                }
//            }

//            function builContext() {
//                return context;
//            }
//        }
//    }

//    app.directive('vrNotificationVralertruleCriteria', VRAlertRuleTypeSettingsDirective);

//})(app);