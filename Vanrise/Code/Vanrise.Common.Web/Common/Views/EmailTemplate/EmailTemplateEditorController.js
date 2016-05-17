(function (appControllers) {

    "use strict";

    emailTemplateEditorController.$inject = ['$scope', 'VRCommon_EmailTemplateAPIService', 'VRNotificationService', 'VRNavigationService', 'UtilsService'];

    function emailTemplateEditorController($scope, VRCommon_EmailTemplateAPIService, VRNotificationService, VRNavigationService, UtilsService) {

        var emailTemplateId;
        var emailTemplateEntity;

        defineScope();
        loadParameters();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                emailTemplateId = parameters.emailTemplateId;
            }
        }

        function defineScope() {
            $scope.saveEmailTemplate = function () {
                return updateEmailTemplate();
            };

            $scope.close = function () {
                $scope.modalContext.closeModal();
            };

            $scope.hasUpdateEmailTemplatePermission = function () {
                return VRCommon_EmailTemplateAPIService.HasUpdateEmailTemplatePermission();
            };

        }

        function load() {
            $scope.isLoading = true;


            getEmailTemplate().then(function () {
                loadAllControls().finally(function () {
                    emailTemplateEntity = undefined;
                });
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
                $scope.isLoading = false;
            });

        }

        function getEmailTemplate() {
            return VRCommon_EmailTemplateAPIService.GetEmailTemplate(emailTemplateId).then(function (currency) {
                emailTemplateEntity = currency;
            });
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.isLoading = false;
              });
        }

        function setTitle() {
            $scope.title = UtilsService.buildTitleForUpdateEditor(emailTemplateEntity.Name, "Email Template");
        }

        function loadStaticData() {
            $scope.name = emailTemplateEntity.Name;
            $scope.bodyTemplate = emailTemplateEntity.BodyTemplate;
            $scope.subjectTemplate = emailTemplateEntity.SubjectTemplate;
        }

        function buildCurrencyObjFromScope() {
            var obj = {
                EmailTemplateId: emailTemplateId,
                Name: $scope.name,
                BodyTemplate:$scope.bodyTemplate,
                SubjectTemplate: $scope.subjectTemplate
            };
            return obj;
        }

        function updateEmailTemplate() {
            $scope.isLoading = true;

            var emailTemplateObject = buildCurrencyObjFromScope();

            VRCommon_EmailTemplateAPIService.UpdateEmailTemplate(emailTemplateObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated("Email Template", response, "Name")) {
                    if ($scope.onEmailTemplateUpdated != undefined)
                        $scope.onEmailTemplateUpdated(response.UpdatedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });
        }
    }

    appControllers.controller('VRCommon_EmailTemplateEditorController', emailTemplateEditorController);
})(appControllers);
