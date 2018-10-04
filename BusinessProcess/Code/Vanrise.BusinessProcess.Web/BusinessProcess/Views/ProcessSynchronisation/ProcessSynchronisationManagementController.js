(function (appControllers) {

    "use strict";

    ProcessSynchronisationManagementController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'BusinessProcess_ProcessSynchronisationService', 'BusinessProcess_ProcessSynchronisationAPIService'];

    function ProcessSynchronisationManagementController($scope, UtilsService, VRUIUtilsService, BusinessProcess_ProcessSynchronisationService, BusinessProcess_ProcessSynchronisationAPIService) {


        var gridAPI;

        var processSynchronisationStatusSelectorAPI;
        var processSynchronisationStatusSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        defineScope();
        load();

        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
                gridAPI.loadGrid(getFilterObject());
            };

            $scope.scopeModel.onProcessSynchronisationStatusSelectorReady = function (api) {
                processSynchronisationStatusSelectorAPI = api;
                processSynchronisationStatusSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.searchClicked = function () {
                return gridAPI.loadGrid(getFilterObject());
            };

            $scope.scopeModel.addClicked = function () {
                var onProcessSynchronisationAdded = function (addedProcessSynchronisation) {
                    gridAPI.onProcessSynchronisationAdded(addedProcessSynchronisation);
                };

                BusinessProcess_ProcessSynchronisationService.addProcessSynchronisation(onProcessSynchronisationAdded);
            };

            $scope.scopeModel.hasAddProcessSynchronisationPermission = function () {
                return BusinessProcess_ProcessSynchronisationAPIService.HasAddProcessSynchronisationPermission();
            };
        }

        function load() {
            var promises = [];

            promises.push(loadProcessSynchronisationStatusSelector());

            function loadProcessSynchronisationStatusSelector() {
                var processSynchronisationStatusSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                processSynchronisationStatusSelectorReadyDeferred.promise.then(function () {
                    VRUIUtilsService.callDirectiveLoad(processSynchronisationStatusSelectorAPI, undefined, processSynchronisationStatusSelectorLoadDeferred);
                });
                return processSynchronisationStatusSelectorLoadDeferred.promise;
            }

            return UtilsService.waitMultiplePromises(promises);
        }

        function getFilterObject() {
            return {
                Name: $scope.scopeModel.name,
                Statuses: processSynchronisationStatusSelectorAPI.getSelectedIds()
            };
        }
    }

    appControllers.controller('BusinessProcess_ProcessSynchronisationManagementController', ProcessSynchronisationManagementController);
})(appControllers);