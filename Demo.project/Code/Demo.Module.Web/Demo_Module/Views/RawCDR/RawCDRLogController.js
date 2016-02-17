rawCDRLogController.$inject = ['$scope', 'UtilsService',  'VRNotificationService', 'VRUIUtilsService'];

function rawCDRLogController($scope, UtilsService,  VRNotificationService, VRUIUtilsService) {
    var mainGridAPI;
    var CDROption = [];
    var isFilterScreenReady;

    var dataSourceDirectiveAPI;
    var dataSourceReadyPromiseDeferred = UtilsService.createPromiseDeferred();

    var directionDirectiveAPI;
    var directionReadyPromiseDeferred = UtilsService.createPromiseDeferred();

    var serviceTypeDirectiveAPI;
    var serviceTypeReadyPromiseDeferred = UtilsService.createPromiseDeferred();

    defineScope();
    load();

    function defineScope() {
        $scope.fromDate = '2015/06/02';
        $scope.toDate = '2016/06/06';
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
        $scope.onCDRDirectionReady = function (api) {
            directionDirectiveAPI = api;
            directionReadyPromiseDeferred.resolve();
        }
        $scope.onServiceTypeReady = function (api) {
            serviceTypeDirectiveAPI = api;
            serviceTypeReadyPromiseDeferred.resolve();
        }
        
        $scope.getData = function () {
            return mainGridAPI.loadGrid(getQuery());
        };

    }

    function getQuery() {
        var filter = buildFilter();
        var query = {
            FromDate: $scope.fromDate,
            ToDate:  $scope.toDate,
            NumberRecords: $scope.nRecords,           
            CDPN: $scope.cdpn,
            CGPN: $scope.cgpn,
            MinDuration: $scope.minDuration,
            MaxDuration: $scope.maxDuration,
            DataSourceIds: dataSourceDirectiveAPI.getSelectedIds(),
            Directions: directionDirectiveAPI.getSelectedIds(),
            ServiceTypes: serviceTypeDirectiveAPI.getSelectedIds()

        }
        return query;
       
    }

    function load() {
        $scope.isLoading = true;
        loadAllControls();
    }

    function loadAllControls() {
        return UtilsService.waitMultipleAsyncOperations([loadDataSources, loadDirection, loadServiceTypes])
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
    function loadDirection() {
        var loadDirectionPromiseDeferred = UtilsService.createPromiseDeferred();
        directionReadyPromiseDeferred.promise.then(function () {

            VRUIUtilsService.callDirectiveLoad(directionDirectiveAPI, undefined, loadDirectionPromiseDeferred);
        });
        return loadDirectionPromiseDeferred.promise;
    }

    function loadServiceTypes() {
        var loadServiceTypesPromiseDeferred = UtilsService.createPromiseDeferred();
        serviceTypeReadyPromiseDeferred.promise.then(function () {

            VRUIUtilsService.callDirectiveLoad(serviceTypeDirectiveAPI, undefined, loadServiceTypesPromiseDeferred);
        });
        return loadServiceTypesPromiseDeferred.promise;
    }
    function buildFilter() {
        var filter = {};
        return filter;
    }

};

appControllers.controller('RawCDRLogController', rawCDRLogController);