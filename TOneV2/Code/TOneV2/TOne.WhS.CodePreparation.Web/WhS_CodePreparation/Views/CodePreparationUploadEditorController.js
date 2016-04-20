(function (appControllers) {

    'use strict';

    CodePrepartionUploadEditorController.$inject = ['$scope', 'VRUIUtilsService', 'UtilsService', 'WhS_CP_CodePrepAPIService', 'VRNotificationService', 'VRNavigationService', 'WhS_BP_CreateProcessResultEnum', 'BusinessProcess_BPInstanceService', 'BusinessProcess_BPInstanceAPIService'];

    function CodePrepartionUploadEditorController($scope, VRUIUtilsService, UtilsService, WhS_CP_CodePrepAPIService, VRNotificationService, VRNavigationService, WhS_BP_CreateProcessResultEnum, BusinessProcess_BPInstanceService, BusinessProcess_BPInstanceAPIService) {
        var fileID;
        var parameters;

        loadParameters();
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
                var inputArguments = {
                    $type: "TOne.WhS.CodePreparation.BP.Arguments.CodePreparationInput, TOne.WhS.CodePreparation.BP.Arguments",
                    SellingNumberPlanId: parameters.SellingNumberPlanId,
                    FileId: $scope.zoneList.fileId,
                    EffectiveDate: $scope.effectiveDate,
                    IsFromExcel: true
                };
                var input = {
                    InputArguments: inputArguments
                };

                return BusinessProcess_BPInstanceAPIService.CreateNewProcess(input).then(function (response) {
                    if (response.Result == WhS_BP_CreateProcessResultEnum.Succeeded.value) {
                        $scope.modalContext.closeModal();
                        return BusinessProcess_BPInstanceService.openProcessTracking(response.ProcessInstanceId);
                    }
                });
            }

            $scope.downloadTemplate = function () {
                return WhS_CP_CodePrepAPIService.DownloadImportCodePreparationTemplate().then(function (response) {
                    UtilsService.downloadFile(response.data, response.headers);
                });
            }

        }
        function load() {
            $scope.title = UtilsService.buildTitleForUploadEditor("Numbering Plan Sheet")
        }
    };

    appControllers.controller('WhS_CP_CodePrepartionUploadEditorController', CodePrepartionUploadEditorController);

})(appControllers);
