(function (appControllers) {

    "use strict";

    MailMessageTypeManagementController.$inject = ['$scope', 'VRCommon_VRMailMessageTypeAPIService', 'VRCommon_VRMailMessageTypeService', 'UtilsService', 'VRUIUtilsService'];

    function MailMessageTypeManagementController($scope, VRCommon_VRMailMessageTypeAPIService, VRCommon_VRMailMessageTypeService, UtilsService, VRUIUtilsService) {

        var gridAPI;
        var devProjectDirectiveApi;
        var devProjectPromiseReadyDeferred = UtilsService.createPromiseDeferred();

        defineScope();
        load();


        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.search = function () {
                var query = buildGridQuery();
                return gridAPI.load(query);
            };
            $scope.scopeModel.add = function () {
                var onMailMessageTypeAdded = function (addedMailMessageType) {
                    gridAPI.onMailMessageTypeAdded(addedMailMessageType);
                };
                VRCommon_VRMailMessageTypeService.addMailMessageType(onMailMessageTypeAdded);
            };

            $scope.scopeModel.hasAddMailMessageTypePermission = function () {
                return VRCommon_VRMailMessageTypeAPIService.HasAddMailMessageTypePermission();
            };

            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
                gridAPI.load({});
            };
            $scope.onDevProjectSelectorReady = function (api) {
                devProjectDirectiveApi = api;
                devProjectPromiseReadyDeferred.resolve();
            };
        }

        function load() {
            $scope.isLoading = true;
            loadAllControls();
        }
        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadDevProjectSelector])
                .catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                })
                .finally(function () {
                    $scope.isLoading = false;
                });
        }
        function loadDevProjectSelector() {
            var devProjectPromiseLoadDeferred = UtilsService.createPromiseDeferred();
            devProjectPromiseReadyDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(devProjectDirectiveApi, undefined, devProjectPromiseLoadDeferred);
            });
            return devProjectPromiseLoadDeferred.promise;
        }

        function buildGridQuery() {
            return {
                Name: $scope.scopeModel.name,
                DevProjectIds: devProjectDirectiveApi.getSelectedIds()
            };
        }
    }

    appControllers.controller('VRCommon_VRMailMessageTypeManagementController', MailMessageTypeManagementController);

})(appControllers);