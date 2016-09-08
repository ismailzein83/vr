(function (app) {

    'use strict';

    VRAlertRuleTypeSettingsDirective.$inject = ['VR_Notification_VRAlertRuleAPIService', 'UtilsService', 'VRUIUtilsService'];

    function VRAlertRuleTypeSettingsDirective(VR_Notification_VRAlertRuleAPIService, UtilsService, VRUIUtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
                normalColNum: '@',
                label: '@',
                customvalidate: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var criteria = new VRAlertRuleTypeSettings($scope, ctrl, $attrs);
                criteria.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: '/Client/Modules/VR_Notification/Directives/VRAlertRule/Templates/VRAlertRuleCriteriaTemplate.html'
        };

        function VRAlertRuleTypeSettings($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var selectorAPI;
            var context;

            var directiveAPI;
            var directiveReadyDeferred;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.templateConfigs = [];
                $scope.scopeModel.selectedTemplateConfig;

                $scope.scopeModel.onSelectorReady = function (api) {
                    selectorAPI = api;
                    defineAPI();
                };

                $scope.scopeModel.onDirectiveReady = function (api) {
                    directiveAPI = api;

                    var directivePayload = { context: builContext() };
                    var setLoader = function (value) {
                        $scope.scopeModel.isLoadingDirective = value;
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveAPI, directivePayload, setLoader, directiveReadyDeferred);
                };
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    selectorAPI.clearDataSource();

                    var promises = [];
                    var criteria;

                    if (payload != undefined) {
                        context = payload.context;
                        criteria = payload.criteria;
                    }

                    var getVRAlertRuleCriteriaTemplateConfigsPromise = getVRAlertRuleCriteriaTemplateConfigs();
                    promises.push(getVRAlertRuleCriteriaTemplateConfigsPromise);

                    if (criteria != undefined) {
                        var loadDirectivePromise = loadDirective();
                        promises.push(loadDirectivePromise);
                    }


                    function getVRAlertRuleCriteriaTemplateConfigs() {
                        return VR_Notification_VRAlertRuleAPIService.GetVRAlertRuleCriteriaExtensionConfigs().then(function (response) {

                            if (response != null) {
                                for (var i = 0; i < response.length; i++) {
                                    $scope.scopeModel.templateConfigs.push(response[i]);
                                }
                                if (criteria != undefined) {
                                    $scope.scopeModel.selectedTemplateConfig =
                                        UtilsService.getItemByVal($scope.scopeModel.templateConfigs, criteria.ConfigId, 'ExtensionConfigurationId');
                                }
                            }
                        });
                    }
                    function loadDirective() {
                        directiveReadyDeferred = UtilsService.createPromiseDeferred();

                        var directiveLoadDeferred = UtilsService.createPromiseDeferred();

                        directiveReadyDeferred.promise.then(function () {
                            directiveReadyDeferred = undefined;

                            var directivePayload = {
                                context: builContext(),
                                criteria: criteria
                            };
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
                        if (data != undefined) {
                            data.ConfigId = $scope.scopeModel.selectedTemplateConfig.ExtensionConfigurationId;
                        }
                    }
                    return data;
                };

                if (ctrl.onReady != null) {
                    ctrl.onReady(api);
                }
            }

            function builContext() {
                return context;
            }
        }
    }

    app.directive('vrNotificationVralertruleCriteria', VRAlertRuleTypeSettingsDirective);

})(app);