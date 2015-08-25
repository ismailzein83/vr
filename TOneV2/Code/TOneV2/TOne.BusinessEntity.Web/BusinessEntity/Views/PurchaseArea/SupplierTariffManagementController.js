SupplierTariffManagementController.$inject = ['$scope', 'CarrierAccountAPIService', 'CarrierTypeEnum', 'SupplierTariffMeasureEnum', 'SupplierTariffAPIService', 'VRNotificationService'];

function SupplierTariffManagementController($scope, CarrierAccountAPIService, CarrierTypeEnum, SupplierTariffMeasureEnum, SupplierTariffAPIService, VRNotificationService) {

    var gridApi = undefined;

    defineScope();
    load();

    function defineScope() {
        $scope.suppliers = [];
        $scope.selectedSupplier = undefined;
        $scope.selectedZones = [];
        $scope.effectiveOn = Date.now();

        $scope.tariffs = [];
        $scope.measures = [];
        $scope.showGrid = false;

        $scope.searchClicked = function () {
            $scope.showGrid = true;
            return retrieveData();
        }

        $scope.gridReady = function (api) {
            gridApi = api;
        }

        $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {

            return SupplierTariffAPIService.GetFilteredSupplierTariffs(dataRetrievalInput)
                .then(function (response) {
                    console.log(response);
                    onResponseReady(response);
                })
                .catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });
        }

        $scope.onSupplierChange = function (supplier) {
            if (supplier != undefined) {
                console.log($scope.selectedSupplier.CarrierAccountID);
            }
        }
    }

    function load() {
        $scope.isInitializing = true;

        loadMeasures();

        // load the suppliers
        CarrierAccountAPIService.GetCarriers(CarrierTypeEnum.Supplier.value, false)
            .then(function (response) {
                angular.forEach(response, function (item) {
                    $scope.suppliers.push(item);
                });
            })
            .catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            })
            .finally(function () {
                $scope.isInitializing = false;
            });
    }

    function retrieveData() {
        var query = {
            selectedSupplierID: ($scope.selectedSupplier != undefined) ? ($scope.selectedSupplier.CarrierAccountID) : null,
            selectedZoneIDs: getSelectedZoneIDs(),
            effectiveOn: $scope.effectiveOn
        };

        return gridApi.retrieveData(query);
    }

    function loadMeasures() {
        for (var property in SupplierTariffMeasureEnum)
            $scope.measures.push(SupplierTariffMeasureEnum[property]);
    }

    function getSelectedZoneIDs() {
        if ($scope.selectedZones.length == 0)
            return [];

        var ids = [];

        for (var i = 0; i < $scope.selectedZones.length; i++)
            ids.push($scope.selectedZones[i].ZoneId);

        return ids;
    }
}

appControllers.controller('Analytics_SupplierTariffManagementController', SupplierTariffManagementController);