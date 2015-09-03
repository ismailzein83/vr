SupplierTariffManagementController.$inject = ['$scope', 'CarrierAccountAPIService', 'CarrierTypeEnum', 'SupplierTariffAPIService', 'UtilsService', 'VRNotificationService'];

function SupplierTariffManagementController($scope, CarrierAccountAPIService, CarrierTypeEnum, SupplierTariffAPIService, UtilsService, VRNotificationService) {

    var gridApi = undefined;

    defineScope();
    load();

    function defineScope() {
        $scope.suppliers = [];
        $scope.selectedSupplier = undefined;
        $scope.selectedZones = [];
        $scope.showZonesMenu = false;
        $scope.effectiveOn = Date.now();

        $scope.tariffs = [];
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
                    onResponseReady(response);
                })
                .catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });
        }
    }

    function load() {
        $scope.isInitializing = true;

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
            SelectedSupplierID: ($scope.selectedSupplier != undefined) ? ($scope.selectedSupplier.CarrierAccountID) : null,
            SelectedZoneIDs: UtilsService.getPropValuesFromArray($scope.selectedZones, 'ZoneId'),
            EffectiveOn: $scope.effectiveOn
        };
        
        return gridApi.retrieveData(query);
    }
}

appControllers.controller('Analytics_SupplierTariffManagementController', SupplierTariffManagementController);