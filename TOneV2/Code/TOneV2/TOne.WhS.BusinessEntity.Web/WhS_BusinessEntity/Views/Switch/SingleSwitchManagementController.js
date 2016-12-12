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
                switchEditorDirectiveAPI.save().finally(function () {
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
