rawCDRLogController.$inject = ['$scope', 'UtilsService',  'VRNotificationService', 'VRUIUtilsService'];

function rawCDRLogController($scope, UtilsService,  VRNotificationService, VRUIUtilsService) {
    var mainGridAPI;
    var CDROption = [];
    var isFilterScreenReady;

    var dataSourceDirectiveAPI;
    var dataSourceReadyPromiseDeferred = UtilsService.createPromiseDeferred();

    defineScope();
    load();

    function defineScope() {
        $scope.fromDate = '2015/06/02';
        $scope.toDate = '2015/06/06';
        $scope.inCarrier;
        $scope.outCarrier;
        $scope.cdpn;
        $scope.cgpn;
        $scope.minDuration;
        $scope.maxDuration;
        $scope.whereCondtion;
        $scope.nRecords = '100'

        $scope.onMainGridReady = function (api) {
            mainGridAPI = api;
        }
        $scope.onDataSourceDirectiveReady = function (api) {
            dataSourceDirectiveAPI = api;
            dataSourceReadyPromiseDeferred.resolve();
        }

        $scope.getData = function () {
            return mainGridAPI.loadGrid(getQuery());
        };

    }

    function getQuery() {
        var filter = buildFilter();
        var query = {
            Switches: filter.SwitchIds,
            FromDate: $scope.fromDate,
            ToDate:  $scope.toDate,
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
        }
        return query;
       
    }

    function load() {
        $scope.isLoading = true;
        loadAllControls();
    }

    function loadAllControls() {
        return UtilsService.waitMultipleAsyncOperations([loadDataSources])
          .catch(function (error) {
              VRNotificationService.notifyExceptionWithClose(error, $scope);
              $scope.isLoading = false;
            })
            .finally(function () {
                $scope.isLoading = false;
            });
    }
    function loadDataSources() {
        var loadDataSourcePromiseDeferred = UtilsService.createPromiseDeferred();
        dataSourceReadyPromiseDeferred.promise.then(function () {

            VRUIUtilsService.callDirectiveLoad(dataSourceDirectiveAPI, undefined, loadDataSourcePromiseDeferred);
        });
        return loadDataSourcePromiseDeferred.promise;
    }
    function buildFilter() {
        var filter = {};
        filter.SwitchIds = switchDirectiveAPI.getSelectedIds();
        return filter;
    }

};

appControllers.controller('RawCDRLogController', rawCDRLogController);