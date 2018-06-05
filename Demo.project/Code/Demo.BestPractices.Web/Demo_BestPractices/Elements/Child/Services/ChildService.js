app.service('Demo_BestPractices_ChildService', ['VRModalService', 'VRNotificationService',
function (VRModalService, VRNotificationService) {

    function addChild(onChildAdded) {

        var settings = {};
        var parameters = {};

        settings.onScopeReady = function (modalScope) {
            modalScope.onChildAdded = onChildAdded;

        };
        VRModalService.showModal('/Client/Modules/Demo_BestPractices/Elements/Child/Views/ChildEditor.html', parameters, settings);
    };

    function editChild(childId, onChildUpdated) {
        var settings = {};
        var parameters = {
            childId: childId
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