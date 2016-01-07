"use strict";

app.directive("vrWhsBeSalezoneGrid", ["UtilsService", "VRNotificationService", "WhS_BE_SaleZoneAPIService",
function (UtilsService, VRNotificationService, WhS_BE_SaleZoneAPIService, WhS_BE_MainService) {

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

        function initializeController() {
           
            $scope.salezones = [];
            $scope.onGridReady = function (api) {
                gridAPI = api;
                
                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());
                function getDirectiveAPI() {
                   
                    var directiveAPI = {};
                    directiveAPI.loadGrid = function (query) {
                       
                        return gridAPI.retrieveData(query);
                    }
                   
                    return directiveAPI;
                }
            };
            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return WhS_BE_SaleZoneAPIService.GetFilteredSaleZones(dataRetrievalInput)
                    .then(function (response) {
                        if (response.Data != undefined) {
                            for (var i = 0; i < response.Data.length; i++) {
                                setDataItemExtension(response.Data[i]);
                            }
                        }
                         onResponseReady(response);
                    })
                    .catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    });
            };
        }

        function setDataItemExtension(dataItem) {

            var extensionObject = {};
            var query = {
                ZonesIds: [dataItem.Entity.SaleZoneId],
            }
            extensionObject.onGridReady = function (api) {
                extensionObject.saleCodeGridAPI = api;
                extensionObject.saleCodeGridAPI.loadGrid(query);
                extensionObject.onGridReady = undefined;
            };
            dataItem.extensionObject = extensionObject;

        }

    }

    return directiveDefinitionObject;

}]);
