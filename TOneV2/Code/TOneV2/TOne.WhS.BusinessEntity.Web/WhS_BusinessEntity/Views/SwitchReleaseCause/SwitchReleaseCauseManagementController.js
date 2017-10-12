(function (appControllers) {
    "use strict";
    switchReleaseCauseManagementController.$inject = ["$scope", "UtilsService", "VRUIUtilsService", "VRNotificationService", "WhS_BE_SwitchReleaseCauseAPIService", "WhS_BE_SwitchReleaseCauseService"];

    function switchReleaseCauseManagementController($scope, UtilsService, VRUIUtilsService, VRNotificationService, WhS_BE_SwitchReleaseCauseAPIService, WhS_BE_SwitchReleaseCauseService) {
        var gridAPI;
        var switchSelectorAPI;
        var switchReleaseCauseEntity;
        var switchSelectorReadyDeferred = UtilsService.createPromiseDeferred();
        loadParameters();
        defineScope();
        load();
        function loadParameters() {

        };
        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
                gridAPI.loadGrid(getGridFilter());
            };

            $scope.scopeModel.addNewSwitchReleaseCause = function () {
                var onSwitchReleaseCauseAdded = function (addedItem) {
                    gridAPI.onSwitchReleaseCauseAdded(addedItem);
                };
                WhS_BE_SwitchReleaseCauseService.addSwitchReleaseCause(onSwitchReleaseCauseAdded);
            };

            $scope.scopeModel.searchClicked = function () {
                gridAPI.loadGrid(getGridFilter());
            };
            $scope.scopeModel.onSwitchSelectorReady = function (api) {
                switchSelectorAPI = api;
                switchSelectorReadyDeferred.resolve();
            };
            $scope.scopeModel.deliveryStatus = [
                {
                    value: true,
                    description:"Yes"
                },
                {
                      value: false,
                      description: "No"
                },
            ]
        };
        function load() {
            loadAllControls();
        }

        function getGridFilter() {
            var query = {
                ReleaseCode: $scope.scopeModel.releaseCode,
                SwitchIds: switchSelectorAPI.getSelectedIds(),
                IsDelivered: $scope.scopeModel.selectedDeliveryStatus != undefined ? $scope.scopeModel.selectedDeliveryStatus.value : undefined,
                Description: $scope.scopeModel.description
            }
            return query;
        }
     
        function loadSwitchSelector() {
            var switchSelectorLoadDeferred = UtilsService.createPromiseDeferred();
            switchSelectorReadyDeferred.promise.then(function () {
                var payload = (switchReleaseCauseEntity != undefined) ? { selectedIds: switchReleaseCauseEntity.SwitchId } : undefined;
                VRUIUtilsService.callDirectiveLoad(switchSelectorAPI, payload, switchSelectorLoadDeferred);
            });
            return switchSelectorLoadDeferred.promise;
        }
        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadSwitchSelector]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
       
    }
    appControllers.controller("WhS_BE_SwitchReleaseCauseManagementController", switchReleaseCauseManagementController);
})(appControllers);