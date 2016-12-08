(function (appControllers) {

    'use strict';

    CodePreparationApplyStateEditorController.$inject = ['$scope', 'VRUIUtilsService', 'UtilsService', 'BusinessProcess_BPInstanceAPIService', 'VRNotificationService', 'VRNavigationService', 'WhS_BP_CreateProcessResultEnum', 'BusinessProcess_BPInstanceService', 'WhS_CP_CodePrepAPIService'];

    function CodePreparationApplyStateEditorController($scope, VRUIUtilsService, UtilsService, BusinessProcess_BPInstanceAPIService, VRNotificationService, VRNavigationService, WhS_BP_CreateProcessResultEnum, BusinessProcess_BPInstanceService, WhS_CP_CodePrepAPIService) {
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
                    $type: "TOne.WhS.CodePreparation.BP.Arguments.CodePreparationInput, TOne.WhS.CodePreparation.BP.Arguments",
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

    	return WhS_CP_CodePrepAPIService.GetCPEffectiveDateDayOffset().then(function (effectiveDateDayOffset) {
    		var effectiveDate = new Date();
    		effectiveDate.setDate(effectiveDate.getDate() + effectiveDateDayOffset);
            $scope.effectiveDate = effectiveDate;
        });
    }

};

appControllers.controller('WhS_CP_CodePreparationApplyStateEditorController', CodePreparationApplyStateEditorController);

})(appControllers);
