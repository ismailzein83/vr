(function (appControllers) {

    "use strict";

    childService.$inject = ['VRModalService'];

    function childService(VRModalService) {

        function addChild(onChildAdded, parentIdItem) {

            var parameters = {
                parentIdItem: parentIdItem
            };

            var settings = {};
            settings.onScopeReady = function (modalScope) {
                modalScope.onChildAdded = onChildAdded;
            };
            VRModalService.showModal('/Client/Modules/Demo_BestPractices/Elements/Child/Views/ChildEditor.html', parameters, settings);
        };

        function editChild(onChildUpdated, childId, parentIdItem) {

            var parameters = {
                childId: childId,
                parentIdItem: parentIdItem
            };

            var settings = {};
            settings.onScopeReady = function (modalScope) {
                modalScope.onChildUpdated = onChildUpdated;
            };
            VRModalService.showModal('/Client/Modules/Demo_BestPractices/Elements/Child/Views/ChildEditor.html', parameters, settings);
        };

        return {
            addChild: addChild,
            editChild: editChild
        };
    };

    appControllers.service("Demo_BestPractices_ChildService", childService);
})(appControllers);