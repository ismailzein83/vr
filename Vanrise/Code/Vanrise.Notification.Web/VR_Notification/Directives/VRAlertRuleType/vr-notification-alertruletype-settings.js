(function (app) {

    'use strict';

    VRAlertRuleTypeSettingsDirective.$inject = ['VR_Notification_VRAlertRuleTypeAPIService', 'UtilsService', 'VRUIUtilsService'];

    function VRAlertRuleTypeSettingsDirective(VR_Notification_VRAlertRuleTypeAPIService, UtilsService, VRUIUtilsService) {
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
                var vrAlertRuleTypeSettings = new VRAlertRuleTypeSettings($scope, ctrl, $attrs);
                vrAlertRuleTypeSettings.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: '/Client/Modules/VR_Notification/Directives/VRAlertRuleType/Templates/VRAlertRuleTypeSettingsTemplate.html'
        };

        function VRAlertRuleTypeSettings($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var selectorAPI;

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
                    var setLoader = function (value) {
                        $scope.scopeModel.isLoadingDirective = value;
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveAPI, undefined, setLoader, directiveReadyDeferred);
                };
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    selectorAPI.clearDataSource();

                    var promises = [];
                    var vrAlertRuleTypeSettings;

                    if (payload != undefined) {
                        vrAlertRuleTypeSettings = payload.vrAlertRuleTypeSettings;
                    }

                    var getVRAlertRuleTypeSettingsTemplateConfigsPromise = getVRAlertRuleTypeSettingsTemplateConfigs();
                    promises.push(getVRAlertRuleTypeSettingsTemplateConfigsPromise);

                    if (vrAlertRuleTypeSettings != undefined) {
                        var loadDirectivePromise = loadDirective();
                        promises.push(loadDirectivePromise);
                    }


                    function getVRAlertRuleTypeSettingsTemplateConfigs() {
                        return VR_Notification_VRAlertRuleTypeAPIService.GetVRAlertRuleTypeSettingsExtensionConfigs().then(function (response) {
                            if (response != null) {
                                for (var i = 0; i < response.length; i++) {
                                    $scope.scopeModel.templateConfigs.push(response[i]);
                                }
                                if (vrAlertRuleTypeSettings != undefined) {
                                    $scope.scopeModel.selectedTemplateConfig =
                                        UtilsService.getItemByVal($scope.scopeModel.templateConfigs, vrAlertRuleTypeSettings.ConfigId, 'ExtensionConfigurationId');
                                }
                            }
                        });
                    }
                    function loadDirective() {
                        directiveReadyDeferred = UtilsService.createPromiseDeferred();

                        var directiveLoadDeferred = UtilsService.createPromiseDeferred();

                        directiveReadyDeferred.promise.then(function () {
                            directiveReadyDeferred = undefined;
                            var directivePayload = { vrAlertRuleTypeSettings: vrAlertRuleTypeSettings };

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
        }
    }

    app.directive('vrNotificationAlertruletypeSettings', VRAlertRuleTypeSettingsDirective);

})(app);