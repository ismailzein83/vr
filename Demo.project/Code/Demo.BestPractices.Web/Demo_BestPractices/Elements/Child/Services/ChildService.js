app.service('Demo_BestPractices_ChildService', ['VRModalService', 'VRNotificationService',
function (VRModalService, VRNotificationService) {

    function addChild(onChildAdded, parentIdItem) {

        var settings = {};
        var parameters  = {
            parentIdItem:parentIdItem
        };

        settings.onScopeReady = function (modalScope) {
            modalScope.onChildAdded = onChildAdded;

        };
        VRModalService.showModal('/Client/Modules/Demo_BestPractices/Elements/Child/Views/ChildEditor.html', parameters, settings);
    };

    function editChild(childId, onChildUpdated, parentIdItem) {
        var settings = {};
        var parameters = {
            childId: childId,
            parentIdItem: parentIdItem
        };

        settings.onScopeReady = function (modalScope) {
            modalScope.onChildUpdated = onChildUpdated;

        };
        VRModalService.showModal('/Client/Modules/Demo_BestPractices/Elements/Child/Views/ChildEditor.html', parameters, settings);
    };

    return {
        addChild: addChild,
        editChild: editChild,
    };

}]);