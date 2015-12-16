(function (appControllers) {

    "use strict";
    defineCDRFieldsAPIService.$inject = ['BaseAPIService', 'UtilsService', 'WhS_CDRProcessing_ModuleConfig'];

    function defineCDRFieldsAPIService(BaseAPIService, UtilsService, WhS_CDRProcessing_ModuleConfig) {

        function GetFilteredCDRFields(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_CDRProcessing_ModuleConfig.moduleName, "DefineCDRFields", "GetFilteredCDRFields"), input);
        }

        function UpdateCDRField(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_CDRProcessing_ModuleConfig.moduleName, "DefineCDRFields", "UpdateCDRField"), input);
        }

        function AddCDRField(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_CDRProcessing_ModuleConfig.moduleName, "DefineCDRFields", "AddCDRField"), input);
        }
        return ({
            GetFilteredCDRFields: GetFilteredCDRFields,
            UpdateCDRField: UpdateCDRField,
            AddCDRField: AddCDRField
        });
    }

    appControllers.service('WhS_CDRProcessing_DefineCDRFieldsAPIService', defineCDRFieldsAPIService);
})(appControllers);