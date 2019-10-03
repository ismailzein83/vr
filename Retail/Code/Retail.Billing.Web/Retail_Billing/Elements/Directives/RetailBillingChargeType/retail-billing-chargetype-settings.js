"use strict";

app.directive("retailBillingChargetypeSettings", ["UtilsService", "VRUIUtilsService",
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new BillingChargeTypeSettings($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/Retail_Billing/Elements/Directives/RetailBillingChargeType/Templates/BillingChargeTypeSettingsTemplate.html"
        };

        function BillingChargeTypeSettings($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var chargeTypeExtendedSettingsDirectiveAPI;
            var chargeTypeExtendedettingsDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();
            var context;

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onChargeTypeEsxtendedSettingsDirectiveReady = function (api) {
                    chargeTypeExtendedSettingsDirectiveAPI = api;
                    chargeTypeExtendedettingsDirectiveReadyPromiseDeferred.resolve();
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var chargeTypeEntity;

                    if (payload != undefined) {
                        context = payload.context;
                        chargeTypeEntity = payload.componentType;
                        $scope.scopeModel.name = chargeTypeEntity != undefined ? chargeTypeEntity.Name : undefined;
                    }

                    function loadChargeTypeEsxtendedSettingsDirective() {
                        var chargeTypeExtendedettingsDirectiveLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                        chargeTypeExtendedettingsDirectiveReadyPromiseDeferred.promise.then(function () {
                            var directivePayload = {
                                extendedSettings: chargeTypeEntity != undefined && chargeTypeEntity.Settings != undefined ? chargeTypeEntity.Settings.ExtendedSettings : undefined,
                                context: getContext()

                            };
                            VRUIUtilsService.callDirectiveLoad(chargeTypeExtendedSettingsDirectiveAPI, directivePayload, chargeTypeExtendedettingsDirectiveLoadPromiseDeferred);
                        });
                        return chargeTypeExtendedettingsDirectiveLoadPromiseDeferred.promise;
                    }

                    return UtilsService.waitPromiseNode({ promises: [loadChargeTypeEsxtendedSettingsDirective()] });
                };

                api.getData = function () {

                    var obj = {
                        $type: "Retail.Billing.Entities.RetailBillingChargeTypeSettings,Retail.Billing.Entities",
                        ExtendedSettings: chargeTypeExtendedSettingsDirectiveAPI.getData()
                    };

                    return {
                        Name: $scope.scopeModel.name,
                        Settings: obj
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function getContext() {

                var currentContext = context;
                if (currentContext == undefined)
                    currentContext = {};

                return currentContext;
            }

        }
        return directiveDefinitionObject;
    }
]);