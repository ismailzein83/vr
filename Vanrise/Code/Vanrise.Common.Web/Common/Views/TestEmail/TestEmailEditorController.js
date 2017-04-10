(function (appControllers) {

    "use strict";

    testEmailEditorController.$inject = ['$scope', 'VRNotificationService', 'UtilsService', 'VRNavigationService', 'VRCommon_VRMailAPIService'];

    function testEmailEditorController($scope, VRNotificationService, UtilsService, VRNavigationService, VRCommon_VRMailAPIService) {

        var emailSettingDataEntity;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                emailSettingDataEntity = parameters.emailSettingData;
            }
        }

        function defineScope() {

            $scope.sendTestEmail = function () {
                var emailSettingDetail = {
                    EmailSettingData: emailSettingDataEntity,
                    ToEmail: $scope.toEmail,
                    Subject: $scope.subject,
                    Body: $scope.body
                };
                $scope.isLoading = true;
                VRCommon_VRMailAPIService.SendTestEmail(emailSettingDetail).finally(function () {
                    $scope.isLoading = false;
                    VRNotificationService.showSuccess("Email sent successfully");
                    $scope.modalContext.closeModal();
                });
            };

            $scope.close = function () {
                $scope.modalContext.closeModal();
            };
        }

        function load() {
            $scope.subject = "Test Subject";
            $scope.body = "Test Body";
            $scope.isLoading = true;
            loadAllControls();
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.isLoading = false;
              });
        }

        function setTitle() {
            $scope.title = "Send Test Email";
        }
    }

    appControllers.controller('VRCommon_TestEmailEditorController', testEmailEditorController);
})(appControllers);
