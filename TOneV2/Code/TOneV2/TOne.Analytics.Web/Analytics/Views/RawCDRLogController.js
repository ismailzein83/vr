RawCDRLogController.$inject = ['$scope', 'RawCDRLogAPIService', 'UtilsService', '$q', 'BusinessEntityAPIService_temp', 'RawCDRLogMeasureEnum'];

function RawCDRLogController($scope, RawCDRLogAPIService, UtilsService, $q, BusinessEntityAPIService, RawCDRLogMeasureEnum) {
    var mainGridAPI;
    var measures = [];
    var CDROption = [];
    var isFilterScreenReady;
    defineScope();
    load();
    function defineScope() {
        $scope.fromDate = '2015/06/02';
        $scope.toDate = '2015/06/06';
        $scope.nRecords = '100'
        $scope.switches = [];
        $scope.selectedSwitches = [];
        $scope.inCarrier;
        $scope.outCarrier;
        $scope.inCDPN;
        $scope.outCDPN;
        $scope.cgpn;
        $scope.minDuration;
        $scope.maxDuration;
        $scope.data = [];
        $scope.measures = measures;
        $scope.onMainGridReady = function (api) {
            mainGridAPI = api;
        }
        $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
            return RawCDRLogAPIService.GetRawCDRData(dataRetrievalInput).then(function (response) {
                onResponseReady(response);
            })
        };


        $scope.getData = function () {
            return retrieveData();
        };


    }

    function retrieveData() {
        var filter = buildFilter();
        var query = {
            Switches: filter.SwitchIds,
            FromDate: $scope.fromDate,
            ToDate: $scope.toDate,
            NRecords: $scope.nRecords,
            InCarrier: $scope.inCarrier,
            OutCarrier: $scope.outCarrier,
            InCDPN: $scope.inCDPN,
            OutCDPN: $scope.OutCDPN,
            CGPN: $scope.cgpn,
            MinDuration: $scope.minDuration,
            MaxDuration: $scope.maxDuration,
            DurationType:$scope.selectedDurationType.description
        }
        return mainGridAPI.retrieveData(query);
    }
    function load() {
        loadDurationType();
        loadMeasures();
        loadSwitches();
    }
    function buildFilter() {
        var filter = {};
        filter.SwitchIds = UtilsService.getPropValuesFromArray($scope.selectedSwitches, "SwitchId");
        return filter;
    }

    function loadSwitches() {
        return BusinessEntityAPIService.GetSwitches().then(function (response) {
            angular.forEach(response, function (itm) {
                $scope.switches.push(itm);
            });
        });
    }


    function loadMeasures() {
        for (var prop in RawCDRLogMeasureEnum) {
            measures.push(RawCDRLogMeasureEnum[prop]);
        }
    }
    function loadDurationType() {
        $scope.durationType = [{
            value: 0,
            description:"Sec"
        }, {
            value: 1,
            description: "Min"
        }]
        
        $scope.selectedDurationType = $scope.durationType[1];
    }


};



appControllers.controller('Analytics_RawCDRLogController', RawCDRLogController);