RawCDRLogController.$inject = ['$scope', 'RawCDRLogAPIService', 'UtilsService', '$q', 'BusinessEntityAPIService_temp', 'VRModalService','VRNotificationService','DurationEnum'];

function RawCDRLogController($scope, RawCDRLogAPIService, UtilsService, $q, BusinessEntityAPIService, VRModalService, VRNotificationService, DurationEnum) {
    var mainGridAPI;
    var CDROption = [];
    var isFilterScreenReady;
    defineScope();
    load();
    function defineScope() {
        $scope.fromDate = '2015/06/02';
        $scope.toDate = '2015/06/06';
        $scope.fromDate1 = '2015/06/02';
        $scope.toDate1 = '2015/06/06';
        $scope.nRecords = '100'
        $scope.nRecords1 = '100'
        $scope.switches = [];
        
        $scope.selectedSwitches = [];
        $scope.inCarrier;
        $scope.outCarrier;
        $scope.inCDPN;
        $scope.outCDPN;
        $scope.showResult = false;
        $scope.cgpn;
        $scope.minDuration;
        $scope.maxDuration;
        $scope.whereCondtion;
        $scope.data = [];
        $scope.onInfoClick = function () {
            var settings = {};
            settings.onScopeReady = function (modalScope) {
                modalScope.title = "CDR Table Definition ";
            };
            VRModalService.showModal('/Client/Modules/Analytics/Views/RawCDRLogTemplate/RawCDRLogTemplate.html', null, settings);

        }
        $scope.onMainGridReady = function (api) {
            mainGridAPI = api;
        }
        $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
            return RawCDRLogAPIService.GetRawCDRData(dataRetrievalInput).then(function (response) {
                onResponseReady(response);
                $scope.showResult = true;
            }).catch(function (error) {
               // VRNotificationService.notifyException("Sintex Error", "dsad");

            });
        };


        $scope.getData = function () {
            return retrieveData();
        };


    }

    function retrieveData() {
        var filter = buildFilter();
        var query = {
            Switches: filter.SwitchIds,
            FromDate: $scope.basicSelected ? $scope.fromDate : $scope.fromDate1,
            ToDate: $scope.basicSelected ? $scope.toDate : $scope.toDate1,
            NRecords: $scope.basicSelected ? $scope.nRecords : $scope.nRecords1,
            InCarrier: $scope.inCarrier,
            OutCarrier: $scope.outCarrier,
            InCDPN: $scope.inCDPN,
            OutCDPN: $scope.OutCDPN,
            CGPN: $scope.cgpn,
            MinDuration: $scope.minDuration,
            MaxDuration: $scope.maxDuration,
            DurationType: $scope.selectedDurationType.value,
            WhereCondition: $scope.whereCondtion
        }
        return mainGridAPI.retrieveData(query);
    }
    function load() {
        loadDurationType();
       
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


    function loadDurationType() {
        $scope.durationType = [];
        for (var prop in DurationEnum) {
            $scope.durationType.push(DurationEnum[prop]);
        }
        
        $scope.selectedDurationType = $scope.durationType[1];
    }


};



appControllers.controller('Analytics_RawCDRLogController', RawCDRLogController);