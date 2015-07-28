CarrierPriceListController.$inject = ['$scope', 'CarrierAccountAPIService', 'CarrierTypeEnum','PriceListAPIService'];
function CarrierPriceListController($scope, CarrierAccountAPIService, CarrierTypeEnum, PriceListAPIService) {
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
        return CarrierAccountAPIService.GetCarriers(CarrierTypeEnum.Supplier.value).then(function (response) {
            angular.forEach(response, function (itm) {
                $scope.suppliers.push(itm);
            });
        });
    }
}
appControllers.controller('BusinessEntity_CarrierPriceListController', CarrierPriceListController);