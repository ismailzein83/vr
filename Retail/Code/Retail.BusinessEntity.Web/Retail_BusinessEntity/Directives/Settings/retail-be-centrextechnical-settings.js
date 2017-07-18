"use strict";

app.directive("retailBeCentrextechnicalSettings", ["UtilsService", "VRNotificationService", "VRUIUtilsService",
    function (UtilsService, VRNotificationService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new CentrexTechnicalSettingsCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/Retail_BusinessEntity/Directives/Settings/Templates/CentrexTechnicalSettingsTemplate.html"
        };
        function CentrexTechnicalSettingsCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var beParentChildRelationDefinitionSelectorAPI;
            var beParentChildRelationDefinitionSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var retailCentrexImportCDRSettings;

                    if (payload != undefined && payload.data != undefined) {
                        retailCentrexImportCDRSettings = payload.data.RetailCentrexImportCDRSettings;
                    }

                    $scope.scopeModel.saleAmountPrecision = retailCentrexImportCDRSettings != undefined ? retailCentrexImportCDRSettings.SaleAmountPrecision : undefined;

                };

                api.getData = function () {

                    var obj = {
                        $type: "Retail.BusinessEntity.Entities.RetailCentrexTechnicalSettings, Retail.BusinessEntity.Entities",
                        RetailCentrexImportCDRSettings: {
                            SaleAmountPrecision: $scope.scopeModel.saleAmountPrecision
                        }
                    };
                    return obj;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }
]);