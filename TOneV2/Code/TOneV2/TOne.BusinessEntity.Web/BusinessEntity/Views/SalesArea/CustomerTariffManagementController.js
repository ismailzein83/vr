CustomerTariffManagementController.$inject = ['$scope', 'CarrierAccountAPIService', 'CarrierTypeEnum', 'CustomerTariffMeasureEnum', 'CustomerTariffAPIService', 'VRNotificationService'];

function CustomerTariffManagementController($scope, CarrierAccountAPIService, CarrierTypeEnum, CustomerTariffMeasureEnum, CustomerTariffAPIService, VRNotificationService) {

    var gridApi = undefined;

    defineScope();
    load();

    function defineScope() {
        $scope.customers = [];
        $scope.selectedCustomer = undefined;
        $scope.selectedZones = [];
        $scope.effectiveOn = Date.now();

        $scope.tariffs = [];
        $scope.showGrid = false;
        $scope.measures = [];

        $scope.searchClicked = function () {
            $scope.showGrid = true;
            //return retrieveData();
        }

        $scope.gridReady = function (api) {
            gridApi = api;
        }

        $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {

            return CustomerTariffAPIService.GetFilteredCustomerTariffs(dataRetrievalInput)
                .then(function (response) {
                    console.log(response);
                    onResponseReady(response);
                })
                .catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });
        }
    }

    function load() {
        $scope.isInitializing = true;

        loadMeasures();

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
            selectedCustomerID: ($scope.selectedCustomer != undefined) ? ($scope.selectedCustomer.CarrierAccountID) : null,
            selectedZoneIDs: getSelectedZoneIDs(),
            effectiveOn: $scope.effectiveOn
        };

        console.log(query);
        return gridApi.retrieveData(query);
    }

    function loadMeasures() {
        for (var property in CustomerTariffMeasureEnum)
            $scope.measures.push(CustomerTariffMeasureEnum[property]);
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

appControllers.controller('BusinessEntity_CustomerTariffManagementController', CustomerTariffManagementController);
