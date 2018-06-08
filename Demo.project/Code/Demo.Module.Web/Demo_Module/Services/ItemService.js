app.service('Demo_Module_ItemService', ['VRModalService', 'Demo_Module_ItemAPIService', 'VRNotificationService',
function (VRModalService, Demo_Module_ItemAPIService, VRNotificationService) {
   
    function addItem(onItemAdded, productIdItem) {
      
        var settings = {};
        var parameters = {
            productIdItem: productIdItem
        };

        settings.onScopeReady = function (modalScope) {
            modalScope.onItemAdded = onItemAdded;
        };

        VRModalService.showModal('/Client/Modules/Demo_Module/Views/ItemEditor.html', parameters, settings);
    }
    function editItem(itemId, onItemUpdated ,productIdItem) {

        var settings = {
        };
        var parameters = {
            itemId: itemId,
            productIdItem: productIdItem
        };
        settings.onScopeReady = function (modalScope) {
            modalScope.onItemUpdated = onItemUpdated;
        };


        VRModalService.showModal('/Client/Modules/Demo_Module/Views/ItemEditor.html', parameters, settings);
    }
   
    return ({
        editItem: editItem,
        addItem: addItem
    });

}]);