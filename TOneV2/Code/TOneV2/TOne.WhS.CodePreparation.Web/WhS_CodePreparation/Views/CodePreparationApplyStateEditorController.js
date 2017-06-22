(function (appControllers) {

    'use strict';

    CodePreparationApplyStateEditorController.$inject = ['$scope', 'VRUIUtilsService', 'UtilsService', 'BusinessProcess_BPInstanceAPIService', 'VRNotificationService', 'VRNavigationService', 'WhS_BP_CreateProcessResultEnum', 'BusinessProcess_BPInstanceService', 'WhS_CP_CodePrepAPIService', 'WhS_BE_SellingNumberPlanAPIService', 'WhS_CP_NumberingPlanDefinitionEnum', 'WhS_CP_CodePrepService'];

    function CodePreparationApplyStateEditorController($scope, VRUIUtilsService, UtilsService, BusinessProcess_BPInstanceAPIService, VRNotificationService, VRNavigationService, WhS_BP_CreateProcessResultEnum, BusinessProcess_BPInstanceService, WhS_CP_CodePrepAPIService, WhS_BE_SellingNumberPlanAPIService, WhS_CP_NumberingPlanDefinitionEnum, WhS_CP_CodePrepService) {
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
                $scope.isLoading = true;
                WhS_CP_CodePrepService.hasRunningProcessesForSellingNumberPlan(sellingNumberPlanId).then(function (response) {
                    $scope.isLoading = false;
                    if (!response.hasRunningProcesses) {
                        WhS_CP_CodePrepService.createNewProcess(sellingNumberPlanId, undefined, $scope.effectiveDate, undefined, false, $scope.onCodePreparationApplied).then(function () {
                            $scope.modalContext.closeModal();
                        }).catch(function (error) {
                            VRNotificationService.notifyException(error, $scope);
                        });
                    }
                    else {
                        VRNotificationService.showWarning("Cannot start process because another instance is still running");
                    }
                }).catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
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

   

    function createNewProcess() {
        var promiseDeffered = UtilsService.createPromiseDeferred();

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
            promiseDeffered.resolve();
        }).catch(function (error) {
            VRNotificationService.notifyException(error, $scope);
        });
        return promiseDeffered.promise;
    }
    function setTitle() {
        $scope.title = "Apply Numbering Plan State";
    }


    function loadEffectiveDate() {

    	return WhS_CP_CodePrepAPIService.GetCPEffectiveDateDayOffset().then(function (effectiveDateDayOffset) {
	        var effectiveDate = UtilsService.getDateFromDateTime(new Date());
    		effectiveDate.setDate(effectiveDate.getDate() + effectiveDateDayOffset);
            $scope.effectiveDate = effectiveDate;
        });
    }

};

appControllers.controller('WhS_CP_CodePreparationApplyStateEditorController', CodePreparationApplyStateEditorController);

})(appControllers);
