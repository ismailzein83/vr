(function (appControllers) {

    "use strict";

    ExcludedItemsService.$inject = ['VRModalService'];

    function ExcludedItemsService(VRModalService) {

        function viewExcludedItems(subscriberProcessInstanceId) {
            console.log("in service");
            var settings = {};
            var parameters = {
                subscriberProcessInstanceId: subscriberProcessInstanceId,
            };
            
            VRModalService.showModal('/Client/Modules/WhS_Sales/Views/ExcludedItem.html', parameters, settings);
        };

        return {
            viewExcludedItems: viewExcludedItems,
        };
    }

    appControllers.service('WhS_Sales_ExcludedItemsService', ExcludedItemsService);

})(appControllers);