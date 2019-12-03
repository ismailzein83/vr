(function (app) {

    "use strict";

    singleSwitchEditorController.$inject = ["$scope", "WhS_BE_SwitchAPIService", "UtilsService", "VRNotificationService", "VRNavigationService", "VRUIUtilsService"];

    function singleSwitchEditorController($scope, WhS_BE_SwitchAPIService, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService) {


        var isEditMode;

        var switchEditorDirectiveAPI;
        var switchEditorReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();
        var switchId;

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != null) {
                switchId = parameters.switchId;
            }
            isEditMode = (switchId != undefined);

        }

        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.hasSaveSwitchPermission = function () {
                if (isEditMode)
                    return WhS_BE_SwitchAPIService.HasUpdateSwitchPermission();
                else
                    return WhS_BE_SwitchAPIService.HasAddSwitchPermission();
            };
            $scope.scopeModel.saveSwitch = function () {
                $scope.isLoading = true;
                var resetSwitchSyncDataPromise = UtilsService.createPromiseDeferred();

                switchEditorDirectiveAPI.save().then(function () {
                    VRNotificationService.showConfirmation("Do you want to reset the switch info in order to trigger the full sync with the next route sync process?").then(function (result) {
                        if (result) {
                            WhS_BE_SwitchAPIService.ResetSwitchSyncData(switchId).then(function () {
                                resetSwitchSyncDataPromise.resolve();
                            }).catch(function (error) {
                                resetSwitchSyncDataPromise.resolve();
                            });
                        }
                        else {
                            resetSwitchSyncDataPromise.resolve();
                        }
                    });

                }).catch(function (error) {
                    resetSwitchSyncDataPromise.resolve();
                });

                resetSwitchSyncDataPromise.promise.then(function () {
                    $scope.isLoading = false;
                });
            };

            $scope.onSwitchEditorReady = function (api) {
                switchEditorDirectiveAPI = api;
                switchEditorReadyDeferred.resolve();
            };
        }
        function load() {
            $scope.isLoading = true;
            loadAllControls();
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadSwitchEditorDirective]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });
        }

        function loadSwitchEditorDirective() {
            var switchEditorLoadDeferred = UtilsService.createPromiseDeferred();
            switchEditorReadyDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(switchEditorDirectiveAPI, { switchId: switchId }, switchEditorLoadDeferred);
            });
            return switchEditorLoadDeferred.promise;
        }
    }

    app.controller("WhS_BE_SingleSwitchManagementController", singleSwitchEditorController);
})(app);
