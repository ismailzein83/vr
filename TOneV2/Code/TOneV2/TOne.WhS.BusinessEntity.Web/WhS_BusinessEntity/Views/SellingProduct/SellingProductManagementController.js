(function (appControllers) {

    "use strict";

    sellingProductManagementController.$inject = ['$scope', 'WhS_BE_SellingProductService', 'UtilsService', 'VRNotificationService', 'VRUIUtilsService', 'WhS_BE_SellingProductAPIService'];

    function sellingProductManagementController($scope, WhS_BE_SellingProductService, UtilsService, VRNotificationService, VRUIUtilsService, WhS_BE_SellingProductAPIService) {
        var gridReady;
        var sellingNumberPlanDirectiveAPI;
        var sellingNumberPlanReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        defineScope();
        load();

        function defineScope() {
            $scope.hasAddSellingProductPermission = function () {
                return WhS_BE_SellingProductAPIService.HasAddSellingProductPermission();
            };
            $scope.searchClicked = function () {
                if (gridReady != undefined)
                    return gridReady.loadGrid(getFilterObject());
            };
            $scope.onGridReady = function (api) {
                gridReady = api;
                var filter = {};
                api.loadGrid(filter);
            };
            $scope.onSellingNumberPlanDirectiveReady = function (api) {
                sellingNumberPlanDirectiveAPI = api;
                sellingNumberPlanReadyPromiseDeferred.resolve();

            };

            $scope.AddNewSellingProduct = AddNewSellingProduct;
        }

        function load() {
            $scope.isLoading = true;
            loadAllControls();
        }
        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadSellingNumberPlans])
                .catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.isLoading = false;
                })
               .finally(function () {
                   $scope.isLoading = false;
               });
        }

        function loadSellingNumberPlans() {
            var sellingNumberPlanLoadPromiseDeferred = UtilsService.createPromiseDeferred();

            sellingNumberPlanReadyPromiseDeferred.promise
                .then(function () {
                    var directivePayload;

                    VRUIUtilsService.callDirectiveLoad(sellingNumberPlanDirectiveAPI, directivePayload, sellingNumberPlanLoadPromiseDeferred);
                });
            return sellingNumberPlanLoadPromiseDeferred.promise;
        }

        function getFilterObject() {

            var data = {
                name: $scope.name,
                SellingNumberPlanIds: sellingNumberPlanDirectiveAPI.getSelectedIds()
            };
            return data;
        }

        function AddNewSellingProduct() {
            var onSellingProductAdded = function (sellingProductObj) {
                if (gridReady != undefined)
                    gridReady.onSellingProductAdded(sellingProductObj);
            };
            WhS_BE_SellingProductService.addSellingProduct(onSellingProductAdded);
        }
    }

    appControllers.controller('WhS_BE_SellingProductManagementController', sellingProductManagementController);
})(appControllers);