(function (appControllers) {

    "use strict";

    parentService.$inject = ['VRModalService'];

    function parentService(VRModalService) {

        function addParent(onParentAdded) {

            var parameters = {};

            var settings = {};
            settings.onScopeReady = function (modalScope) {
                modalScope.onParentAdded = onParentAdded;
            };
            VRModalService.showModal('/Client/Modules/Demo_BestPractices/Elements/Parent/Views/ParentEditor.html', parameters, settings);
        };

        function editParent(onParentUpdated, parentId) {

            var parameters = {
                parentId: parentId
            };

            var settings = {};
            settings.onScopeReady = function (modalScope) {
                modalScope.onParentUpdated = onParentUpdated;
            };
            VRModalService.showModal('/Client/Modules/Demo_BestPractices/Elements/Parent/Views/ParentEditor.html', parameters, settings);
        };

        return {
            addParent: addParent,
            editParent: editParent
        };
    };

    appControllers.service("Demo_BestPractices_ParentService", parentService);
})(appControllers);
