"use strict";

app.directive("vrWhsBeSupplierrateGrid", ["UtilsService", "VRNotificationService", "WhS_BE_SupplierRateAPIService", "VRUIUtilsService",
function (UtilsService, VRNotificationService, WhS_BE_SupplierRateAPIService, VRUIUtilsService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var grid = new SupplierRateGrid($scope, ctrl, $attrs);
            grid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/WhS_BusinessEntity/Directives/SupplierRate/Templates/SupplierRateGridTemplate.html"

    };

    function SupplierRateGrid($scope, ctrl, $attrs) {

        var gridAPI;
        var drillDownManager;
        var effectiveOn;
        this.initializeController = initializeController;

        function initializeController() {
           
            $scope.supplierrates = [];
            $scope.onGridReady = function (api) {
                gridAPI = api;
                drillDownManager = VRUIUtilsService.defineGridDrillDownTabs(getDirectiveTabs(), gridAPI);

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());
                function getDirectiveAPI() {
                   
                    var directiveAPI = {};
                    directiveAPI.loadGrid = function (query) {
                        effectiveOn = query.EffectiveOn;
                        return gridAPI.retrieveData(query);
                    };
                   
                    return directiveAPI;
                }
            };
            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return WhS_BE_SupplierRateAPIService.GetFilteredSupplierRates(dataRetrievalInput)
                    .then(function (response) {
                        if (response && response.Data) {
                            for (var i = 0; i < response.Data.length; i++)
                                drillDownManager.setDrillDownExtensionObject(response.Data[i]);

                        }
                         onResponseReady(response);
                    })
                    .catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    });
            };
        }

        function getDirectiveTabs() {
            var directiveTabs = [];

            var otherRatesTab = {
                title: "Other Rates",
                directive: "vr-whs-be-supplierotherrate-grid",
                loadDirective: function (directiveAPI, rateDataItem) {
                    rateDataItem.otherRateGridAPI = directiveAPI;

                    var otherRateGridPayload = {
                        ZoneId: rateDataItem.Entity.ZoneId,
                        EffectiveOn: effectiveOn
                    };

                    return rateDataItem.otherRateGridAPI.loadGrid(otherRateGridPayload);
                }
            };

            directiveTabs.push(otherRatesTab);

            return directiveTabs;
        }


    }

    return directiveDefinitionObject;

}]);
