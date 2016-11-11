"use strict";

app.directive("vrWhsBeSalerateGrid", ["UtilsService", "VRNotificationService", "WhS_BE_SaleRateAPIService","VRUIUtilsService",
function (UtilsService, VRNotificationService, WhS_BE_SaleRateAPIService, VRUIUtilsService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var grid = new SaleRateGrid($scope, ctrl, $attrs);
            grid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/WhS_BusinessEntity/Directives/SaleRate/Templates/SaleRateGridTemplate.html"

    };

    function SaleRateGrid($scope, ctrl, $attrs) {

        var gridAPI;
        this.initializeController = initializeController;

        function initializeController() {
            $scope.showGrid = false;
            var gridDrillDownTabsObj;
            $scope.salerates = [];
            $scope.onGridReady = function (api) {
                gridAPI = api;

                var drillDownDefinitions = [];
                var drillDownDefinition = {};

                drillDownDefinition.title = "Other Rates";
                drillDownDefinition.directive = "vr-whs-be-saleotherrate-grid";

                drillDownDefinition.loadDirective = function (directiveAPI, rateItem) {
                    rateItem.otherRateGridAPI = directiveAPI;
                    rateItem.otherRateGridAPI.loadGrid(rateItem.OtherRates);
                };
                drillDownDefinitions.push(drillDownDefinition);
                gridDrillDownTabsObj = VRUIUtilsService.defineGridDrillDownTabs(drillDownDefinitions, gridAPI, $scope.gridMenuActions);
                
                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());
                function getDirectiveAPI() {
                   
                    var directiveAPI = {};
                    directiveAPI.loadGrid = function (query) {

                        return gridAPI.retrieveData(query);
                    };
                   
                    return directiveAPI;
                }
            };
            $scope.isExpandable = function (dataItem) {
                return (dataItem.OtherRates != null && dataItem.OtherRates.length > 0);
            };
            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return WhS_BE_SaleRateAPIService.GetFilteredSaleRate(dataRetrievalInput)
                    .then(function (response) {
                        if (response && response.Data) {
                            for (var i = 0; i < response.Data.length; i++) {
                                gridDrillDownTabsObj.setDrillDownExtensionObject(response.Data[i]);
                            }
                        }
                        $scope.showGrid = true;
                         onResponseReady(response);
                    })
                    .catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    });
            };
        }

    }

    return directiveDefinitionObject;

}]);
