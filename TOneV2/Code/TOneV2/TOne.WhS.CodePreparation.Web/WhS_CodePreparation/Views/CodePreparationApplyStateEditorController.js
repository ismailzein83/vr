(function (appControllers) {

    'use strict';

    CodePreparationApplyStateEditorController.$inject = ['$scope', 'VRUIUtilsService', 'UtilsService', 'WhS_CodePrep_CodePrepAPIService', 'VRNotificationService', 'VRNavigationService', 'WhS_BP_CreateProcessResultEnum', 'BusinessProcessService'];

    function CodePreparationApplyStateEditorController($scope, VRUIUtilsService, UtilsService, WhS_CodePrep_CodePrepAPIService, VRNotificationService, VRNavigationService, WhS_BP_CreateProcessResultEnum, BusinessProcessService) {
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
                var input = {
                    SellingNumberPlanId: parameters.SellingNumberPlanId,
                    EffectiveDate: $scope.effectiveDate,
                    IsFromExcel: false
                };
                return WhS_CodePrep_CodePrepAPIService.ApplyCodePreparationForEntities(input).then(function (response) {
                    if (response.Result == WhS_BP_CreateProcessResultEnum.Succeeded.value) {
                        $scope.modalContext.closeModal();
                        return BusinessProcessService.openProcessTracking(response.ProcessInstanceId);
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
