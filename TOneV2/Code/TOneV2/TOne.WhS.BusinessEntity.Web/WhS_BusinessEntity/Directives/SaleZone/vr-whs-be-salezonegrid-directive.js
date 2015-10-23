"use strict";

app.directive("vrWhsBeCarrieraccountgrid", ["WhS_BE_SaleZoneAPIService", "WhS_BE_MainService", "UtilsService", "VRNotificationService",

    function (WhS_BE_SaleZoneAPIService, WhS_BE_MainService, UtilsService, VRNotificationService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            var saleZoneGrid = new SaleZoneGrid($scope, ctrl, $attrs);
            saleZoneGrid.initializeController();
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

            $scope.saleZones = [];

            $scope.onGridReady = function (api) {
                gridAPI = api;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());

                function getDirectiveAPI() {
                    var directiveAPI = {};

                    directiveAPI.loadGrid = function (query) {
                        return gridAPI.retrieveData(query);
                    };

                    directiveAPI.onSaleZoneAdded = function (saleZoneObj) {
                        gridAPI.itemAdded(saleZoneObj);
                    };

                    return directiveAPI;
                }
            };

            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return WhS_BE_SaleZoneAPIService.GetFilteredSaleZones(dataRetrievalInput)
                    .then(function (response) {
                        onResponseReady(response);
                    })
                    .catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    });
            };

            defineMenuActions();
        }

        function defineMenuActions() {
            $scope.gridMenuActions = [
                {
                    name: "Edit",
                    clicked: editSaleZone
                },
                {
                    name: "Edit",
                    clicked: deleteSaleZone
                }
            ];
        }

        function editSaleZone(saleZoneDataItem) {
            return;
        }

        function deleteSaleZone(saleZoneDataItem) {
            return;
        }
    }

    return directiveDefinitionObject;

}]);
