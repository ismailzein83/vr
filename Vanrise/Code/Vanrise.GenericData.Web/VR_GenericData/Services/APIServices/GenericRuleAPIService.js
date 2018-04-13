(function (appControllers) {

    'use strict';

    GenericRuleAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_GenericData_ModuleConfig'];

    function GenericRuleAPIService(BaseAPIService, UtilsService, VR_GenericData_ModuleConfig) {
        return {
            GetFilteredGenericRules: GetFilteredGenericRules,
            GetGenericRule: GetGenericRule,
            AddGenericRule: AddGenericRule,            
            DoesUserHaveAddAccess: DoesUserHaveAddAccess,
            UpdateGenericRule: UpdateGenericRule,
            DoesUserHaveEditAccess:DoesUserHaveEditAccess,
            DeleteGenericRule: DeleteGenericRule,
            DownloadGenericRulesTemplate: DownloadGenericRulesTemplate,
            DoesRuleSupportUpload: DoesRuleSupportUpload,
            UploadGenericRules: UploadGenericRules,
            DownloadUploadGenericRulesOutput: DownloadUploadGenericRulesOutput
        };

        function GetFilteredGenericRules(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, 'GenericRule', 'GetFilteredGenericRules'), input);
        }

        function GetGenericRule(ruleDefinitionId, ruleId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, 'GenericRule', 'GetGenericRule'), {
                ruleDefinitionId: ruleDefinitionId,
                ruleId: ruleId
            });
        }

        function AddGenericRule(genericRule) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, 'GenericRule', 'AddGenericRule'), genericRule);
        }
        function DoesUserHaveAddAccess(ruleDefinitionId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, 'GenericRule', 'DoesUserHaveAddAccess'), {
                ruleDefinitionId: ruleDefinitionId
            });
        }
        function UpdateGenericRule(genericRule) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, 'GenericRule', 'UpdateGenericRule'), genericRule);
        }
        function DoesUserHaveEditAccess(ruleDefinitionId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, 'GenericRule', 'DoesUserHaveEditAccess'),{
                    ruleDefinitionId: ruleDefinitionId
            }, {
                useCache: true
            });
        }
        function DeleteGenericRule(genericRule) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, 'GenericRule', 'DeleteGenericRule'), genericRule);
        }

        function DownloadGenericRulesTemplate(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, 'GenericRule', "DownloadGenericRulesTemplate"), input,
            {
            returnAllResponseParameters: true,
            responseTypeAsBufferArray: true
            });
        }

        function DoesRuleSupportUpload(ruleDefinitionId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, 'GenericRule', 'DoesRuleSupportUpload'), {
                ruleDefinitionId: ruleDefinitionId
            });
        }
        
        function UploadGenericRules(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, 'GenericRule', 'UploadGenericRules'), input);
        }

        function DownloadUploadGenericRulesOutput(fileId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, 'GenericRule', 'DownloadUploadGenericRulesOutput'), {
                    fileId: fileId
            },
            {
            returnAllResponseParameters: true,
                    responseTypeAsBufferArray: true
            });
        }
    }

    appControllers.service('VR_GenericData_GenericRuleAPIService', GenericRuleAPIService);

})(appControllers);