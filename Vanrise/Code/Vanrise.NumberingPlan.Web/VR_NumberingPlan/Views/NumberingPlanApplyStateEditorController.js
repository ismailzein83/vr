(function (appControllers) {

    'use strict';

    NumberingPlanApplyStateEditorController.$inject = ['$scope', 'VRUIUtilsService', 'UtilsService', 'BusinessProcess_BPInstanceAPIService', 'VRNotificationService', 'VRNavigationService', 'WhS_BP_CreateProcessResultEnum', 'BusinessProcess_BPInstanceService', 'Vr_NP_CodePrepAPIService'];

    function NumberingPlanApplyStateEditorController($scope, VRUIUtilsService, UtilsService, BusinessProcess_BPInstanceAPIService, VRNotificationService, VRNavigationService, WhS_BP_CreateProcessResultEnum, BusinessProcess_BPInstanceService, Vr_NP_CodePrepAPIService) {
        var sellingNumberPlanId;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            sellingNumberPlanId = parameters.SellingNumberPlanId;
        }

        function defineScope() {

            $scope.effectiveDate = new Date();

            $scope.close = function () {
                $scope.modalContext.closeModal();
            };

            $scope.applyState = function () {

                var inputArguments = {
                    $type: "Vanrise.NumberingPlan.BP.Arguments.CodePreparationInput, Vanrise.NumberingPlan.BP.Arguments",
                    SellingNumberPlanId: sellingNumberPlanId,
                    EffectiveDate: $scope.effectiveDate,
                    IsFromExcel: false
                };
                var input = {
                    InputArguments: inputArguments
                };


                return BusinessProcess_BPInstanceAPIService.CreateNewProcess(input).then(function (response) {
                    if (response.Result == WhS_BP_CreateProcessResultEnum.Succeeded.value) {
                        $scope.modalContext.closeModal();
                        var context = {
                            onClose: $scope.onCodePreparationApplied
                        };
                    }
                    return BusinessProcess_BPInstanceService.openProcessTracking(response.ProcessInstanceId, context);
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
        $scope.title = "Apply Numbering Plan State";
    }


    function loadEffectiveDate() {

        return Vr_NP_CodePrepAPIService.GetCPSettings().then(function (response) {
            var effectiveDate = new Date();
            effectiveDate.setDate(effectiveDate.getDate() + response.EffectiveDateOffset);
            $scope.effectiveDate = effectiveDate;
        });
    }

};

    appControllers.controller('Vr_NP_NumberingPlanApplyStateEditorController', NumberingPlanApplyStateEditorController);

})(appControllers);
