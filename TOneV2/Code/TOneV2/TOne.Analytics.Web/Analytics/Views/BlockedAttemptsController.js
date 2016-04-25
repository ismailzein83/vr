BlockedAttemptsController.$inject = ['$scope', 'UtilsService', '$q', 'BlockedAttemptsAPIService', 'VRNotificationService', 'DataRetrievalResultTypeEnum', 'PeriodEnum', 'CarrierAccountAPIService', 'CarrierTypeEnum', 'ZonesService', 'BusinessEntityAPIService_temp', 'AnalyticsService', 'VRModalService'];

function BlockedAttemptsController($scope, UtilsService, $q, BlockedAttemptsAPIService, VRNotificationService, DataRetrievalResultTypeEnum, PeriodEnum, CarrierAccountAPIService, CarrierTypeEnum, ZonesService, BusinessEntityAPIService, AnalyticsService, VRModalService) {

    var customerAccountDirectiveAPI;
    var customerAccountReadyPromiseDeferred = UtilsService.createPromiseDeferred();

    var saleZoneDirectiveAPI;
    var saleZoneReadyPromiseDeferred = UtilsService.createPromiseDeferred();

    var switchDirectiveAPI;
    var switchReadyPromiseDeferred = UtilsService.createPromiseDeferred();

    var mainGridAPI;
    defineScope();
    load();

    function defineScope() {
        var date = new Date();
        $scope.fromDate = new Date(date.getFullYear(), date.getMonth(), date.getDate(), 00, 00, 00, 00);
        $scope.toDate = new Date(date.getFullYear(), date.getMonth(), date.getDate() + 1, 00, 00, 00, 00);
        $scope.groupByNumber = false;
        $scope.onMainGridReady = function (api) {
            mainGridAPI = api;
        }

        $scope.onCustomerAccountDirectiveReady = function (api) {
            customerAccountDirectiveAPI = api;
            customerAccountReadyPromiseDeferred.resolve();
        }

        $scope.onSwitchDirectiveReady = function (api) {
            switchDirectiveAPI = api;
            switchReadyPromiseDeferred.resolve();
        }

        $scope.onSaleZoneDirectiveReady = function (api) {
            saleZoneDirectiveAPI = api;
            saleZoneReadyPromiseDeferred.resolve();
        }

        $scope.searchClicked = function () {
            return retrieveData();
        };
    }

    function retrieveData() {
        var filter = buildFilter();
        var query = {
            Filter: filter,
            From: $scope.fromDate,
            To: $scope.toDate,
            GroupByNumber: $scope.groupByNumber
        };
        return mainGridAPI.retrieveData(query);
    }
    function load() {
        $scope.isLoading = true;
        loadAllControls();
    }

    function loadAllControls() {
        return UtilsService.waitMultipleAsyncOperations([loadCustomers, loadSaleZones, loadSwitches])
            .catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            })
            .finally(function () {
                $scope.isLoading = false;
            });
    }

    function buildFilter() {
        var filter = {};
        filter.SwitchIds = switchDirectiveAPI.getSelectedIds();
        filter.CustomerIds = customerAccountDirectiveAPI.getSelectedIds();
        filter.SaleZoneIds = saleZoneDirectiveAPI.getSelectedIds();
        return filter;

    }

    function loadSwitches() {
        var loadSwitchPromiseDeferred = UtilsService.createPromiseDeferred();
        switchReadyPromiseDeferred.promise.then(function () {
            VRUIUtilsService.callDirectiveLoad(switchDirectiveAPI, undefined, loadSwitchPromiseDeferred);
        });
        return loadSwitchPromiseDeferred.promise;
    }

    function loadSaleZones() {
        var loadSaleZonePromiseDeferred = UtilsService.createPromiseDeferred();
        saleZoneReadyPromiseDeferred.promise.then(function () {
            VRUIUtilsService.callDirectiveLoad(saleZoneDirectiveAPI, undefined, loadSaleZonePromiseDeferred);
        });
        return loadSaleZonePromiseDeferred.promise;
    }

    function loadCustomers() {
        var loadCustomerAccountPromiseDeferred = UtilsService.createPromiseDeferred();
        customerAccountReadyPromiseDeferred.promise.then(function () {
            VRUIUtilsService.callDirectiveLoad(customerAccountDirectiveAPI, undefined, loadCustomerAccountPromiseDeferred);
        });
        return loadCustomerAccountPromiseDeferred.promise;
    }

};

appControllers.controller('Analytics_BlockedAttemptsController', BlockedAttemptsController);