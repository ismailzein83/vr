DailyReportController.$inject = ['$scope', 'CarrierAccountAPIService', 'CarrierTypeEnum', 'ZonesService', 'DailyReportMeasureEnum', 'VRNotificationService', 'UtilsService'];

function DailyReportController($scope, CarrierAccountAPIService, CarrierTypeEnum, ZonesService, DailyReportMeasureEnum, VRNotificationService, UtilsService) {

    var gridApi = undefined;

    defineScope();
    load();

    function defineScope() {

        $scope.customers = [];
        $scope.selectedCustomers = [];
        $scope.suppliers = [];
        $scope.selectedSuppliers = [];
        $scope.selectedZones = [];
        $scope.targetDate = undefined;
        $scope.measures = [];

        $scope.calls = [];

        $scope.gridReady = function (api) {
            gridApi = api;
            return retrieveData();
        }

        $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {

            return DailyReportAPIService.GetDailyReportCalls(dataRetrievalInput)
                .then(function (response) {
                    onResponseReady(response);
                })
                .catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });
        }

        $scope.searchZones = function (text) {
            return ZonesService.getSalesZones(text);
        }
    }

    function load() {
        $scope.isInitializing = true;

        loadMeasures();

        UtilsService.waitMultipleAsyncOperations([loadCustomers, loadSuppliers])
            .catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            })
            .finally(function () {
                $scope.isInitializing = false;
            });
    }

    function retrieveData() {
        var query = {
            CarrierIds: getCarrierIds(),
            SupplierIds: getSupplierIds(),
            ZoneIds: getZoneIds(),
            targetDate: $scope.targetDate
        };

        return gridApi.retrieveData(query);
    }

    function loadCustomers() {
        return CarrierAccountAPIService.GetCarriers(CarrierTypeEnum.Customer.value)
            .then(function (response) {
                angular.forEach(response, function (item) {
                    $scope.customers.push(item);
                });
            });
    }

    function loadSuppliers() {
        return CarrierAccountAPIService.GetCarriers(CarrierTypeEnum.Supplier.value)
            .then(function (response) {
                angular.forEach(response, function (item) {
                    $scope.suppliers.push(item);
                });
            });
    }

    function loadMeasures() {
        for (var property in DailyReportMeasureEnum) {
            $scope.measures.push(DailyReportMeasureEnum[property]);
        }
    }
}

appControllers.controller('Analytics_DailyReportController', DailyReportController);