(function (appControllers) {

    "use strict";

    emailEditorContorller.$inject = ['$scope', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService', 'VRCommon_VRMailAPIService'];

    function emailEditorContorller($scope, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService, VRCommon_VRMailAPIService) {

        var emailObject;
        var fileId;
        defineScope();
        loadParameters();
        loadAllControls();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined) {
                emailObject = parameters.evaluatedEmail;
                fileId = parameters.fileId;
            }
            console.log(fileId);
        }

        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.sendEmail = function () {
                return sendEmail();
            };
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };

            function sendEmail() {
                $scope.scopeModel.isLoading = true;
                var emailObject = buildEmailObjFromScope();
                return VRCommon_VRMailAPIService.SendEmail(emailObject)
               .then(function (response) {
                   VRNotificationService.showSuccess("Email sent successfully");
                   $scope.modalContext.closeModal();
               })
               .catch(function (error) {
                   VRNotificationService.notifyException(error, $scope);
               }).finally(function () {
                   $scope.scopeModel.isLoading = false;
               });
            }
        }


        function loadAllControls() {

            function setTitle() {
                $scope.title = "Email";
            }

            function loadStaticData() {
                if (emailObject != undefined) {
                    $scope.scopeModel.cc = emailObject.CC;
                    $scope.scopeModel.to = emailObject.To;
                    $scope.scopeModel.subject = emailObject.Subject;
                    $scope.scopeModel.body = emailObject.Body;
                }
                $scope.scopeModel.priceListSheet = null;
                //if (fileId != undefined)
                //    $scope.scopeModel.priceListSheet = {
                //        fileId: fileId
                //    };
                //else
                //    $scope.scopeModel.priceListSheet = null;
            }
            function loadFileName() {
                if (fileId != undefined) {
                    return VRCommon_VRMailAPIService.GetFileName(fileId)
                        .then(function(response) {
                            scopeModel.fileName = response;
                        });
                }
            }
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadFileName])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.scopeModel.isLoading = false;
              });
        }

        function buildEmailObjFromScope() {
            var attachment =
            {
                $type: "VRMailMessageAttachment"
            };

            var obj = {
                ToEmail: $scope.toEmail,
                Subject: $scope.subject,
                Body: $scope.body,
                atta: [attachment]
            };
            return obj;
        }
    }

    appControllers.controller('VRCommon_VRMailMessageEvaluatorController', emailEditorContorller);
})(appControllers);
