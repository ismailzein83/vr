CodePrepartionUploadEditorController.$inject = ['$scope', 'VRUIUtilsService', 'UtilsService', 'WhS_CodePrep_CodePrepAPIService', 'VRNotificationService', 'VRNavigationService', 'WhS_BP_CreateProcessResultEnum', 'BusinessProcessService'];

function CodePrepartionUploadEditorController($scope, VRUIUtilsService, UtilsService, WhS_CodePrep_CodePrepAPIService, VRNotificationService, VRNavigationService, WhS_BP_CreateProcessResultEnum, BusinessProcessService) {
    loadParameters();
    var fileID;
    var parameters;
    defineScope();
    load();
    function loadParameters() {
        parameters = VRNavigationService.getParameters($scope);
    }
    function defineScope() {
        $scope.close = function () {
            $scope.modalContext.closeModal();
        };
        $scope.isUploadingComplete = false;
        $scope.upload = function () {
            var input = {
                SellingNumberPlanId: parameters.SellingNumberPlanId,
                FileId: $scope.zoneList.fileId,
                EffectiveDate: $scope.effectiveDate,
                IsFromExcel: true
            };
            return WhS_CodePrep_CodePrepAPIService.ApplyCodePreparationForEntities(input).then(function (response) {
                if (response.Result == WhS_BP_CreateProcessResultEnum.Succeeded.value) {
                    $scope.modalContext.closeModal();
                    return BusinessProcessService.openProcessTracking(response.ProcessInstanceId);
                }

            });
        }

        $scope.downloadTemplate = function () {
            return WhS_CodePrep_CodePrepAPIService.DownloadImportCodePreparationTemplate().then(function (response) {
                UtilsService.downloadFile(response.data, response.headers);
            });
        }

    }
    function load() {
        $scope.title = UtilsService.buildTitleForUploadEditor("Code Prepartion Sheet")
    }


};
appControllers.controller('WhS_CodePreparation_CodePrepartionUploadEditorController', CodePrepartionUploadEditorController);