(function (appControllers) {

    "use strict";

    accountStatusUploaderController.$inject = ['$scope', "Fzero_FraudAnalysis_AccountStatusAPIService", 'VRNotificationService', 'UtilsService'];

    function accountStatusUploaderController($scope, Fzero_FraudAnalysis_AccountStatusAPIService, VRNotificationService, UtilsService) {

        defineScope();
        load();

        function defineScope() {
            $scope.downloadTemplate = function () {
                return Fzero_FraudAnalysis_AccountStatusAPIService.DownloadAccountStatusesTemplate().then(function (response) {
                    UtilsService.downloadFile(response.data, response.headers);
                });
            }
            $scope.hasDownloadAccountStatusPermission = function () {
                return Fzero_FraudAnalysis_AccountStatusAPIService.HasDownloadAccountStatusPermission();
            };


            $scope.uploadAccountStatuses = function () {
                return Fzero_FraudAnalysis_AccountStatusAPIService.UploadAccountStatuses($scope.file.fileId, $scope.validTill).then(function (response) {
                    VRNotificationService.showInformation(response)
                });
            }
            $scope.hasUploadAccountStatusPermission = function () {
                return Fzero_FraudAnalysis_AccountStatusAPIService.HasUploadAccountStatusPermission();
            };
        }

        function load() {
        }
    }

    appControllers.controller('FraudAnalysis_AccountStatusUploaderController', accountStatusUploaderController);
})(appControllers);
