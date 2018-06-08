(function (appControllers) {

    'use strict';

    ItemAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Demo_Module_ModuleConfig'];

    function ItemAPIService(BaseAPIService, UtilsService, Demo_Module_ModuleConfig) {

        var controller = 'Item';


        function GetFilteredItems(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "GetFilteredItems"), input);
        }

        function GetItemById(itemId) {
            return BaseAPIService.get(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, 'GetItemById'),
                { itemId: itemId }
                );
        }

        function AddItem(item) {
            console.log(item);
            return BaseAPIService.post(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "AddItem"), item);
        }
        function UpdateItem(item) {
            return BaseAPIService.post(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "UpdateItem"), item);
        }
     
        function GetItemShapeConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "GetItemShapeConfigs"));
        }

        return ({
            AddItem: AddItem,
            UpdateItem: UpdateItem,
            GetFilteredItems: GetFilteredItems,
            GetItemById: GetItemById,
            GetItemShapeConfigs: GetItemShapeConfigs
        });
    }


    appControllers.service('Demo_Module_ItemAPIService', ItemAPIService);
})(appControllers);