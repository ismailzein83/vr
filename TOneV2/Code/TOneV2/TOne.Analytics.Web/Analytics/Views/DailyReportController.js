DailyReportController.$inject = ['$scope', 'DailyReportAPIService', 'ZonesService', 'DailyReportMeasureEnum', 'VRNotificationService', 'UtilsService'];

function DailyReportController($scope, DailyReportAPIService, ZonesService, DailyReportMeasureEnum, VRNotificationService, UtilsService) {

    var gridApi = undefined;

    defineScope();
    load();

    function defineScope() {

        $scope.selectedCustomers = [];
        $scope.selectedSuppliers = [];
        $scope.selectedZones = [];
        $scope.targetDate = Date.now();
        $scope.showGrid = false;

        $scope.calls = [];

        $scope.searchZones = function (text) {
            return ZonesService.getSalesZones(text);
        }

        $scope.searchClicked = function () {
            $scope.showGrid = true;
            return retrieveData();
        }

        $scope.gridReady = function (api) {
            gridApi = api;
        }

        $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {

            return DailyReportAPIService.GetFilteredDailyReportCalls(dataRetrievalInput)
                .then(function (response) {
                    onResponseReady(response);
                })
                .catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });
        }
    }

    function load() {
    }

    function retrieveData() {
        var query = {
            selectedZoneIDs: getSelectedZoneIDs(),
            selectedCustomerIDs: getCarrierAccountIDs($scope.selectedCustomers),
            selectedSupplierIDs: getCarrierAccountIDs($scope.selectedSuppliers),
            targetDate: $scope.targetDate
        };

        return gridApi.retrieveData(query);
    }

    function getSelectedZoneIDs() {
        if ($scope.selectedZones.length == 0)
            return [];

        var ids = [];

        for (var i = 0; i < $scope.selectedZones.length; i++)
            ids.push($scope.selectedZones[i].ZoneId);

        return ids;
    }

    function getCarrierAccountIDs(carriers) {
        if (carriers.length == 0)
            return [];

        var ids = [];

        for (var i = 0; i < carriers.length; i++) {
            ids.push(carriers[i].CarrierAccountID);
        }

        return ids;
    }
}

appControllers.controller('Analytics_DailyReportController', DailyReportController);