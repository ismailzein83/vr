CarrierPriceListController.$inject = ['$scope', 'CarrierAPIService', 'CarrierTypeEnum'];
function CarrierPriceListController($scope, CarrierAPIService, CarrierTypeEnum) {
    defineScope();
    load();

    function defineScope() {
        $scope.suppliers = [];
        $scope.selectedSuppliers = [];
    }
    function load() {
        loadSuppliers();
    }
    function loadSuppliers() {
        return CarrierAPIService.GetCarriers(CarrierTypeEnum.Supplier.value).then(function (response) {
            angular.forEach(response, function (itm) {
                $scope.suppliers.push(itm);
            });
        });
    }
}
appControllers.controller('Carrier_CarrierPriceListController', CarrierPriceListController);