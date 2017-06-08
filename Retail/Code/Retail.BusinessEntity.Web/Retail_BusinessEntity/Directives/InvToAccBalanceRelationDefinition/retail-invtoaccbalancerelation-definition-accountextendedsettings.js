"use strict";

app.directive("retailInvtoaccbalancerelationDefinitionAccountextendedsettings", ["UtilsService", "VRNotificationService", "VRUIUtilsService",
    function (UtilsService, VRNotificationService, VRUIUtilsService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new AccountInvToAccBalanceDefinitionExtendedSettings($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/Retail_BusinessEntity/Directives/InvToAccBalanceRelationDefinition/Templates/AccountInvToAccBalanceDefinitionExtendedSettings.html"

        };

        function AccountInvToAccBalanceDefinitionExtendedSettings($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var businessEntityDefinitionSelectorAPI;
            var businessEntityDefinitionSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onBusinessEntityDefinitionSelectorReady = function (api) {
                    businessEntityDefinitionSelectorAPI = api;
                    businessEntityDefinitionSelectorReadyDeferred.resolve();

                };

                UtilsService.waitMultiplePromises([businessEntityDefinitionSelectorReadyDeferred.promise]).then(function () {
                    defineAPI();
                });
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var extendedSettingsEntity;
                    if (payload != undefined) {
                        extendedSettingsEntity = payload.extendedSettingsEntity;
                    }
                    var promises = [];
                    function loadBusinessEntityDefinitionSelector() {
                        var businessEntityDefinitionPayload = {
                            filter: {
                                Filters: [{
                                    $type: "Retail.BusinessEntity.Business.AccountBEDefinitionFilter, Retail.BusinessEntity.Business"
                                }]
                            },
                            selectedIds: extendedSettingsEntity != undefined ? extendedSettingsEntity.AccountBEDefinitionId : undefined
                        };
                        return businessEntityDefinitionSelectorAPI.load(businessEntityDefinitionPayload);
                    }
                    promises.push(loadBusinessEntityDefinitionSelector());
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "Retail.BusinessEntity.Business.AccountInvToAccBalanceRelationDefinitionExtendedSettings ,Retail.BusinessEntity.Business",
                        AccountBEDefinitionId: businessEntityDefinitionSelectorAPI.getSelectedIds(),
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;

    }
]);