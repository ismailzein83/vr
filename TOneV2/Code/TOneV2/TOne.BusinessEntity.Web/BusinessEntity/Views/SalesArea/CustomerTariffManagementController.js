CustomerTariffManagementController.$inject = ['$scope', 'CarrierAccountAPIService', 'CarrierTypeEnum', 'CustomerTariffAPIService', 'UtilsService', 'VRNotificationService'];

function CustomerTariffManagementController($scope, CarrierAccountAPIService, CarrierTypeEnum, CustomerTariffAPIService, UtilsService, VRNotificationService) {

    var gridApi = undefined;

    defineScope();
    load();

    function defineScope() {
        $scope.customers = [];
        $scope.selectedCustomer = undefined;
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

            return CustomerTariffAPIService.GetFilteredCustomerTariffs(dataRetrievalInput)
                .then(function (response) {
                    onResponseReady(response);
                })
                .catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });
        }

        $scope.onCustomerChanged = function (selectedCustomer, allCustomers) {
            $scope.showZonesMenu = ($scope.selectedCustomer != undefined) ? true : false;
        }
    }

    function load() {
        $scope.isInitializing = true;

        // load the customers
        CarrierAccountAPIService.GetCarriers(CarrierTypeEnum.Customer.value, false)
            .then(function (response) {
                angular.forEach(response, function (item) {
                    $scope.customers.push(item);
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
            SelectedCustomerID: ($scope.selectedCustomer != undefined) ? ($scope.selectedCustomer.CarrierAccountID) : null,
            SelectedZoneIDs: UtilsService.getPropValuesFromArray($scope.selectedZones, 'ZoneId'),
            EffectiveOn: $scope.effectiveOn
        };

        return gridApi.retrieveData(query);
    }
}

appControllers.controller('BusinessEntity_CustomerTariffManagementController', CustomerTariffManagementController);
