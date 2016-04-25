'use strict'
AnalyticTableManagementController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRUIUtilsService', 'VRModalService'];

function AnalyticTableManagementController($scope, UtilsService, VRNotificationService, VRUIUtilsService, VRModalService) {
    var mainGridAPI;
    defineScope();
    load();

    function defineScope() {
        $scope.onGridReady = function (api) {
            mainGridAPI = api;
            var filter = {};
            api.loadGrid(filter);
        };

        $scope.searchClicked = function () {
            return mainGridAPI.loadGrid(getFilterObject());
        }

    }

    function getFilterObject() {
        var query = {
            Name: $scope.viewName,
        }
        return query;
    }

    function load() {
        $scope.isLoading = true;
        loadAllControls();
    }

    function loadAllControls() {
        return UtilsService.waitMultipleAsyncOperations([])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.isLoading = false;
              });
    }

};

appControllers.controller('VR_Analytic_AnalyticTableManagementController', AnalyticTableManagementController);