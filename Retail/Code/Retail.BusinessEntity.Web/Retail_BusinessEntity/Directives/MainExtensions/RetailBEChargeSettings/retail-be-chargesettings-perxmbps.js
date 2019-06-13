"use strict";
app.directive("retailBeChargesettingsPerxmbps", ["UtilsService", "VRNotificationService", "VRUIUtilsService",
    function (UtilsService, VRNotificationService, VRUIUtilsService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new RetailBEChargeSettings($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/Retail_BusinessEntity/Directives/MainExtensions/RetailBEChargeSettings/Templates/PerXMBPSChargeSettingsTemplate.html"

        };

        function RetailBEChargeSettings($scope, ctrl, $attrs) {
            this.initializeController = initializeController;


            function initializeController() {
                $scope.scopeModel = {};
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    if (payload != undefined && payload.retailBEChargeSettings != undefined) {
                        $scope.scopeModel.chargeValue = payload.retailBEChargeSettings.ChargeValue;
                        $scope.scopeModel.mbps = payload.retailBEChargeSettings.MBPS;
                    }
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "Retail.BusinessEntity.MainExtensions.RetailBECharge.RetailBEPerXMBPS,Retail.BusinessEntity.MainExtensions",
                        ChargeValue: $scope.scopeModel.ChargeValue,
                        MBPS: $scope.scopeModel.mbps
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;

    }
]);