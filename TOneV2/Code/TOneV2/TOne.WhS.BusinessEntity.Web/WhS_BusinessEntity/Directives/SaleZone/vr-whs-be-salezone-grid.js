"use strict";

app.directive("vrWhsBeSalezoneGrid", ["UtilsService", "VRNotificationService", "WhS_BE_SaleZoneAPIService", "WhS_BE_SaleZoneService", "VRUIUtilsService",
function (UtilsService, VRNotificationService, WhS_BE_SaleZoneAPIService, WhS_BE_SaleZoneService, VRUIUtilsService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var grid = new SaleZoneGrid($scope, ctrl, $attrs);
            grid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/WhS_BusinessEntity/Directives/SaleZone/Templates/SaleZoneGridTemplate.html"

    };

    function SaleZoneGrid($scope, ctrl, $attrs) {

        var gridAPI;
        this.initializeController = initializeController;
        var effectiveOn;
        function initializeController() {
            $scope.showGrid = false;
            var gridDrillDownTabsObj;
            $scope.salezones = [];
            $scope.onGridReady = function (api) {
                gridAPI = api;


                var drillDownDefinitions = [];
                var drillDownDefinition = {};

                drillDownDefinition.title = "Sale Codes";
                drillDownDefinition.directive = "vr-whs-be-salecode-grid";

                drillDownDefinition.loadDirective = function (directiveAPI, saleZoneItem) {
                    saleZoneItem.saleCodeGridAPI = directiveAPI;
                    var queryHandler = {
                        $type: "TOne.WhS.BusinessEntity.Business.SaleCodeQueryHandler, TOne.WhS.BusinessEntity.Business"
                    };

                    queryHandler.Query = {
                        ZonesIds: [saleZoneItem.Entity.SaleZoneId],
                        EffectiveOn: effectiveOn
                    };

                    var payload = {
                        queryHandler: queryHandler,
                        hidesalezonecolumn: true
                    };
                    
                    return saleZoneItem.saleCodeGridAPI.loadGrid(payload);
                };
                drillDownDefinitions.push(drillDownDefinition);
                gridDrillDownTabsObj = VRUIUtilsService.defineGridDrillDownTabs(drillDownDefinitions, gridAPI, $scope.gridMenuActions);


                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());
                function getDirectiveAPI() {
                   
                    var directiveAPI = {};
                    directiveAPI.loadGrid = function (query) {
                        effectiveOn = query.EffectiveOn;
                        return gridAPI.retrieveData(query);
                    }
                   
                    return directiveAPI;
                }
            };
            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return WhS_BE_SaleZoneAPIService.GetFilteredSaleZones(dataRetrievalInput)
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
