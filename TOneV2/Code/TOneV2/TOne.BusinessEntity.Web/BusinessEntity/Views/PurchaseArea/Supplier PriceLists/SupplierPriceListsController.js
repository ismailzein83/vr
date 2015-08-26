SupplierPricelistsController.$inject = ['$scope', 'UtilsService', '$q', 'CarrierAccountAPIService', 'CarrierTypeEnum', 'SupplierPricelistsAPIService'];

function SupplierPricelistsController($scope, UtilsService, $q, CarrierAccountAPIService, CarrierTypeEnum, SupplierPricelistsAPIService) {
    var mainGridAPI;
    defineScope();
    load();
    function defineScope() {
        $scope.data = [];
        $scope.showResult = false;
        $scope.selectedSupplier;
        $scope.suppliers = [];
        $scope.onMainGridReady = function (api) {
            mainGridAPI = api;
        }
        $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
            return SupplierPricelistsAPIService.GetSupplierPriceLists(dataRetrievalInput).then(function (response) {
                onResponseReady(response);
                $scope.showResult = true;
            })
        };
        $scope.getData = function () {
            return retrieveData();
        };
    }

    function retrieveData() {
        if (mainGridAPI == undefined)
            return;
        var query = $scope.selectedSupplier.CarrierAccountID;

        return mainGridAPI.retrieveData(query);
    }
    function load() {
        loadSuppliers();
    }

    function loadSuppliers() {
        return CarrierAccountAPIService.GetCarriers(CarrierTypeEnum.Supplier.value, false).then(function (response) {
            angular.forEach(response, function (itm) {
                $scope.suppliers.push(itm);
            });
            $scope.selectedSupplier = $scope.suppliers[0];
        });

    }
};
appControllers.controller('BE_SupplierPricelistsController', SupplierPricelistsController);