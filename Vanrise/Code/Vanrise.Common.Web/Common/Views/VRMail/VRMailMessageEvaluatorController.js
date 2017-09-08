(function (appControllers) {

    "use strict";

    emailEditorContorller.$inject = ['$scope', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService', 'VRCommon_VRMailAPIService'];

    function emailEditorContorller($scope, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService, VRCommon_VRMailAPIService) {

        var emailObject;
        var fileId;
        var fileAPI;
        defineScope();
        loadParameters();
        loadAllControls();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined) {
                emailObject = parameters.evaluatedEmail;
                fileId = parameters.fileId;
                $scope.scopeModel.fileId = fileId;
            }
        }

        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.uploadedAttachements = [];
            $scope.scopeModel.confirmEmail = function () {
                return confirmEmail();
            };
            $scope.scopeModel.onUploadedAttachementFileReady = function (api) {
                fileAPI = api;
            };
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };

            function confirmEmail() {
                var emailObject = buildEmailObjFromScope();
                if ($scope.onSalePriceListSendingEmail != undefined)
                    $scope.onSalePriceListSendingEmail(emailObject);
                $scope.modalContext.closeModal();
            }

            $scope.scopeModel.downloadAttachement = function (attachedfileId) {
                return VRCommon_VRMailAPIService.DownloadAttachement(attachedfileId).then(function (response) {
                    UtilsService.downloadFile(response.data, response.headers);
                });
            };
            $scope.scopeModel.addUploadedAttachement = function (obj) {
                if (obj != undefined) {
                    $scope.scopeModel.uploadedAttachements.push(obj);
                    fileAPI.clearFileUploader();
                }
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
                $scope.scopeModel.UploadpriceListSheet = null;
            }
            function loadFileName() {
                if (fileId != undefined) {
                    return VRCommon_VRMailAPIService.GetFileName(fileId)
                        .then(function (response) {
                            $scope.scopeModel.fileName = response;
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
            var attachementFileIds = $scope.scopeModel.uploadedPriceLists.map(function (a) { return a.fileId; });
            attachementFileIds.push(fileId);
            var obj = {
                CC: $scope.scopeModel.cc,
                To: $scope.scopeModel.to,
                Subject: $scope.scopeModel.subject,
                Body: $scope.scopeModel.body,
                AttachementFileIds: attachementFileIds
            };
            return obj;
        }
    }

    appControllers.controller('VRCommon_VRMailMessageEvaluatorController', emailEditorContorller);
})(appControllers);
