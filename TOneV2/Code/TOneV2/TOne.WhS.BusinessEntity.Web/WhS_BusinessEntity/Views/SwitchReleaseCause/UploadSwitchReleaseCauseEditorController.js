(function (appControllers) {

    "use strict";

    uploadSwitchReleaseCauseEditorController.$inject = ["$scope", "UtilsService", "VRUIUtilsService", "VRNotificationService", "WhS_BE_SwitchReleaseCauseAPIService", "WhS_BE_SwitchReleaseCauseService"];

    function uploadSwitchReleaseCauseEditorController($scope, UtilsService, VRUIUtilsService, VRNotificationService, WhS_BE_SwitchReleaseCauseAPIService, WhS_BE_SwitchReleaseCauseService) {
        var switchSelectorAPI;
        var switchSelectorReadyDeferred = UtilsService.createPromiseDeferred();
        var fileID;
        defineScope();
        load();
        function defineScope() {

            $scope.scopeModel = {};

            $scope.scopeModel.isUploadingComplete = false;

            $scope.scopeModel.downloadTemplate = function () {
                return WhS_BE_SwitchReleaseCauseAPIService.DownloadSwitchReleaseCausesTemplate().then(function (response) {
                    if (response != undefined)
                    UtilsService.downloadFile(response.data, response.headers);
                });
            };

            $scope.scopeModel.uploadSwitchReleaseCauses = function () {
                $scope.scopeModel.isLoading = true;
                var switchId = switchSelectorAPI.getSelectedIds();

                return WhS_BE_SwitchReleaseCauseAPIService.UploadSwitchReleaseCauses($scope.file.fileId, switchId).then(function (response) {
                    if (response != undefined) {

                        $scope.scopeModel.isUploadingComplete = true;

                        $scope.scopeModel.addedSwitchReleaseCauses = response.CountOfSwitchReleaseCausesAdded;

                        $scope.scopeModel.existsSwitchReleaseCauses = response.CountOfSwitchReleaseCausesExist;

                        fileID = response.fileID;

                        VRNotificationService.showSuccess("Switch Release Cause Finished Upload");
                    }
                }).catch(function (error) {
                    VRNotificationService.showError(error.ExceptionMessage);
                }).finally(function () {
                    $scope.scopeModel.isLoading = false;
                });
            };

            $scope.scopeModel.downloadLog = function () {
                if (fileID != undefined) {
                    return WhS_BE_SwitchReleaseCauseAPIService.DownloadSwitchReleaseCauseLog(fileID).then(function (response) {
                        if (response != undefined)
                        UtilsService.downloadFile(response.data, response.headers);
                    });
                }
            };

            $scope.scopeModel.onSwitchSelectorReady = function (api) {
                switchSelectorAPI = api;
                switchSelectorReadyDeferred.resolve();
            };
        }

        function load() {
            $scope.scopeModel.isLoading = true;
            loadAllControls();
        }

        function loadAllControls() {
            function setTitle() {
                $scope.title = UtilsService.buildTitleForUploadEditor("Switch Release Cause");
            };

            function loadSwitchSelector() {
                var switchSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                switchSelectorReadyDeferred.promise.then(function () {
                    var payload;
                    VRUIUtilsService.callDirectiveLoad(switchSelectorAPI, payload, switchSelectorLoadDeferred);
                });
                return switchSelectorLoadDeferred.promise;
            }

            return UtilsService.waitMultipleAsyncOperations([loadSwitchSelector, setTitle]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
    }
    
    appControllers.controller('WhS_BE_UploadSwitchReleaseCauseEditorController', uploadSwitchReleaseCauseEditorController);
})(appControllers);