
(function (appControllers) {

    "use strict";
    VRAlertRuleTypeAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_Notification_ModuleConfig', 'SecurityService'];

    function VRAlertRuleTypeAPIService(BaseAPIService, UtilsService, VR_Notification_ModuleConfig, SecurityService) {

        var controllerName = "VRAlertRuleType";


        function GetFilteredVRAlertRuleTypes(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Notification_ModuleConfig.moduleName, controllerName, 'GetFilteredVRAlertRuleTypes'), input);
        }

        function GetVRAlertRuleType(vrAlertRuleTypeId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Notification_ModuleConfig.moduleName, controllerName, 'GetVRAlertRuleType'), {
                VRAlertRuleTypeId: vrAlertRuleTypeId
            });
        }

        function AddVRAlertRuleType(vrAlertRuleTypeItem) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Notification_ModuleConfig.moduleName, controllerName, 'AddVRAlertRuleType'), vrAlertRuleTypeItem);
        }

        function UpdateVRAlertRuleType(vrAlertRuleTypeItem) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Notification_ModuleConfig.moduleName, controllerName, 'UpdateVRAlertRuleType'), vrAlertRuleTypeItem);
        }


        //function HasAddVRAlertRuleTypePermission() {
        //    return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(VR_Notification_ModuleConfig.moduleName, controllerName, ['AddVRAlertRuleType']));
        //}

        //function HasUpdateVRAlertRuleTypePermission() {
        //    return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(VR_Notification_ModuleConfig.moduleName, controllerName, ['UpdateVRAlertRuleType']));
        //}

        //function GetStyleFormatingExtensionConfigs() {
        //    return BaseAPIService.get(UtilsService.getServiceURL(VR_Notification_ModuleConfig.moduleName, controllerName, "GetStyleFormatingExtensionConfigs"));
        //}

        //function GetVRAlertRuleTypesInfo(filter) {
        //    return BaseAPIService.get(UtilsService.getServiceURL(VR_Notification_ModuleConfig.moduleName, controllerName, "GetVRAlertRuleTypesInfo"), {
        //        filter: filter
        //    });
        //}


        return ({
            GetFilteredVRAlertRuleTypes: GetFilteredVRAlertRuleTypes,
            GetVRAlertRuleType: GetVRAlertRuleType,
            AddVRAlertRuleType: AddVRAlertRuleType,
            HasAddVRAlertRuleTypePermission: HasAddVRAlertRuleTypePermission,
            //HasUpdateVRAlertRuleTypePermission: HasUpdateVRAlertRuleTypePermission,
            //UpdateVRAlertRuleType: UpdateVRAlertRuleType,
            //GetStyleFormatingExtensionConfigs: GetStyleFormatingExtensionConfigs,
            //GetVRAlertRuleTypesInfo: GetVRAlertRuleTypesInfo
        });
    }

    appControllers.service('VR_Notification_VRAlertRuleTypeAPIService', VRAlertRuleTypeAPIService);

})(appControllers);