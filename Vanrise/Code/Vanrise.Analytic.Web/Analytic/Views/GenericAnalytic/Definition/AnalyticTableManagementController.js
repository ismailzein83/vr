'use strict';
AnalyticTableManagementController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRUIUtilsService', 'VRModalService', 'VR_Analytic_AnalyticTableService', 'VR_Analytic_AnalyticTableAPIService'];

function AnalyticTableManagementController($scope, UtilsService, VRNotificationService, VRUIUtilsService, VRModalService, VR_Analytic_AnalyticTableService, VR_Analytic_AnalyticTableAPIService) {
    var mainGridAPI;

    var devProjectDirectiveApi;
    var devProjectPromiseReadyDeferred = UtilsService.createPromiseDeferred();

    defineScope();
    load();

    function defineScope() {
        $scope.onGridReady = function (api) {
            mainGridAPI = api;
            var filter = {};
            api.loadGrid(filter);
        };
        $scope.hasAddAnalyticTablePermission = function () {
            return VR_Analytic_AnalyticTableAPIService.HasAddAnalyticTablePermission();
        };
        $scope.addTable = function () {
            var onAnalyticTableAdded = function (tableObj) {
                mainGridAPI.onAnalyticTableAdded(tableObj);
            };

            VR_Analytic_AnalyticTableService.addAnalyticTable(onAnalyticTableAdded);
        };
        $scope.searchClicked = function () {
            return mainGridAPI.loadGrid(getFilterObject());
        };
        $scope.onDevProjectSelectorReady = function (api) {
            devProjectDirectiveApi = api;
            devProjectPromiseReadyDeferred.resolve();
        };
    }

    function getFilterObject() {
        var query = {
            Name: $scope.tableName,
            DevProjectIds: devProjectDirectiveApi.getSelectedIds()
        };
        return query;
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

};

appControllers.controller('VR_Analytic_AnalyticTableManagementController', AnalyticTableManagementController);