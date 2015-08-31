DailyReportController.$inject = ['$scope', 'DailyReportAPIService', 'ZonesService', 'VRNotificationService', 'UtilsService'];

function DailyReportController($scope, DailyReportAPIService, ZonesService, VRNotificationService, UtilsService) {

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
            SelectedZoneIDs: UtilsService.getPropValuesFromArray($scope.selectedZones, 'ZoneId'),
            SelectedCustomerIDs: UtilsService.getPropValuesFromArray($scope.selectedCustomers, 'CarrierAccountID'),
            SelectedSupplierIDs: UtilsService.getPropValuesFromArray($scope.selectedSuppliers, 'CarrierAccountID'),
            TargetDate: $scope.targetDate
        };

        return gridApi.retrieveData(query);
    }
}

appControllers.controller('Analytics_DailyReportController', DailyReportController);