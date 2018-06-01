(function (appControllers) {

    'use strict';

    ItemAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Demo_Module_ModuleConfig'];

    function ItemAPIService(BaseAPIService, UtilsService, Demo_Module_ModuleConfig) {

        var controller = 'Item';


        function GetFilteredItems(input) {
            console.log("as");
            return BaseAPIService.post(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "GetFilteredItems"), input);
        }

        function GetItemById(Id) {
            return BaseAPIService.get(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, 'GetItemById'),
                { itemId: Id }
                );
        }

        function AddItem(Item) {
            return BaseAPIService.post(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "AddItem"), Item);
        }
        function UpdateItem(Item) {
            return BaseAPIService.post(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "UpdateItem"), Item);
        }
        function DeleteItem(Id) {
            return BaseAPIService.get(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, 'DeleteItem'), {
                ItemId: Id
            });
        }

        return ({
            AddItem: AddItem,
            UpdateItem: UpdateItem,
            DeleteItem: DeleteItem,
            GetFilteredItems: GetFilteredItems,
            GetItemById: GetItemById
        });
    }


    appControllers.service('Demo_Module_ItemAPIService', ItemAPIService);
})(appControllers);