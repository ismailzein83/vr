BlockedAttemptsController.$inject = ['$scope', 'UtilsService', '$q', 'BlockedAttemptsAPIService', 'VRNotificationService', 'DataRetrievalResultTypeEnum', 'PeriodEnum', 'BlockedAttemptsMeasureEnum', 'CarrierAccountAPIService', 'CarrierTypeEnum', 'ZonesService', 'BusinessEntityAPIService_temp','AnalyticsService','VRModalService'];

function BlockedAttemptsController($scope, UtilsService, $q, BlockedAttemptsAPIService, VRNotificationService, DataRetrievalResultTypeEnum, PeriodEnum, BlockedAttemptsMeasureEnum, CarrierAccountAPIService, CarrierTypeEnum, ZonesService, BusinessEntityAPIService, AnalyticsService, VRModalService) {

    var mainGridAPI;
    var measures = [];
    var selectedPeriod;
    defineScope();
    load();

    function defineScope() {
        definePeriods();
        $scope.onValueChanged = function () {
            console.log($scope.selectedPeriod);
            if ($scope.selectedPeriod != selectedPeriod) {
                var customize = {
                    value: -1,
                    description: "Customize"
                }
                selectedPeriod = $scope.selectedPeriod;
                $scope.selectedPeriod = customize;
            }

        }
        $scope.periodSelectionChanged = function () {
            if ($scope.selectedPeriod != undefined && $scope.selectedPeriod.value != -1) {
                var date = UtilsService.getPeriod($scope.selectedPeriod.value);
                $scope.fromDate = date.from;
                $scope.toDate = date.to;
            }

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
        for (var i = 0; i < $scope.measures.length; i++) {
            if ($scope.measures[i].value == BlockedAttemptsMeasureEnum.CLI.value || $scope.measures[i].value == BlockedAttemptsMeasureEnum.PhoneNumber.value)
                $scope.measures[i].isShown = $scope.groupByNumber;
        }
        var filter=buildFilter();
        var query = {
            Filter: filter,
            From: $scope.fromDate,
            To: $scope.toDate,
            GroupByNumber: $scope.groupByNumber
        };
        return mainGridAPI.retrieveData(query);
    }
    function load() {
        loadMeasures();
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
                    fromDate: $scope.fromDate,
                    toDate: $scope.toDate,
                    customerIds: dataItem.CustomerID != null || dataItem.CustomerID != undefined ? [dataItem.CustomerID] : null,
                    zoneIds: dataItem.OurZoneID != null || dataItem.OurZoneID != undefined ? [dataItem.OurZoneID] : null,
                    supplierIds: dataItem.SupplierID != null || dataItem.SupplierID != undefined ? [dataItem.SupplierID] : null,
                    switchIds: [dataItem.SwitchID],
                    ///[dataItem.GroupKeyValues[0].Id]
                };

             



                VRModalService.showModal('/Client/Modules/Analytics/Views/CDR/CDRLog.html', parameters, modalSettings);
            }
        },
        {
            name: "Show Confirmation",
            clicked: function (dataItem) {
                VRNotificationService.showConfirmation('Are you sure you want to delete?')
                .then(function (result) {
                    if (result)
                        console.log('Confirmed');
                    else
                        console.log('not confirmed');
                });
            }
        },
        {
            name: "Show Error",
            clicked: function (dataItem) {
                VRNotificationService.showError('Error Message');
            }
        }];
    }

    function loadMeasures() {
        for (var prop in BlockedAttemptsMeasureEnum) {
            measures.push(BlockedAttemptsMeasureEnum[prop]);
        }
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
        $scope.selectedPeriod = $scope.periods[0];
    }

};

appControllers.controller('Analytics_BlockedAttemptsController', BlockedAttemptsController);