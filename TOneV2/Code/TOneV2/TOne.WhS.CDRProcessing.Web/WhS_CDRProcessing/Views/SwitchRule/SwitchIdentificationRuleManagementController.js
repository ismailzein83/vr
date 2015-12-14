(function (appControllers) {

    "use strict";

    switchIdentificationRuleManagementController.$inject = ['$scope', 'WhS_CDRProcessing_MainService', 'UtilsService', 'VRNotificationService', 'VRUIUtilsService'];

    function switchIdentificationRuleManagementController($scope, WhS_CDRProcessing_MainService, UtilsService, VRNotificationService, VRUIUtilsService) {
        var gridAPI;
        var switchDirectiveAPI;
        var switchReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var dataSourceDirectiveAPI;
        var dataSourceReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        defineScope();
        load();

        function defineScope() {

            $scope.searchClicked = function () {
                if (!$scope.isGettingData && gridAPI != undefined)
                    return gridAPI.loadGrid(getFilterObject());
            };

            $scope.onSwitchDirectiveReady = function (api) {
                switchDirectiveAPI = api;
                switchReadyPromiseDeferred.resolve();
            }

            $scope.onDataSourceDirectiveReady = function (api) {
                dataSourceDirectiveAPI = api;
                dataSourceReadyPromiseDeferred.resolve();
            }
            $scope.onGridReady = function (api) {
                gridAPI = api;
                var filter = {};
                api.loadGrid(filter);
            }
            $scope.name;
            $scope.AddNewSwitchRule = AddNewSwitchRule;
        }

        function load() {
            $scope.isLoadingFilterData = true;

            return UtilsService.waitMultipleAsyncOperations([ loadSwitches, loadDataSources])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
                   $scope.isLoadingFilterData = false;
               })
              .finally(function () {
                  $scope.isLoadingFilterData = false;
              });
        }

        function loadSwitches() {
            var loadSwitchPromiseDeferred = UtilsService.createPromiseDeferred();
            switchReadyPromiseDeferred.promise.then(function () {

                VRUIUtilsService.callDirectiveLoad(switchDirectiveAPI, undefined, loadSwitchPromiseDeferred);
            });
            return loadSwitchPromiseDeferred.promise;
        }

        function loadDataSources() {
            var loadDataSourcePromiseDeferred = UtilsService.createPromiseDeferred();
            dataSourceReadyPromiseDeferred.promise.then(function () {

                VRUIUtilsService.callDirectiveLoad(dataSourceDirectiveAPI, undefined, loadDataSourcePromiseDeferred);
            });
            return loadDataSourcePromiseDeferred.promise;
        }

        function getFilterObject() {
            var data = {
                Description: $scope.description,
                SwitchIds: switchDirectiveAPI.getSelectedIds(),
                DataSourceIds: dataSourceDirectiveAPI.getSelectedIds(),
                EffectiveDate: $scope.effectiveDate
            };
            return data;
        }

        function AddNewSwitchRule() {
            var onSwitchIdentificationRuleAdded = function (switchRuleObj) {
                if (gridAPI != undefined)
                    gridAPI.onSwitchIdentificationRuleAdded(switchRuleObj);
            };

            WhS_CDRProcessing_MainService.addSwitchIdentificationRule(onSwitchIdentificationRuleAdded);
        }
    }

    appControllers.controller('WhS_CDRProcessing_SwitchIdentificationRuleManagementController', switchIdentificationRuleManagementController);
})(appControllers);