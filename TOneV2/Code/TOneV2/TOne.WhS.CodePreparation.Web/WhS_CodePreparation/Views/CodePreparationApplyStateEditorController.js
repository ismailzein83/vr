(function (appControllers) {

    'use strict';

    CodePreparationApplyStateEditorController.$inject = ['$scope', 'VRUIUtilsService', 'UtilsService', 'BusinessProcess_BPInstanceAPIService', 'VRNotificationService', 'VRNavigationService', 'WhS_BP_CreateProcessResultEnum', 'BusinessProcess_BPInstanceService'];

    function CodePreparationApplyStateEditorController($scope, VRUIUtilsService, UtilsService, BusinessProcess_BPInstanceAPIService, VRNotificationService, VRNavigationService, WhS_BP_CreateProcessResultEnum, BusinessProcess_BPInstanceService) {
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
            $scope.applyState = function () {

                var inputArguments = {
                    $type: "TOne.WhS.CodePreparation.BP.Arguments.CodePreparationInput, TOne.WhS.CodePreparation.BP.Arguments",
                    SellingNumberPlanId: parameters.SellingNumberPlanId,
                    EffectiveDate: $scope.effectiveDate,
                    IsFromExcel: false
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
        }

        function load() {
            $scope.title = UtilsService.buildTitleForUploadEditor("Apply Code Prepartion State")
        }
    };

    appControllers.controller('WhS_CP_CodePreparationApplyStateEditorController', CodePreparationApplyStateEditorController);

})(appControllers);
