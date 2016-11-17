(function (appControllers) {

    "use strict";

    loggerManagementController.$inject = ['$scope', 'VRCommon_LogAttributeEnum', 'VRNotificationService', 'UtilsService', 'VRUIUtilsService', 'VRValidationService'];

    function loggerManagementController($scope, VRCommon_LogAttributeEnum, VRNotificationService, UtilsService, VRUIUtilsService, VRValidationService) {
        var gridAPI;
        var filter = {};

        var entryTypeDirectiveApi;
        var entryTypeReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var machineDirectiveApi;
        var machineReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var applicationDirectiveApi;
        var applicationReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var assemblyDirectiveApi;
        var assemblyReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var typeDirectiveApi;
        var typeReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var methodDirectiveApi;
        var methodReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        defineScope();
        load();

        function defineScope() {
            $scope.searchClicked = function () {
                if (gridAPI != undefined) {
                    setFilterObject();
                    return gridAPI.loadGrid(filter);
                }
            };

            $scope.onEntryTypeDirectiveReady = function (api) {
                entryTypeDirectiveApi = api;
                entryTypeReadyPromiseDeferred.resolve();
            };

            $scope.onMachineDirectiveReady = function (api) {
                machineDirectiveApi = api;
                machineReadyPromiseDeferred.resolve();
            };

            $scope.onApplicationDirectiveReady = function (api) {
                applicationDirectiveApi = api;
                applicationReadyPromiseDeferred.resolve();
            };

            $scope.onAssemblyDirectiveReady = function (api) {
                assemblyDirectiveApi = api;
                assemblyReadyPromiseDeferred.resolve();
            };

            $scope.onTypeDirectiveReady = function (api) {
                typeDirectiveApi = api;
                typeReadyPromiseDeferred.resolve();
            };

            $scope.onMethodDirectiveReady = function (api) {
                methodDirectiveApi = api;
                methodReadyPromiseDeferred.resolve();
            };
            $scope.validateDateTime = function () {
                return VRValidationService.validateTimeRange($scope.fromDate, $scope.toDate);
            };
            $scope.onGridReady = function (api) {
                gridAPI = api;
                api.loadGrid(filter);
            };
        }

        function load() {
            $scope.isLoadingFilters = true;
            loadAllControls();
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadEntryTypeSelector, loadMachineSelector, loadApplicationSelector, loadAssemblySelector, loadTypeSelector, loadMethodSelector])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.isLoadingFilters = false;
              });
        }
        function loadEntryTypeSelector() {
            var logEntryTypeLoadPromiseDeferred = UtilsService.createPromiseDeferred();
            
            entryTypeReadyPromiseDeferred.promise
                .then(function () {
                    VRUIUtilsService.callDirectiveLoad(entryTypeDirectiveApi, undefined, logEntryTypeLoadPromiseDeferred);
                });
            return logEntryTypeLoadPromiseDeferred.promise;
        }

        function loadMachineSelector() {
            var machineLoadPromiseDeferred = UtilsService.createPromiseDeferred();
            var payload = {
                attribute: VRCommon_LogAttributeEnum.MachineName.value
            };
            machineReadyPromiseDeferred.promise
                .then(function () {
                    VRUIUtilsService.callDirectiveLoad(machineDirectiveApi, payload, machineLoadPromiseDeferred);
                });
            return machineLoadPromiseDeferred.promise;
        }

        function loadApplicationSelector() {
            var applicationLoadPromiseDeferred = UtilsService.createPromiseDeferred();
            var payload = {
                attribute: VRCommon_LogAttributeEnum.ApplicationName.value
            };
            applicationReadyPromiseDeferred.promise
                .then(function () {
                    VRUIUtilsService.callDirectiveLoad(applicationDirectiveApi, payload, applicationLoadPromiseDeferred);
                });
            return applicationLoadPromiseDeferred.promise;
        }

        function loadAssemblySelector() {
            var assemblyLoadPromiseDeferred = UtilsService.createPromiseDeferred();
            var payload = {
                attribute: VRCommon_LogAttributeEnum.AssemblyName.value
            };
            assemblyReadyPromiseDeferred.promise
                .then(function () {
                    VRUIUtilsService.callDirectiveLoad(assemblyDirectiveApi, payload, assemblyLoadPromiseDeferred);
                });
            return assemblyLoadPromiseDeferred.promise;
        }

        function loadTypeSelector() {
            var typeLoadPromiseDeferred = UtilsService.createPromiseDeferred();
            var payload = {
                attribute: VRCommon_LogAttributeEnum.TypeName.value
            };
            typeReadyPromiseDeferred.promise
                .then(function () {
                    VRUIUtilsService.callDirectiveLoad(typeDirectiveApi, payload, typeLoadPromiseDeferred);
                });
            return typeLoadPromiseDeferred.promise;
        }

        function loadMethodSelector() {
            var methodLoadPromiseDeferred = UtilsService.createPromiseDeferred();
            var payload = {
                attribute: VRCommon_LogAttributeEnum.MethodName.value
            };
            methodReadyPromiseDeferred.promise
                .then(function () {
                    VRUIUtilsService.callDirectiveLoad(methodDirectiveApi, payload, methodLoadPromiseDeferred);
                });
            return methodLoadPromiseDeferred.promise;
        }

        function setFilterObject() {
            filter = {
                MachineIds: machineDirectiveApi.getSelectedIds(),
                ApplicationIds: applicationDirectiveApi.getSelectedIds(),
                TypeIds: typeDirectiveApi.getSelectedIds(),
                MethodIds: methodDirectiveApi.getSelectedIds(),
                AssemblyIds: assemblyDirectiveApi.getSelectedIds(),
                EntryType: entryTypeDirectiveApi.getSelectedIds(),
                Message : $scope.Message,
                FromDate: $scope.fromDate,
                ToDate: $scope.toDate
            };
        }

    }

    appControllers.controller('VRCommon_LoggerManagementController', loggerManagementController);
})(appControllers);