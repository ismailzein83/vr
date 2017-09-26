(function (appControllers) {

    'use strict';

    CodePrepartionUploadEditorController.$inject = ['$scope', 'VRUIUtilsService', 'UtilsService', 'WhS_CP_CodePrepAPIService', 'VRNotificationService', 'VRNavigationService', 'WhS_BP_CreateProcessResultEnum', 'BusinessProcess_BPInstanceService', 'BusinessProcess_BPInstanceAPIService', 'WhS_BE_SellingNumberPlanAPIService', 'WhS_CP_NumberingPlanDefinitionEnum', 'WhS_CP_CodePrepService','VRDateTimeService'];

    function CodePrepartionUploadEditorController($scope, VRUIUtilsService, UtilsService, WhS_CP_CodePrepAPIService, VRNotificationService, VRNavigationService, WhS_BP_CreateProcessResultEnum, BusinessProcess_BPInstanceService, BusinessProcess_BPInstanceAPIService, WhS_BE_SellingNumberPlanAPIService, WhS_CP_NumberingPlanDefinitionEnum, WhS_CP_CodePrepService, VRDateTimeService) {
        var fileID;
        var sellingNumberPlanId;

        loadParameters();

        defineScope();

        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            sellingNumberPlanId = parameters.SellingNumberPlanId;
        }

        function defineScope() {

            $scope.hasHeader = true;

            $scope.close = function () {
                $scope.modalContext.closeModal();
            };

            $scope.isUploadingComplete = false;

            $scope.upload = function () {
                $scope.isLoading = true;
                WhS_CP_CodePrepService.hasRunningProcessesForSellingNumberPlan(sellingNumberPlanId).then(function (response) {
                    $scope.isLoading = false;
                    if (!response.hasRunningProcesses) {
                        WhS_CP_CodePrepService.createNewProcess(sellingNumberPlanId, $scope.zoneList.fileId, $scope.effectiveDate, $scope.hasHeader, true, $scope.onCodePreparationUpdated).then(function () { $scope.modalContext.closeModal(); }).catch(function (error) {
                            VRNotificationService.notifyException(error, $scope);
                        });
                    }
                    else {
                        VRNotificationService.showWarning("Cannot start process because another instance is still running");
                    }
                });
                
            };

            $scope.downloadTemplate = function () {
                return WhS_CP_CodePrepAPIService.DownloadImportCodePreparationTemplate().then(function (response) {
                    UtilsService.downloadFile(response.data, response.headers);
                });
            };

        }
     
        function load() {
            $scope.isLoading = true;
            loadAllControls();
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadEffectiveDate])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.isLoading = false;
              });
        }

        function setTitle() {
            $scope.title = UtilsService.buildTitleForUploadEditor("Numbering Plan Sheet");
        }


        function loadEffectiveDate() {

        	return WhS_CP_CodePrepAPIService.GetCPEffectiveDateDayOffset().then(function (effectiveDateDayOffset) {
        	    var effectiveDate = UtilsService.getDateFromDateTime(VRDateTimeService.getNowDateTime());
                effectiveDate.setDate(effectiveDate.getDate() + effectiveDateDayOffset);
                $scope.effectiveDate = effectiveDate;
            });
        }

    };

    appControllers.controller('WhS_CP_CodePrepartionUploadEditorController', CodePrepartionUploadEditorController);

})(appControllers);
