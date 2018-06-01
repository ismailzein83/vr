app.service('Demo_Module_ItemService', ['VRModalService', 'Demo_Module_ItemAPIService', 'VRNotificationService',
function (VRModalService, Demo_Module_ItemAPIService, VRNotificationService) {
    var drillDownDefinitions = [];
    return ({
        editItem: editItem,
        addItem: addItem,
        deleteItem: deleteItem

    });
    function addItem(onItemAdded) {
        var settings = {
        };

        settings.onScopeReady = function (modalScope) {
            modalScope.onItemAdded = onItemAdded;
        };
        var parameters = {};


        VRModalService.showModal('/Client/Modules/Demo_Module/Views/ItemEditor.html', parameters, settings);
    }
    function editItem(itemId, itemProduct, onItemUpdated) {
        var settings = {
        };
        var parameters = {
            itemId: itemId,
            itemProduct: itemProduct

        };
        settings.onScopeReady = function (modalScope) {
            modalScope.onItemUpdated = onItemUpdated;
        };


        VRModalService.showModal('/Client/Modules/Demo_Module/Views/ItemEditor.html', parameters, settings);
    }
    function deleteItem(scope, dataItem, onItemDeleted) {
        VRNotificationService.showConfirmation().then(function (confirmed) {
            if (confirmed) {
                return Demo_Module_ItemAPIService.DeleteItem(dataItem.ItemId).then(function (responseObject) {
                    var deleted = VRNotificationService.notifyOnItemDeleted('Item', responseObject);

                    if (deleted && onItemDeleted && typeof onItemDeleted == 'function') {
                        onItemDeleted(dataItem);
                    }
                }).catch(function (error) {
                    VRNotificationService.notifyException(error, scope);
                })
            }
        });
    }

}]);