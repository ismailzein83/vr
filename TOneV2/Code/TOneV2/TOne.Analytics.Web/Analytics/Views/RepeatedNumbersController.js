RepeatedNumbersController.$inject = ['$scope', 'UtilsService', '$q', 'RepeatedNumbersAPIService', 'VRNotificationService', 'DataRetrievalResultTypeEnum', 'PeriodEnum', 'RepeatedNumbersMeasureEnum', 'CarrierAccountAPIService', 'CarrierTypeEnum', 'ZonesService', 'BusinessEntityAPIService_temp', 'CallsOptionEnum','VRModalService'];

function RepeatedNumbersController($scope, UtilsService, $q, RepeatedNumbersAPIService, VRNotificationService, DataRetrievalResultTypeEnum, PeriodEnum, RepeatedNumbersMeasureEnum, CarrierAccountAPIService, CarrierTypeEnum, ZonesService, BusinessEntityAPIService, CallsOptionEnum, VRModalService) {

    var mainGridAPI;
    var measures = [];
    var selectedPeriod;
    defineScope();
    load();

    function defineScope() {
        $scope.repeatMoreThan = 10;
        definePeriods();
        defineCallsOption();
        $scope.selectedCallsOption;
        var date;
        var customize = {
            value: -1,
            description: "Customize"
        }
        $scope.onBlurChanged = function () {

            var from = UtilsService.getShortDate($scope.basicSelected ? $scope.fromDate : $scope.fromDate1);
            var oldFrom = UtilsService.getShortDate(date.from);
            var to = UtilsService.getShortDate($scope.basicSelected ? $scope.toDate : $scope.toDate1);
            var oldTo = UtilsService.getShortDate(date.to);
            if (from != oldFrom || to != oldTo)
                $scope.selectedPeriod = customize;

        }
        $scope.periodSelectionChanged = function () {
            if ($scope.selectedPeriod != undefined && $scope.selectedPeriod.value != -1) {
                date = UtilsService.getPeriod($scope.selectedPeriod.value);
                $scope.fromDate = date.from, $scope.toDate = date.to;
                $scope.fromDate1 = date.from, $scope.toDate1 = date.to;
                
                
            }

        }
        $scope.data = [];
        $scope.switches = [];
        $scope.selectedSwitches = [];
        $scope.showResult = false;
        $scope.measures = measures;
        defineMenuActions();
        $scope.onMainGridReady = function (api) {
            mainGridAPI = api;
        }
        $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
            return RepeatedNumbersAPIService.GetRepeatedNumbersData(dataRetrievalInput).then(function (response) {
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
                if($scope.measures[i].value==RepeatedNumbersMeasureEnum.SwitchName.value)
                    $scope.measures[i].isShown=$scope.selectedSwitches.length != 0;
            }

        var filter = buildFilter();
        var query = {
            SwitchIds: filter.SwitchIds,
            From: $scope.basicSelected ?$scope.fromDate:$scope.fromDate1,
            To: $scope.basicSelected ? $scope.toDate : $scope.toDate1,
            Number: $scope.repeatMoreThan,
            Type: $scope.selectedCallsOption.value,
            PhoneNumberType:$scope.selectedPhoneNumberType.value
        };
        return mainGridAPI.retrieveData(query);
    }
    function load() {
        loadMeasures();
        loadSwitches();
        definePhoneNumberType();
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
                };
                modalSettings.onScopeReady = function (modalScope) {
                    modalScope.title = "CDR Log";
                };
                VRModalService.showModal('/Client/Modules/Analytics/Views/CDR/CDRLog.html', parameters, modalSettings);
            }
        }
        
     ];
    }

    function loadMeasures() {
        for (var prop in RepeatedNumbersMeasureEnum) {
            measures.push(RepeatedNumbersMeasureEnum[prop]);
        }

    }

    function buildFilter() {
        var filter = {};
        filter.SwitchIds = getFilterIds($scope.selectedSwitches, "SwitchId");
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

    function defineCallsOption() {
        $scope.callsOption = [];
        for (var p in CallsOptionEnum)
            $scope.callsOption.push(CallsOptionEnum[p]);
        $scope.selectedCallsOption = CallsOptionEnum.AllCalls;

    }
    function definePhoneNumberType() {
        $scope.phoneNumberType = [{
            value:"CDPN",
            description: "CDPN"
        },
        {
            value: "CGPN",
            description: "CGPN"
        }];
        $scope.selectedPhoneNumberType = $scope.phoneNumberType[0];

    }

    function definePeriods() {
        $scope.periods = [];
        for (var p in PeriodEnum)
            $scope.periods.push(PeriodEnum[p]);
        $scope.selectedPeriod = $scope.periods[0];
    }

};

appControllers.controller('Analytics_RepeatedNumbersController', RepeatedNumbersController);