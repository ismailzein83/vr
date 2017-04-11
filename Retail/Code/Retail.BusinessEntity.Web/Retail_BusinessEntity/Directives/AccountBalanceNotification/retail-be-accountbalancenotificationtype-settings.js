(function (app) {

    'use strict';

    AccountBalanceNotificationTypeSettingsDirective.$inject = ['UtilsService', 'VRUIUtilsService'];

    function AccountBalanceNotificationTypeSettingsDirective(UtilsService, VRUIUtilsService) {
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
                var ctor = new AccountBalanceNotificationTypeSettingsCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Retail_BusinessEntity/Directives/AccountBalanceNotification/Templates/AccountBalanceNotificationTypeSettingsTemplate.html"
        };

        function AccountBalanceNotificationTypeSettingsCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var beDefinitionSelectorAPI;
            var beDefinitionSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onBusinessEntityDefinitionSelectorReady = function (api) {
                    beDefinitionSelectorAPI = api;
                    beDefinitionSelectorPromiseDeferred.resolve();
                };

                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    var accountBEDefinitionId;

                    if (payload != undefined) {
                        var accountBalanceNotificationTypeExtendedSettings = payload.accountBalanceNotificationTypeExtendedSettings;

                        if (accountBalanceNotificationTypeExtendedSettings != undefined) {
                            accountBEDefinitionId = accountBalanceNotificationTypeExtendedSettings.AccountBEDefinitionId;
                        }
                    }

                    //Loading BusinessEntityDefinition Selector
                    var businessEntityDefinitionSelectorLoadPromise = getBusinessEntityDefinitionSelectorLoadPromise();
                    promises.push(businessEntityDefinitionSelectorLoadPromise);

                    function getBusinessEntityDefinitionSelectorLoadPromise() {
                        var businessEntityDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                        beDefinitionSelectorPromiseDeferred.promise.then(function () {

                            var selectorPayload = {
                                filter: {
                                    Filters: [{
                                        $type: "Retail.BusinessEntity.Business.AccountBEDefinitionFilter, Retail.BusinessEntity.Business"
                                    }]
                                },
                                selectedIds: accountBEDefinitionId
                            };
                            VRUIUtilsService.callDirectiveLoad(beDefinitionSelectorAPI, selectorPayload, businessEntityDefinitionSelectorLoadDeferred);
                        });

                        return businessEntityDefinitionSelectorLoadDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);

                };

                api.getData = function () {
                    return {
                        $type: "Retail.BusinessEntity.Business.RetailAccountBalanceNotificationTypeSettings, Retail.BusinessEntity.Business",
                        AccountBEDefinitionId: beDefinitionSelectorAPI.getSelectedIds()
                    };
                };

                if (ctrl.onReady != null) {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('retailBeAccountbalancenotificationtypeSettings', AccountBalanceNotificationTypeSettingsDirective);

})(app);