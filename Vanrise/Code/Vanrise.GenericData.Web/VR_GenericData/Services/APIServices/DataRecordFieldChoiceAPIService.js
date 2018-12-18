(function (appControllers) {

    'use strict';

    DataRecordFieldChoiceAPIService.$inject = ['BaseAPIService', 'UtilsService', 'SecurityService', 'VR_GenericData_ModuleConfig'];

    function DataRecordFieldChoiceAPIService(BaseAPIService, UtilsService, SecurityService, VR_GenericData_ModuleConfig) {
        var controllerName = 'DataRecordFieldChoice';


        function GetDataRecordFieldChoicesInfo(input) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, controllerName, 'GetDataRecordFieldChoicesInfo'), input);
        }

        function GetFilteredDataRecordFieldChoices(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, controllerName, 'GetFilteredDataRecordFieldChoices'), input);
        }

        function GetDataRecordFieldChoice(dataRecordFieldChoiceId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, controllerName, 'GetDataRecordFieldChoice'), {
                dataRecordFieldChoiceId: dataRecordFieldChoiceId
            });
        }

        function AddDataRecordFieldChoice(dataRecordFieldChoice) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, controllerName, 'AddDataRecordFieldChoice'), dataRecordFieldChoice);
        }
        function HasAddDataRecordFieldChoice() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(VR_GenericData_ModuleConfig.moduleName, controllerName, ['AddDataRecordFieldChoice']));
        }
        function UpdateDataRecordFieldChoice(dataRecordFieldChoice) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, controllerName, 'UpdateDataRecordFieldChoice'), dataRecordFieldChoice);
        }
        function HasUpdateDataRecordFieldChoice() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(VR_GenericData_ModuleConfig.moduleName, controllerName, ['UpdateDataRecordFieldChoice']));
        }


        return {
            GetDataRecordFieldChoicesInfo: GetDataRecordFieldChoicesInfo,
            GetFilteredDataRecordFieldChoices: GetFilteredDataRecordFieldChoices,
            GetDataRecordFieldChoice: GetDataRecordFieldChoice,
            AddDataRecordFieldChoice: AddDataRecordFieldChoice,
            HasAddDataRecordFieldChoice: HasAddDataRecordFieldChoice,
            UpdateDataRecordFieldChoice: UpdateDataRecordFieldChoice,
            HasUpdateDataRecordFieldChoice: HasUpdateDataRecordFieldChoice
        };
    }

    appControllers.service('VR_GenericData_DataRecordFieldChoiceAPIService', DataRecordFieldChoiceAPIService);

})(appControllers);