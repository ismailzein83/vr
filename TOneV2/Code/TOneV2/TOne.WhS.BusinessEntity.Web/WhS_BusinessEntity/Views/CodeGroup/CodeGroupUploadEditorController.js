codeGroupUploadEditorController.$inject = ['$scope', 'VRUIUtilsService', 'UtilsService','WhS_BE_CodeGroupAPIService','VRNotificationService'];

function codeGroupUploadEditorController($scope, VRUIUtilsService, UtilsService, WhS_BE_CodeGroupAPIService, VRNotificationService) {
    loadParameters();
    var fileID;
    defineScope();
    load();
    function loadParameters() {
    }
    function defineScope() {
        $scope.isUploadingComplete = false;
        $scope.upload = function () {
            return WhS_BE_CodeGroupAPIService.UploadCodeGroupList($scope.codeGroupList.fileId).then(function (response) {
                $scope.isUploadingComplete = true;
                $scope.addedCodeGroups = response.CountOfCodeGroupsAdded;
                $scope.failedCodeGroup = response.CountOfCodeGroupsFailed;
                fileID = response.fileID;
                VRNotificationService.showSuccess("Code Group Finished Upload");
            });
        }
        $scope.downloadTemplate = function () {
            return WhS_BE_CodeGroupAPIService.DownloadCodeGroupListTemplate().then(function (response) {
                UtilsService.downloadFile(response.data, response.headers);
            });
        }
        $scope.downloadLog = function () {
            if (fileID != undefined)
            {
                return WhS_BE_CodeGroupAPIService.DownloadCodeGroupLog(fileID).then(function (response) {
                    UtilsService.downloadFile(response.data, response.headers);
                });
            }
        }

    }
    function load() {
        $scope.title = UtilsService.buildTitleForUploadEditor("Code Group")
    }


};
appControllers.controller('WhS_BE_CodeGroupUploadEditorController', codeGroupUploadEditorController);