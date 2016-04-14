"use strict";

app.directive("vrLogEntrySearch", ['VRCommon_LogAttributeEnum', 'VRNotificationService', 'UtilsService', 'VRUIUtilsService',
function ( VRCommon_LogAttributeEnum , VRNotificationService,UtilsService, VRUIUtilsService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var logSearch = new LogSearch($scope, ctrl, $attrs);
            logSearch.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/Common/Directives/Logger/Templates/LoggerSearch.html"

    };

    function LogSearch($scope, ctrl, $attrs) {


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


        this.initializeController = initializeController;
        function initializeController() {
            
            defineScope();

            if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                ctrl.onReady(getDirectiveAPI());
            function getDirectiveAPI() {

                var directiveAPI = {};
                directiveAPI.load = function () {
                    return load();
                }
                return directiveAPI;
            }

        }

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
            }

            $scope.onMachineDirectiveReady = function (api) {
                machineDirectiveApi = api;
                machineReadyPromiseDeferred.resolve();
            }

            $scope.onApplicationDirectiveReady = function (api) {
                applicationDirectiveApi = api;
                applicationReadyPromiseDeferred.resolve();
            }

            $scope.onAssemblyDirectiveReady = function (api) {
                assemblyDirectiveApi = api;
                assemblyReadyPromiseDeferred.resolve();
            }

            $scope.onTypeDirectiveReady = function (api) {
                typeDirectiveApi = api;
                typeReadyPromiseDeferred.resolve();
            }

            $scope.onMethodDirectiveReady = function (api) {
                methodDirectiveApi = api;
                methodReadyPromiseDeferred.resolve();
            }

            $scope.onGridReady = function (api) {
                gridAPI = api;
                api.loadGrid(filter);
            }

        }

        function load() {
            return loadAllControls();
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadMachineSelector, loadApplicationSelector, loadAssemblySelector, loadTypeSelector, loadMethodSelector])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  //  $scope.isLoadingFilters = false;
              });
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
                //SupplierIds: supplierDirectiveApi.getSelectedIds(),
                MachineIds: machineDirectiveApi.getSelectedIds(),
                ApplicationIds: applicationDirectiveApi.getSelectedIds(),
                TypeIds: typeDirectiveApi.getSelectedIds(),
                MethodIds: methodDirectiveApi.getSelectedIds(),
                AssemblyIds: assemblyDirectiveApi.getSelectedIds(),
                EntryType: entryTypeDirectiveApi.getSelectedIds(),
                Message: $scope.Message,
                FromDate: $scope.fromDate,
                ToDate: $scope.toDate
            };
        }


    }

    return directiveDefinitionObject;

}]);
