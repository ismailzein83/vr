(function (appControllers) {

    "use strict";

    genericRuleUploaderController.$inject = ['$scope', 'VR_GenericData_GenericRuleAPIService', 'VRNotificationService', 'UtilsService', 'VRNavigationService'];

    function genericRuleUploaderController($scope, VR_GenericData_GenericRuleAPIService, VRNotificationService, UtilsService, VRNavigationService) {
        
        var genericRuleDefinitionId;
        var context;
        var criteriaFieldsToHide = [];
        var criteriaFieldsValues;
        var fileId;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined) {
                genericRuleDefinitionId = parameters.ruleDefinitionId;
                context = parameters.context;
                criteriaFieldsToHide = parameters.criteriaFieldsToHide;
                criteriaFieldsValues = parameters.criteriaFieldsValues;
            }
        }

        function defineScope() {

            $scope.disableUpload = false;
            $scope.isErrorOccured = false;
            $scope.isUploadingComplete = false;
            $scope.downloadTemplate = function () {
                var input = {
                    CriteriaFieldsToHide: criteriaFieldsToHide,
                    RuleDefinitionId: genericRuleDefinitionId,
                };
                return VR_GenericData_GenericRuleAPIService.DownloadGenericRulesTemplate(input).then(function (response) {
                    UtilsService.downloadFile(response.data, response.headers);
                });
            };

            $scope.uploadGenericRules = function () {
                var input = {
                    GenericRuleDefinitionId: genericRuleDefinitionId,
                    EffectiveDate: $scope.effectiveDate,
                    FileId: $scope.file.fileId,
                    CriteriaFieldsValues: criteriaFieldsValues
                };
                return VR_GenericData_GenericRuleAPIService.UploadGenericRules(input).then(function (response) {
                    $scope.isUploadingComplete = true;
                   
                    $scope.genericRulesAdded = response.NumberOfGenericRulesAdded;
                    $scope.genericRulesFailed = response.NumberOfGenericRulesFailed;
                    fileId = response.FileId;
                    $scope.errorMessage = response.ErrorMessage;

                    if ($scope.errorMessage != undefined) {
                        $scope.disableUpload = false;
                        $scope.isErrorOccured = true;
                        VRNotificationService.showError("Upload Process Failed");
                    }
                    else {
                        $scope.disableUpload = true;
                        $scope.isErrorOccured = false;
                        VRNotificationService.showSuccess("Upload Process Complete");
                    }
                }).catch(function(error){
                VRNotificationService.showError(error.ExceptionMessage);
                });
            };

            $scope.downloadOutput = function () {
                if (fileId != undefined) {
                    return VR_GenericData_GenericRuleAPIService.DownloadUploadGenericRulesOutput(fileId).then(function (response) {
                        UtilsService.downloadFile(response.data, response.headers);
                    });
                }
            };
        }

        function load() {

            $scope.isLoading = true;
            return UtilsService.waitMultipleAsyncOperations([setTitle])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.isLoading = false;
              });
        }
        function setTitle() {
            $scope.title = 'Upload Rules';
        }
    }

    appControllers.controller('VR_GenericData_GenericRuleUploaderController', genericRuleUploaderController);
})(appControllers);
