(function (appControllers) {

    "use strict";
    codePreparationService.$inject = ["VRNotificationService", "BusinessProcess_BPInstanceAPIService", "BusinessProcess_BPInstanceService", "UtilsService", "WhS_BE_SellingNumberPlanAPIService", "WhS_CP_NumberingPlanDefinitionEnum", "WhS_BP_CreateProcessResultEnum"];

    function codePreparationService(VRNotificationService, BusinessProcess_BPInstanceAPIService, BusinessProcess_BPInstanceService, UtilsService, WhS_BE_SellingNumberPlanAPIService, WhS_CP_NumberingPlanDefinitionEnum, WhS_BP_CreateProcessResultEnum) {

        function NotifyValidationWarning(message) {
            VRNotificationService.showWarning(message);
        }
        function hasRunningProcessesForSellingNumberPlan(SellingNumberingPlanId) {
            var entityIds = [];
            entityIds.push(SellingNumberingPlanId);
            var promiseDeferred = UtilsService.createPromiseDeferred();
            WhS_BE_SellingNumberPlanAPIService.GetSellingNumberPlan(SellingNumberingPlanId).then(function (response) {

                var runningInstanceEditorSettings = {
                    message: "Another processes of numbering plan for the selling number plan'" + response.Name + "' is still pending"
                };
                BusinessProcess_BPInstanceService.displayRunningInstancesIfExist(WhS_CP_NumberingPlanDefinitionEnum.BPDefinitionId.value, entityIds, runningInstanceEditorSettings).then(function (response) {
                    promiseDeferred.resolve(response);
                });
            }).catch(function (error) {
                VRNotificationService.notifyException(error);
            });
            return promiseDeferred.promise;
        }

        function createNewProcess(sellingNumberPlanId, fileId, effectiveDate, hasHeader, isFromExcel, onCodePreparationAction) {

            var inputArguments = {
                $type: "TOne.WhS.CodePreparation.BP.Arguments.CodePreparationInput, TOne.WhS.CodePreparation.BP.Arguments",
                SellingNumberPlanId: (sellingNumberPlanId!=undefined)?sellingNumberPlanId:undefined,
                FileId: (fileId!=undefined)?fileId:undefined,
                EffectiveDate: (effectiveDate!=undefined)?effectiveDate:undefined,
                HasHeader: (hasHeader!=undefined)?hasHeader:undefined,
                IsFromExcel: (isFromExcel!=undefined)?isFromExcel:undefined
            };
            var input = {
                InputArguments: inputArguments
            };

            return BusinessProcess_BPInstanceAPIService.CreateNewProcess(input).then(function (response) {
                if (response.Result == WhS_BP_CreateProcessResultEnum.Succeeded.value) {
                    var context = {
                        onClose: (onCodePreparationAction != undefined) ? onCodePreparationAction : undefined
                    }
                }

                BusinessProcess_BPInstanceService.openProcessTracking(response.ProcessInstanceId, context); 
            });
        }

        return ({
            NotifyValidationWarning: NotifyValidationWarning,
            hasRunningProcessesForSellingNumberPlan: hasRunningProcessesForSellingNumberPlan,
            createNewProcess: createNewProcess
           
        });
    }

    appControllers.service("WhS_CP_CodePrepService", codePreparationService);
})(appControllers);