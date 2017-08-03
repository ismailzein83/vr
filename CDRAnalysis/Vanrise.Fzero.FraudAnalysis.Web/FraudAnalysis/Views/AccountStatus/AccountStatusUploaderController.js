(function (appControllers) {

    "use strict";

    accountStatusUploaderController.$inject = ['$scope', "Fzero_FraudAnalysis_AccountStatusAPIService", 'VRNotificationService', 'UtilsService','VRValidationService'];

    function accountStatusUploaderController($scope, Fzero_FraudAnalysis_AccountStatusAPIService, VRNotificationService, UtilsService, VRValidationService) {

        defineScope();
        load();

        function defineScope() {
            $scope.downloadTemplate = function () {
                return Fzero_FraudAnalysis_AccountStatusAPIService.DownloadAccountStatusesTemplate().then(function (response) {
                    UtilsService.downloadFile(response.data, response.headers);
                });
            };
            $scope.hasDownloadAccountStatusPermission = function () {
                return Fzero_FraudAnalysis_AccountStatusAPIService.HasDownloadAccountStatusPermission();
            };

            $scope.validateIsValidDate = function () {
                return VRValidationService.validateTimeEqualorGreaterthanToday($scope.validTill);
            };

            $scope.uploadAccountStatuses = function () {
                return Fzero_FraudAnalysis_AccountStatusAPIService.UploadAccountStatuses($scope.file.fileId, $scope.validTill, $scope.reason).then(function (response) {
                    VRNotificationService.showInformation(response)
                });
            };
            $scope.hasUploadAccountStatusPermission = function () {
                return Fzero_FraudAnalysis_AccountStatusAPIService.HasUploadAccountStatusPermission();
            };
        }

        function load() {
        }
    }

    appControllers.controller('FraudAnalysis_AccountStatusUploaderController', accountStatusUploaderController);
})(appControllers);
