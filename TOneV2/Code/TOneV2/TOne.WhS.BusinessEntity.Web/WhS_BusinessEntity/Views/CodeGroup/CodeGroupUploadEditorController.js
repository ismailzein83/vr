codeGroupUploadEditorController.$inject = ['$scope', 'VRUIUtilsService', 'UtilsService','WhS_BE_CodeGroupAPIService'];

function codeGroupUploadEditorController($scope, VRUIUtilsService, UtilsService, WhS_BE_CodeGroupAPIService) {
    loadParameters();
    defineScope();
    load();
    function loadParameters() {
    }
    function defineScope() {
  
        $scope.upload = function () {
            return WhS_BE_CodeGroupAPIService.UploadCodeGroupList($scope.codeGroupList.fileId).then(function (response) {
            });
        }
        $scope.downloadTemplate = function () {
            return WhS_BE_CodeGroupAPIService.DownloadCodeGroupListTemplate().then(function (response) {
                UtilsService.downloadFile(response.data, response.headers);
            });
        }

    }
    function load() {
        $scope.title = UtilsService.buildTitleForUploadEditor("Code Group")
    }


};
appControllers.controller('WhS_BE_CodeGroupUploadEditorController', codeGroupUploadEditorController);