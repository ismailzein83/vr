(function (appControllers) {

    'use strict';

    SwitchConnectivityManagementController.$inject = ['$scope', 'WhS_BE_SwitchConnectivityService', 'WhS_BE_SwitchConnectivityTypeEnum', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService'];

    function SwitchConnectivityManagementController($scope, WhS_BE_SwitchConnectivityService, WhS_BE_SwitchConnectivityTypeEnum, UtilsService, VRUIUtilsService, VRNotificationService) {
        var carrierAccountSelectorAPI;
        var carrierAccountSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var switchSelectorAPI;
        var switchSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var gridAPI;

        defineScope();
        load();

        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.connectionTypes = UtilsService.getArrayEnum(WhS_BE_SwitchConnectivityTypeEnum);
            $scope.scopeModel.selectedConnectionTypes = [];

            $scope.scopeModel.onCarrierAccountSelectorReady = function (api) {
                carrierAccountSelectorAPI = api;
                carrierAccountSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.onSwitchSelectorReady = function (api) {
                switchSelectorAPI = api;
                switchSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
                api.load({});
            };

            $scope.scopeModel.search = function () {
                return gridAPI.load(buildGridQuery());
            };

            $scope.scopeModel.add = function () {
                var onSwitchConnectivityAdded = function (addedSwitchConnectivity) {
                    gridAPI.onSwitchConnectivityAdded(addedSwitchConnectivity);
                };
                WhS_BE_SwitchConnectivityService.addSwitchConnectivity(onSwitchConnectivityAdded);
            };
        }

        function load() {
            $scope.scopeModel.isLoading = true;
            loadAllControls();
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadCarrierAccountSelector, loadSwitchSelector]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function loadCarrierAccountSelector() {
            var carrierAccountSelectorLoadDeferred = UtilsService.createPromiseDeferred();

            carrierAccountSelectorReadyDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(carrierAccountSelectorAPI, undefined, carrierAccountSelectorLoadDeferred);
            });

            return carrierAccountSelectorLoadDeferred.promise;
        }

        function loadSwitchSelector() {
            var switchSelectorLoadDeferred = UtilsService.createPromiseDeferred();

            switchSelectorReadyDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(switchSelectorAPI, undefined, switchSelectorLoadDeferred);
            });

            return switchSelectorLoadDeferred.promise;
        }

        function buildGridQuery() {
            return {
                Name: $scope.scopeModel.name,
                CarrierAccountIds: carrierAccountSelectorAPI.getSelectedIds(),
                SwitchIds: switchSelectorAPI.getSelectedIds(),
                ConnectionTypes: ($scope.scopeModel.selectedConnectionTypes.length > 0) ? UtilsService.getPropValuesFromArray($scope.scopeModel.selectedConnectionTypes, 'value') : null
            };
        }
    }

    appControllers.controller('WhS_BE_SwitchConnectivityManagementController', SwitchConnectivityManagementController);

})(appControllers);