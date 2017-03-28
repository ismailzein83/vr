(function (appControllers) {

    "use strict";

    DAProfCalcNotitificationAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_Analytic_ModuleConfig'];

    function DAProfCalcNotitificationAPIService(BaseAPIService, UtilsService, VR_Analytic_ModuleConfig) {

        var controllerName = "DAProfCalcNotification";

        function GetDAProfCalcNotificationTypeId(alertRuleTypeId, dataAnalysisItemDefinitionId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Analytic_ModuleConfig.moduleName, controllerName, 'GetDAProfCalcNotificationTypeId'), {
                alertRuleTypeId: alertRuleTypeId,
                dataAnalysisItemDefinitionId: dataAnalysisItemDefinitionId
            });
        };

        return ({
            GetDAProfCalcNotificationTypeId: GetDAProfCalcNotificationTypeId
        });
    }

    appControllers.service('VR_Analytic_DAProfCalcNotificationAPIService', DAProfCalcNotitificationAPIService);
})(appControllers);