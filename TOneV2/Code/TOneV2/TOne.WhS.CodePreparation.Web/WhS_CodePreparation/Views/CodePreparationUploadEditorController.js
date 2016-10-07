(function (appControllers) {

    'use strict';

    CodePrepartionUploadEditorController.$inject = ['$scope', 'VRUIUtilsService', 'UtilsService', 'WhS_CP_CodePrepAPIService', 'VRNotificationService', 'VRNavigationService', 'WhS_BP_CreateProcessResultEnum', 'BusinessProcess_BPInstanceService', 'BusinessProcess_BPInstanceAPIService'];

    function CodePrepartionUploadEditorController($scope, VRUIUtilsService, UtilsService, WhS_CP_CodePrepAPIService, VRNotificationService, VRNavigationService, WhS_BP_CreateProcessResultEnum, BusinessProcess_BPInstanceService, BusinessProcess_BPInstanceAPIService) {
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
                var inputArguments = {
                    $type: "TOne.WhS.CodePreparation.BP.Arguments.CodePreparationInput, TOne.WhS.CodePreparation.BP.Arguments",
                    SellingNumberPlanId: sellingNumberPlanId,
                    FileId: $scope.zoneList.fileId,
                    EffectiveDate: $scope.effectiveDate,
                    HasHeader: $scope.hasHeader,
                    IsFromExcel: true
                };
                var input = {
                    InputArguments: inputArguments
                };

                return BusinessProcess_BPInstanceAPIService.CreateNewProcess(input).then(function (response) {
                    if (response.Result == WhS_BP_CreateProcessResultEnum.Succeeded.value) {
                        $scope.modalContext.closeModal();
                        var context = {
                            onClose: $scope.onCodePreparationUpdated
                            }
                        }
                        return BusinessProcess_BPInstanceService.openProcessTracking(response.ProcessInstanceId, context);
                    });
            }

            $scope.downloadTemplate = function () {
                return WhS_CP_CodePrepAPIService.DownloadImportCodePreparationTemplate().then(function (response) {
                    UtilsService.downloadFile(response.data, response.headers);
                });
            }

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
            $scope.title = UtilsService.buildTitleForUploadEditor("Numbering Plan Sheet")
        }


        function loadEffectiveDate() {

            return WhS_CP_CodePrepAPIService.GetCPSettings().then(function (response) {
                var effectiveDate = new Date();
                effectiveDate.setDate(effectiveDate.getDate() + response.EffectiveDateOffset);
                $scope.effectiveDate = effectiveDate;
            })
        }

    };

    appControllers.controller('WhS_CP_CodePrepartionUploadEditorController', CodePrepartionUploadEditorController);

})(appControllers);
