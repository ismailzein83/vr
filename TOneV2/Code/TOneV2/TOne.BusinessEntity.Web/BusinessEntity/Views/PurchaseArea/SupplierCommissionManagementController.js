SupplierCommissionManagementController.$inject = ['$scope', 'UtilsService', '$q', 'CarrierAccountAPIService', 'CarrierTypeEnum', 'SupplierCommissionAPIService'];

function SupplierCommissionManagementController($scope, UtilsService, $q, CarrierAccountAPIService, CarrierTypeEnum, SupplierCommissionAPIService) {
    var mainGridAPI;
    defineScope();
    load();
    function defineScope() {
        $scope.data = [];
        $scope.showResult = false;
        $scope.selectedSupplier;
        $scope.suppliers = [];
        $scope.selectedZones=[];
        $scope.effectiveFrom = new Date();
        $scope.onMainGridReady = function (api) {
            mainGridAPI = api;
        }
        $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
            return SupplierCommissionAPIService.GetSupplierCommissions(dataRetrievalInput).then(function (response) {
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
        var query = {

            SupplierId: $scope.selectedSupplier != undefined ? $scope.selectedSupplier.CarrierAccountID : null,
            ZoneIds: $scope.selectedZones.length > 0 ? UtilsService.getPropValuesFromArray($scope.selectedZones, 'ZoneId') : undefined,
            EffectiveFrom: $scope.effectiveFrom
        }

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
           // $scope.selectedSupplier = $scope.suppliers[0];
        });

    }
};
appControllers.controller('BE_SupplierCommissionManagementController', SupplierCommissionManagementController);