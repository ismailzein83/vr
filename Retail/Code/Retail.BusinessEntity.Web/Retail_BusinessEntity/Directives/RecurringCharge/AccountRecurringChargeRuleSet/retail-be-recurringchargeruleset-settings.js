(function (app) {

    'use strict';

    AccountRecurringChargeRuleSetSettingsDirective.$inject = ['Retail_BE_RecurringChargeAPIService', 'UtilsService', 'VRUIUtilsService'];

    function AccountRecurringChargeRuleSetSettingsDirective(Retail_BE_RecurringChargeAPIService, UtilsService, VRUIUtilsService) {
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
                var ctor = new AccountRecurringChargeRuleSetSettingsCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Retail_BusinessEntity/Directives/RecurringCharge/AccountRecurringChargeRuleSet/Templates/RecurringChargeRuleSetSettingsTemplate.html"
        };

        function AccountRecurringChargeRuleSetSettingsCtor($scope, ctrl, $attrs) {
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
                    var accountRecurringChargeRuleSetSettings;

                    if (payload != undefined) {
                        accountRecurringChargeRuleSetSettings = payload.accountRecurringChargeRuleSetSettings;
                    }

                    var accountRecurringChargeRuleSetSettingsTemplateConfigsLoadPromise = getAccountRecurringChargeRuleSetSettingsTemplateConfigs();
                    promises.push(accountRecurringChargeRuleSetSettingsTemplateConfigsLoadPromise);

                    if (accountRecurringChargeRuleSetSettings != undefined) {
                        var loadDirectivePromise = loadDirective();
                        promises.push(loadDirectivePromise);
                    }


                    function getAccountRecurringChargeRuleSetSettingsTemplateConfigs() {
                        return Retail_BE_RecurringChargeAPIService.GetAccountRecurringChargeRuleSetSettingsExtensionConfigs().then(function (response) {
                            if (response != undefined) {
                                for (var i = 0; i < response.length; i++) {
                                    $scope.scopeModel.templateConfigs.push(response[i]);
                                }
                                if (accountRecurringChargeRuleSetSettings != undefined) {
                                    $scope.scopeModel.selectedTemplateConfig =
                                        UtilsService.getItemByVal($scope.scopeModel.templateConfigs, accountRecurringChargeRuleSetSettings.ConfigId, 'ExtensionConfigurationId');
                                }
                            }
                        });
                    }
                    function loadDirective() {
                        directiveReadyDeferred = UtilsService.createPromiseDeferred();

                        var directiveLoadDeferred = UtilsService.createPromiseDeferred();

                        directiveReadyDeferred.promise.then(function () {
                            directiveReadyDeferred = undefined;
                            var directivePayload = { accountRecurringChargeRuleSetSettings: accountRecurringChargeRuleSetSettings };
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

    app.directive('retailBeRecurringchargerulesetSettings', AccountRecurringChargeRuleSetSettingsDirective);

})(app);