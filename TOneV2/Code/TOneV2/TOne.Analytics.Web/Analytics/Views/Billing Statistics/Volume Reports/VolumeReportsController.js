VolumeReportsController.$inject = ['$scope', 'BillingStatisticsAPIService', 'VolumeReportsTimePeriodEnum', 'CarrierTypeEnum', 'CarrierAccountAPIService', 'ZonesService'];
function VolumeReportsController($scope, BillingStatisticsAPIService, VolumeReportsTimePeriodEnum, CarrierTypeEnum, CarrierAccountAPIService, ZonesService) {

    defineScope();
    load();

    function defineScope() {
        $scope.fromDate = '2012/07/01';
        $scope.toDate = '2015/07/10';
        $scope.attempts = 10;
        $scope.topDestinations = 5;
        $scope.timePeriods = [];
        $scope.customers = [];
        $scope.suppliers = [];
        $scope.selectedZones = [];
        $scope.searchZones = function (text) {
            return ZonesService.getSalesZones(text);
        };
        $scope.params = {};
        $scope.params.chartsOption = false;
        loadCustomers();
        loadSuppliers();
        loadTimePeriods();
        loadVolumeReportsOptions();

        $scope.onSearch = function () {
            return getVolumeReportsData();
        };
    }

    function load() {
    }

    function buildFilter() {
        $scope.filter = {};
        $scope.filter.fromDate = $scope.fromDate;
        $scope.filter.toDate = $scope.toDate;
        $scope.filter.selectedTrafficTypeReport = $scope.selectedTrafficTypeReport;
        $scope.filter.timePeriod = $scope.selectedTimePeriod.value;
        $scope.filter.customerId = $scope.params.selectedCustomer != undefined ? $scope.params.selectedCustomer.CarrierAccountID : "";//$scope.selectedCustomer != undefined ? $scope.selectedCustomer.CarrierAccountID : "";
        $scope.filter.requiredCustomerId = $scope.params.requiredCustomer != undefined ? $scope.params.requiredCustomer.CarrierAccountID : "";//$scope.requiredCustomer != undefined ? $scope.requiredCustomer.CarrierAccountID : "";
        $scope.filter.supplierId = $scope.selectedSupplier != undefined ? $scope.selectedSupplier.CarrierAccountID : "";
        $scope.filter.zoneId = $scope.selectedZone != undefined ? $scope.selectedZone.ZoneId : 0;
        $scope.filter.attempts = $scope.attempts;
        $scope.filter.topDestination = $scope.topDestinations;
        $scope.filter.showChartsInPie = $scope.params.chartsOption;
    }

    function loadTimePeriods() {
        $scope.timePeriods = [];
        for (var prop in VolumeReportsTimePeriodEnum) {
            $scope.timePeriods.push(VolumeReportsTimePeriodEnum[prop]);
        }
        $scope.selectedTimePeriod = $scope.timePeriods[0];
    }

    function loadVolumeReportsOptions() {
        $scope.subViewResultTrafficVolumes = {};
        $scope.subViewResultInOutTraffic = {};
        $scope.subViewResultDestinationTrafficVolumes = {};
        $scope.trafficTypeReports = [
            { value: 0, description: "Traffic Volumes", subViewConnector: $scope.subViewResultTrafficVolumes },
            { value: 1, description: "Compare In/Out Traffic", subViewConnector: $scope.subViewResultInOutTraffic },
            { value: 2, description: "Destination Traffic Volumes", subViewConnector: $scope.subViewResultDestinationTrafficVolumes }
        ];
        $scope.selectedTrafficTypeReport = $scope.trafficTypeReports[0];
    }

    function loadCustomers() {
        return CarrierAccountAPIService.GetCarriers(CarrierTypeEnum.Customer.value,false).then(function (response) {
            angular.forEach(response, function (itm) {
                $scope.customers.push(itm);
            });
        });
    }

    function loadSuppliers() {
        return CarrierAccountAPIService.GetCarriers(CarrierTypeEnum.Supplier.value,false).then(function (response) {
            angular.forEach(response, function (itm) {
                $scope.suppliers.push(itm);
            });
        });
    }

    function getVolumeReportsData() {
        $scope.isLoading = true;
        buildFilter();
        return $scope.selectedTrafficTypeReport.subViewConnector.getData($scope.filter)
             .finally(function () {
                 $scope.isLoading = false;
             });

    }
};

appControllers.controller('Analytics_VolumeReportsController', VolumeReportsController);