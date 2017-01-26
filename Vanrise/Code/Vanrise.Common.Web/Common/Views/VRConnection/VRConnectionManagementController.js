(function (app) {

    "use strict";

    VRConnectionManagementController.$inject = ['$scope', 'VRCommon_VRConnectionAPIService', 'VRCommon_VRConnectionService', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService'];

    function VRConnectionManagementController($scope, VRCommon_VRConnectionAPIService, VRCommon_VRConnectionService, UtilsService, VRUIUtilsService, VRNotificationService) {

        var gridAPI;


        defineScope();
        load();


        function defineScope() {

            $scope.scopeModel = {};
            $scope.scopeModel.selectedComponentType;

            $scope.scopeModel.search = function () {
                loadGrid(buildGridQuery());
            };

           
           
            $scope.scopeModel.add = function () {
                var onVRConnectionAdded = function (addedItem) {
                    gridAPI.onVRConnectionAdded(addedItem);
                };
                VRCommon_VRConnectionService.addVRConnection(onVRConnectionAdded);
            };

            $scope.scopeModel.hasAddVRConnectionPermission = function () {
                return VRCommon_VRConnectionAPIService.HasAddVRConnectionPermission();
            };

            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
                gridAPI.load(buildGridQuery());
            };
        }

        function load() {
            $scope.scopeModel.isLoading = true;
            //return UtilsService.waitMultipleAsyncOperations([loadVRConnectionsTypw]).catch(function (error) {
            //    VRNotificationService.notifyExceptionWithClose(error, $scope);
            //}).finally(function () {
            //    $scope.scopeModel.isLoading = false;
            //});
        }

        function loadGrid() {
            if (gridAPI != undefined) {
                var query = buildGridQuery();
                $scope.scopeModel.isLoading = true;
                return gridAPI.load(query).finally(function () {
                    $scope.scopeModel.isLoading = false;
                });
            }
        }
        //function loadVRConnections() {
        //    var loadComponentTypesPromiseDeferred = UtilsService.createPromiseDeferred();
        //    componentTypeSelectorReadyDeferred.promise.then(function () {
        //        var payloadDirective;
        //        VRUIUtilsService.callDirectiveLoad(componentTypeSelectorAPI, payloadDirective, loadComponentTypesPromiseDeferred);
        //    });
        //    return loadComponentTypesPromiseDeferred.promise;
        //}

        function buildGridQuery() {
            return {
                Name: $scope.scopeModel.name//,
                //ExtensionConfigId: $scope.scopeModel.selectedComponentType == undefined ? undefined : $scope.scopeModel.selectedComponentType.ExtensionConfigurationId
            };
        }
    }

    app.controller('VRCommon_VRConnectionManagementController', VRConnectionManagementController);

})(app);