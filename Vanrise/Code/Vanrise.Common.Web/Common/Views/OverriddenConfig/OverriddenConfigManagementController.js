(function (appControllers) {

    "use strict";

    overriddenConfigManagementController.$inject = ['$scope', 'VRCommon_OverriddenConfigService', 'UtilsService', 'VRUIUtilsService'];

    function overriddenConfigManagementController($scope, VRCommon_OverriddenConfigService, UtilsService, VRUIUtilsService) {
        var gridAPI;
        var filter = {};

        var overriddenConfigGroupSelectorApi;
        var overriddenConfigGroupSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        defineScope();
        load();

        function defineScope() {
            $scope.searchClicked = function () {
                getFilterObject();
                return gridAPI.loadGrid(filter);
            };
            $scope.onOverrinddenConfigGroupSelectorReady = function (api) {
                overriddenConfigGroupSelectorApi = api;
                overriddenConfigGroupSelectorReadyPromiseDeferred.resolve();
            };

            function getFilterObject() {
                filter = {
                    Name: $scope.name,
                    OverriddenConfigGroupIds: overriddenConfigGroupSelectorApi.getSelectedIds()
                };

            }

            $scope.onGridReady = function (api) {
                gridAPI = api;
                api.loadGrid(filter);
            };
            $scope.addOverriddenConfig = addOverriddenConfig;
        }

        function load() {
            $scope.isLoadingFilters = true;
            loadAllControls();
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadOverrinddenConfigGroupSelector])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.isLoadingFilters = false;
              });
        }
        function loadOverrinddenConfigGroupSelector() {
            var OverrinddenConfigGroupSelectorLoadPromiseDeferred = UtilsService.createPromiseDeferred();

            overriddenConfigGroupSelectorReadyPromiseDeferred.promise
                .then(function () {
                    VRUIUtilsService.callDirectiveLoad(overriddenConfigGroupSelectorApi, undefined, OverrinddenConfigGroupSelectorLoadPromiseDeferred);
                });
            return OverrinddenConfigGroupSelectorLoadPromiseDeferred.promise;
        }


        function addOverriddenConfig() {
            var onOverriddenConfigAdded = function (OverriddenConfigObj) {
                gridAPI.onOverriddenConfigAdded(OverriddenConfigObj);
            };

            VRCommon_OverriddenConfigService.addOverriddenConfig(onOverriddenConfigAdded);
        }

    }

    appControllers.controller('VRCommon_OveriddenConfigManagementController', overriddenConfigManagementController);
})(appControllers);