codeGroupUploadEditorController.$inject = ['$scope', 'VRUIUtilsService', 'UtilsService', 'Vr_NP_CodeGroupAPIService', 'VRNotificationService'];

function codeGroupUploadEditorController($scope, VRUIUtilsService, UtilsService, Vr_NP_CodeGroupAPIService, VRNotificationService) {
    loadParameters();
    var fileID;
    defineScope();
    load();
    function loadParameters() {
    }
    function defineScope() {
        $scope.hasDownloadCodeGroupListTemplatePermission = function () {
            return Vr_NP_CodeGroupAPIService.HasDownloadCodeGroupListTemplatePermission();
        };

        $scope.hasDownloadCodeGroupLogPermission = function () {
            return Vr_NP_CodeGroupAPIService.HasDownloadCodeGroupLogPermission();
        };

        $scope.hasUploadCodeGroupListPermission = function () {
            return Vr_NP_CodeGroupAPIService.HasUploadCodeGroupListPermission();
        };

        $scope.isUploadingComplete = false;
        $scope.upload = function () {
            return Vr_NP_CodeGroupAPIService.UploadCodeGroupList($scope.codeGroupList.fileId).then(function (response) {
                $scope.isUploadingComplete = true;
                $scope.addedCodeGroups = response.CountOfCodeGroupsAdded;
                $scope.failedCodeGroup = response.CountOfCodeGroupsFailed;
                fileID = response.fileID;
                VRNotificationService.showSuccess("Code Group Finished Upload");
            }).catch(function (error) {
                VRNotificationService.showError(error.ExceptionMessage);
            });
        };
        $scope.downloadTemplate = function () {
            return Vr_NP_CodeGroupAPIService.DownloadCodeGroupListTemplate().then(function (response) {
                UtilsService.downloadFile(response.data, response.headers);
            });
        };
        $scope.downloadLog = function () {
            if (fileID != undefined) {
                return Vr_NP_CodeGroupAPIService.DownloadCodeGroupLog(fileID).then(function (response) {
                    UtilsService.downloadFile(response.data, response.headers);
                });
            }
        };

    }
    function load() {
        $scope.title = UtilsService.buildTitleForUploadEditor("Code Group");
    }


}
appControllers.controller('Vr_NP_CodeGroupUploadEditorController', codeGroupUploadEditorController);