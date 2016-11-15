"use strict";
rawCDRLogController.$inject = ['$scope', 'UtilsService', '$q', 'VRModalService', 'VRNotificationService', 'WhS_Analytics_DurationEnum', 'VRUIUtilsService'];

function rawCDRLogController($scope, UtilsService, $q, VRModalService, VRNotificationService, WhS_Analytics_DurationEnum, VRUIUtilsService) {
    var mainGridAPI;
    var CDROption = [];
    var isFilterScreenReady;

    var switchDirectiveAPI;
    var switchReadyPromiseDeferred = UtilsService.createPromiseDeferred();

    defineScope();
    load();

    function defineScope() {
        $scope.fromDate = '2015/06/02';
        $scope.toDate = '2015/06/06';
        $scope.inCarrier;
        $scope.outCarrier;
        $scope.inCDPN;
        $scope.outCDPN;
        $scope.cgpn;
        $scope.minDuration;
        $scope.maxDuration;
        $scope.whereCondtion;
        $scope.nRecords = '100';
     
        $scope.onSwitchDirectiveReady = function (api) {
            switchDirectiveAPI = api;
            switchReadyPromiseDeferred.resolve();
        };

        $scope.onInfoClick = function () {
            var settings = {};
            settings.onScopeReady = function (modalScope) {
                modalScope.title = "CDR Table Definition ";
            };
            VRModalService.showModal('/Client/Modules/WhS_Analytics/Views/RawCDR/RawCDRLogTemplate.html', null, settings);

        };

        $scope.onMainGridReady = function (api) {
            mainGridAPI = api;
        };

        $scope.getData = function () {
            return mainGridAPI.loadGrid(getQuery());
        };

    }

    function getQuery() {
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
            DurationType: $scope.selectedDurationType.value,
            WhereCondition: $scope.whereCondtion
        };
        return query;
       
    }

    function load() {
        $scope.isLoading = true;
        loadAllControls();
    }

    function loadAllControls() {
        return UtilsService.waitMultipleAsyncOperations([loadDurationType, loadSwitches])
          .catch(function (error) {
              VRNotificationService.notifyExceptionWithClose(error, $scope);
              $scope.isLoading = false;
            })
            .finally(function () {
                $scope.isLoading = false;
            });
    }

    function buildFilter() {
        var filter = {};
        filter.SwitchIds = switchDirectiveAPI.getSelectedIds();
        return filter;
    }

    function loadSwitches() {
        var loadSwitchPromiseDeferred = UtilsService.createPromiseDeferred();
        switchReadyPromiseDeferred.promise.then(function () {

            VRUIUtilsService.callDirectiveLoad(switchDirectiveAPI, undefined, loadSwitchPromiseDeferred);
        });
        return loadSwitchPromiseDeferred.promise;
    }

    function loadDurationType() {
        $scope.durationType = [];
        for (var prop in WhS_Analytics_DurationEnum) {
            $scope.durationType.push(WhS_Analytics_DurationEnum[prop]);
        }
        
        $scope.selectedDurationType = $scope.durationType[1];
    }


};

appControllers.controller('WhS_Analytics_RawCDRLogController', rawCDRLogController);