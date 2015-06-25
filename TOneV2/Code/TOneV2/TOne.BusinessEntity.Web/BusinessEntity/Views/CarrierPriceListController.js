CarrierPriceListController.$inject = ['$scope', 'CarrierAPIService', 'CarrierTypeEnum','PriceListAPIService'];
function CarrierPriceListController($scope, CarrierAPIService, CarrierTypeEnum, PriceListAPIService) {
    defineScope();
    load();

    function defineScope() {
        $scope.suppliers = [];
        $scope.selectedSuppliers = [];
    }
    function load() {
        loadSuppliers();
    }
    function GetPRiceList() {
        return PriceListAPIService.GetPRiceList().then(function (response) {
            //gridApi.addItemsToSource(response);
        });
    }
    function loadSuppliers() {
        return CarrierAPIService.GetCarriers(CarrierTypeEnum.Supplier.value).then(function (response) {
            angular.forEach(response, function (itm) {
                $scope.suppliers.push(itm);
            });
        });
    }
}
appControllers.controller('BusinessEntity_CarrierPriceListController', CarrierPriceListController);