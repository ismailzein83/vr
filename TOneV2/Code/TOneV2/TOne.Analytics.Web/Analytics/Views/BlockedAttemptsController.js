BlockedAttemptsController.$inject = ['$scope', 'UtilsService', '$q', 'BlockedAttemptsAPIService', 'VRNotificationService', 'DataRetrievalResultTypeEnum', 'PeriodEnum', 'CarrierAccountAPIService', 'CarrierTypeEnum', 'ZonesService', 'BusinessEntityAPIService_temp','AnalyticsService','VRModalService'];

function BlockedAttemptsController($scope, UtilsService, $q, BlockedAttemptsAPIService, VRNotificationService, DataRetrievalResultTypeEnum, PeriodEnum, CarrierAccountAPIService, CarrierTypeEnum, ZonesService, BusinessEntityAPIService, AnalyticsService, VRModalService) {

    var mainGridAPI;
    var measures = [];
    defineScope();
    load();

    function defineScope() {
        definePeriods();
        $scope.basicToDate;
        $scope.basicFromDate;
        $scope.advancedFromDate;
        $scope.advancedToDate;
        $scope.isShown = false;
        var date;
        var customize = {
            value: -1,
            description: "Customize"
        }
        $scope.periodSelectionChanged = function () {
            if ($scope.selectedPeriod != undefined && $scope.selectedPeriod.value != -1) {
                date = UtilsService.getPeriod($scope.selectedPeriod.value);
                $scope.basicFromDate = date.from;
                $scope.basicToDate = date.to;
                $scope.advancedFromDate = date.from;
                $scope.advancedToDate = date.to;
            }

        }
        $scope.onBlurFromChanged= function () 
        {
            var from = UtilsService.getShortDate($scope.basicSelected ? $scope.basicFromDate : $scope.advancedFromDate);
            var oldFrom = UtilsService.getShortDate(date.from);
            if (from != oldFrom)
                $scope.selectedPeriod = customize;
            $scope.basicSelected ? $scope.advancedFromDate = $scope.basicFromDate : $scope.basicFromDate = $scope.advancedFromDate
        }
        $scope.onBlurToChanged = function () {
            var to = UtilsService.getShortDate($scope.basicSelected ? $scope.basicToDate : $scope.advancedToDate);
            var oldTo = UtilsService.getShortDate(date.to);
            if (to != oldTo)
                $scope.selectedPeriod = customize;
            console.log(UtilsService.dateToServerFormat($scope.basicToDate));
            $scope.basicSelected ? $scope.advancedToDate = $scope.basicToDate : $scope.basicToDate = $scope.advancedToDate
        }
        $scope.data = [];
        $scope.switches = [];
        $scope.zones = [];
        $scope.selectedZones = [];
        $scope.selectedSwitches = [];
        $scope.customers = [];
        $scope.selectedCustomers = [];
        $scope.showResult = false;
        $scope.groupByNumber = false;
        $scope.measures = measures;
        defineMenuActions();
        $scope.onMainGridReady = function (api) {
            mainGridAPI = api;
        }
        $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
            return BlockedAttemptsAPIService.GetBlockedAttempts(dataRetrievalInput).then(function (response) {
                onResponseReady(response);
                $scope.showResult = true;
            })
        };

       
        $scope.searchClicked = function () {
            
            return retrieveData();
        };
        $scope.searchZones = function (text) {
            return ZonesService.getSalesZones(text);
        }
    }

    function retrieveData() {
            $scope.isShown = $scope.groupByNumber;
        var filter=buildFilter();
        var query = {
            Filter: filter,
            From: $scope.basicFromDate,
            To: $scope.basicToDate,
            GroupByNumber: $scope.groupByNumber
        };
        return mainGridAPI.retrieveData(query);
    }
    function load() {
        loadSwitches();
    }

    function defineMenuActions() {
        $scope.gridMenuActions = [{
            name: "CDRs",
            clicked: function (dataItem) {
                var modalSettings = {
                    useModalTemplate: true,
                    width: "80%"//,
                    //maxHeight: "800px"
                };
                var parameters = {
                    fromDate: $scope.basicFromDate,
                    toDate: $scope.basicToDate,
                    customerIds: dataItem.CustomerID != null || dataItem.CustomerID != undefined ? [dataItem.CustomerID] : null,
                    zoneIds: dataItem.OurZoneID != null || dataItem.OurZoneID != undefined ? [dataItem.OurZoneID] : null,
                    supplierIds: dataItem.SupplierID != null || dataItem.SupplierID != undefined ? [dataItem.SupplierID] : null,
                    switchIds: [dataItem.SwitchID],
                    ///[dataItem.GroupKeyValues[0].Id]
                };

                modalSettings.onScopeReady = function (modalScope) {
                    modalScope.title = "CDR Log";
                };



                VRModalService.showModal('/Client/Modules/Analytics/Views/CDR/CDRLog.html', parameters, modalSettings);
            }
        }];
    }

    function buildFilter() {
        var filter = {};
        filter.SwitchIds = getFilterIds($scope.selectedSwitches, "SwitchId");
        filter.CustomerIds = getFilterIds($scope.selectedCustomers, "CarrierAccountID");
        filter.Zones = getFilterIds($scope.selectedZones, "Zone");
        return filter;
    }

    function getFilterIds(values, idProp) {
        var filterIds = [];
        if (values.length > 0) {
            angular.forEach(values, function (val) {
                filterIds.push(val[idProp]);
            });
        }
        return filterIds;
    }

    function loadSwitches() {
        return BusinessEntityAPIService.GetSwitches().then(function (response) {
            angular.forEach(response, function (itm) {
                $scope.switches.push(itm);
            });
        });
    }




    function definePeriods() {
        $scope.periods = [];
        for (var p in PeriodEnum)
            $scope.periods.push(PeriodEnum[p]);
        $scope.selectedPeriod = PeriodEnum.Today;
    }

};

appControllers.controller('Analytics_BlockedAttemptsController', BlockedAttemptsController);