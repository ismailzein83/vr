"use strict";
app.directive("retailBeChargesettingsPere1", ["UtilsService", "VRNotificationService", "VRUIUtilsService",
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
            templateUrl: "/Client/Modules/Retail_BusinessEntity/Directives/MainExtensions/RetailBEChargeSettings/Templates/PerE1ChargeSettingsTemplate.html"

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
                    }
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "Retail.BusinessEntity.MainExtensions.RetailBECharge.RetailBEPerE1,Retail.BusinessEntity.MainExtensions",
                        ChargeValue: $scope.scopeModel.chargeValue
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;

    }
]);