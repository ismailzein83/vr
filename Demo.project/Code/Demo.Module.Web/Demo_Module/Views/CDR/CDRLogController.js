cdrLogController.$inject = ['$scope', 'UtilsService',  'VRNotificationService', 'VRUIUtilsService'];

function cdrLogController($scope, UtilsService, VRNotificationService, VRUIUtilsService) {
    var mainGridAPI;
    var CDROption = [];
    var isFilterScreenReady;

    var dataSourceDirectiveAPI;
    var dataSourceReadyPromiseDeferred = UtilsService.createPromiseDeferred();

    var directionDirectiveAPI;
    var directionReadyPromiseDeferred = UtilsService.createPromiseDeferred();

    var serviceTypeDirectiveAPI;
    var serviceTypeReadyPromiseDeferred = UtilsService.createPromiseDeferred();

    var cdrTypeDirectiveAPI;
    var cdrTypeReadyPromiseDeferred = UtilsService.createPromiseDeferred();

    var operatorDirectiveAPI;
    var operatorReadyPromiseDeferred = UtilsService.createPromiseDeferred();
    
    var saleZoneDirectiveAPI;
    var saleZoneReadyPromiseDeferred = UtilsService.createPromiseDeferred();

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
        $scope.onCDRTypeReady = function (api) {
            cdrTypeDirectiveAPI = api;
            cdrTypeReadyPromiseDeferred.resolve();
        }
        $scope.onOperatorAccountReady = function (api) {
            operatorDirectiveAPI = api;
            operatorReadyPromiseDeferred.resolve();
        }
        $scope.onSaleZoneDirectiveReady = function (api) {
            saleZoneDirectiveAPI = api;
            saleZoneReadyPromiseDeferred.resolve();
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
            ServiceTypes: serviceTypeDirectiveAPI.getSelectedIds(),
            CDRTypes: cdrTypeDirectiveAPI.getSelectedIds(),
            OperatorIds: operatorDirectiveAPI.getSelectedIds(),
            ZoneIds: saleZoneDirectiveAPI.getSelectedIds()

        }
        return query;
       
    }

    function load() {
        $scope.isLoading = true;
        loadAllControls();
    }

    function loadAllControls() {
        return UtilsService.waitMultipleAsyncOperations([loadDataSources, loadDirection, loadServiceTypes, loadCdrTypes, loadOperator])
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
    function loadCdrTypes() {
        var loadCdrTypesPromiseDeferred = UtilsService.createPromiseDeferred();
        cdrTypeReadyPromiseDeferred.promise.then(function () {

            VRUIUtilsService.callDirectiveLoad(cdrTypeDirectiveAPI, undefined, loadCdrTypesPromiseDeferred);
        });
        return loadCdrTypesPromiseDeferred.promise;
    }

    function loadOperator() {
        var loadOperatorPromiseDeferred = UtilsService.createPromiseDeferred();
        operatorReadyPromiseDeferred.promise.then(function () {

            VRUIUtilsService.callDirectiveLoad(operatorDirectiveAPI, undefined, loadOperatorPromiseDeferred);
        });
        return loadOperatorPromiseDeferred.promise;
    }

    
    function buildFilter() {
        var filter = {};
        return filter;
    }

};

appControllers.controller('CDRLogController', cdrLogController);