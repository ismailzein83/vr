VolumeReportsController.$inject = ['$scope', 'VolumeReportsAPIService', 'VolumeReportsTimePeriodEnum', 'VolumeReportsOptionsEnum', 'CarrierTypeEnum', 'CarrierAPIService', 'ZonesService'];
function VolumeReportsController($scope, VolumeReportsAPIService, VolumeReportsTimePeriodEnum, VolumeReportsOptionsEnum, CarrierTypeEnum, CarrierAPIService, ZonesService) {

    defineScope();
    load();


    function defineScope() {
        $scope.fromDate = '2013/07/01';
        $scope.toDate = '2015/07/10';
        $scope.timePeriods = [];
        $scope.trafficTypeReports = [];
        $scope.customers = [];
        $scope.selectedCustomers = [];
        $scope.suppliers = [];
        $scope.selectedSuppliers = [];
        $scope.selectedZones = [];
        $scope.searchZones = function (text) {
            return ZonesService.getSalesZones(text);
        }
        loadCustomers();
        loadSuppliers();
        loadTimePeriods();
        loadVolumeReportsOptions();


        $scope.data = [];
        $scope.chartData = [];
        $scope.filterObject = [];

       

        $scope.onSearch = function () {
            // return getVolumeReportsData();
            alert('search clicked');
        }
        //$scope.chartReady = function (api) {
        //    chartAPI = api;
        //};
    }

    function load() { }
    function loadTimePeriods() {
        $scope.timePeriods = [];
        for (var prop in VolumeReportsTimePeriodEnum) {
            $scope.timePeriods.push(VolumeReportsTimePeriodEnum[prop]);
        }
        $scope.selectedTimePeriod = $scope.timePeriods[0];
    }
    function loadVolumeReportsOptions() {
        $scope.trafficTypeReports = [];
        for (var prop in VolumeReportsOptionsEnum) {
            $scope.trafficTypeReports.push(VolumeReportsOptionsEnum[prop]);
        }
        $scope.selectedReportOption = $scope.trafficTypeReports[0];
    }
    function loadCustomers() {
        return CarrierAPIService.GetCarriers(CarrierTypeEnum.Customer.value).then(function (response) {
            angular.forEach(response, function (itm) {
                $scope.customers.push(itm);
            });
        });
    }
    function loadSuppliers() {
        return CarrierAPIService.GetCarriers(CarrierTypeEnum.Supplier.value).then(function (response) {
            angular.forEach(response, function (itm) {
                $scope.suppliers.push(itm);
            });
        });
    }
    function getVolumeReportsData() {



    }
   
};

appControllers.controller('Analytics_VolumeReportsController', VolumeReportsController);