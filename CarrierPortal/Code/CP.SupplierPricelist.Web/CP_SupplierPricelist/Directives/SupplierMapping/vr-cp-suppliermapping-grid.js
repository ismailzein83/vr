"use strict";
app.directive("vrCpSuppliermappingGrid", ["UtilsService", "CP_SupplierPricelist_SupplierMappingService", "CP_SupplierPricelist_SupplierMappingAPIService", "VRNotificationService",
function (UtilsService, supplierMappingService , supplierMappingAPIService, VRNotificationService) {

    function SupplierMappingGrid($scope, ctrl) {

        var gridAPI;

        this.initializeController = initializeController;

        function initializeController() {

            $scope.supplierMappings = [];
            $scope.onGridReady = function (api) {
                gridAPI = api;
                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());

                function getDirectiveAPI() {
                    var directiveAPI = {};
                    directiveAPI.loadGrid = function (query) {
                        return gridAPI.retrieveData(query);
                    }

                    directiveAPI.onSupplierMappingAdded = function (supplierObject) {
                        gridAPI.itemAdded(supplierObject);
                    }

                    directiveAPI.onSupplierMappingUpdated = function (supplierObject) {
                        console.log(supplierObject)
                        gridAPI.itemUpdated(supplierObject);
                    }

                    return directiveAPI;
                }

            }
        };

        $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
            return supplierMappingAPIService.GetFilteredCustomerSupplierMappings(dataRetrievalInput)
               .then(function (response) {
                   onResponseReady(response);
               })
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               });
        };

        defineMenuActions();


        function defineMenuActions() {
            $scope.gridMenuActions = [{
                name: "Edit",
                clicked: editSupplierMapping
            }
            ];
        }
        function editSupplierMapping(supplierMapping) {
            var ontSupplierMappingUpdated = function (updatedItem) {
                gridAPI.itemUpdated(updatedItem);
            };
            supplierMappingService.editSupplierMapping(supplierMapping.Entity.UserId, ontSupplierMappingUpdated);
        }
    }



    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            var supplierMappingGrid = new SupplierMappingGrid($scope, ctrl, $attrs);
            supplierMappingGrid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/CP_SupplierPriceList/Directives/SupplierMapping/Templates/SupplierMappingGridTemplate.html"

    };
    return directiveDefinitionObject;

}]);