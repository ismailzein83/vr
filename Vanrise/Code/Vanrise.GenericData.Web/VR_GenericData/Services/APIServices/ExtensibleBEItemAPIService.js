(function (appControllers) {

    'use strict';

    ExtensibleBEItemAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_GenericData_ModuleConfig'];

    function ExtensibleBEItemAPIService(BaseAPIService, UtilsService, VR_GenericData_ModuleConfig) {
        return {
            GetExtensibleBEItem: GetExtensibleBEItem,
            AddExtensibleBEItem: AddExtensibleBEItem,
            GetFilteredExtensibleBEItems: GetFilteredExtensibleBEItems,
            UpdateExtensibleBEItem: UpdateExtensibleBEItem,
        };

        function GetExtensibleBEItem(extensibleBEItemId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, 'ExtensibleBEItem', 'GetExtensibleBEItem'), { extensibleBEItemId: extensibleBEItemId });
        }
        function AddExtensibleBEItem(extensibleBEItem) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, 'ExtensibleBEItem', 'AddExtensibleBEItem'), extensibleBEItem);
        }

        function UpdateExtensibleBEItem(extensibleBEItem) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, 'ExtensibleBEItem', 'UpdateExtensibleBEItem'), extensibleBEItem);
        }
        function GetFilteredExtensibleBEItems(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, 'ExtensibleBEItem', 'GetFilteredExtensibleBEItems'), input);
        }

    }

    appControllers.service('VR_GenericData_ExtensibleBEItemAPIService', ExtensibleBEItemAPIService);

})(appControllers);