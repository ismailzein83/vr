app.service('Demo_BestPractices_ParentService', ['VRModalService', 'VRNotificationService',
function (VRModalService, VRNotificationService) {
   
    function addParent(onParentAdded) {

        var settings = {};
        var parameters = {};

        settings.onScopeReady = function (modalScope) {
            modalScope.onParentAdded = onParentAdded;

        };
        VRModalService.showModal('/Client/Modules/Demo_BestPractices/Elements/Parent/Views/ParentEditor.html', parameters, settings);
    };

    function editParent(parentId, onParentUpdated) {
        var settings = {};
        var parameters = {
            parentId: parentId
        };

        settings.onScopeReady = function (modalScope) {
            modalScope.onParentUpdated = onParentUpdated;

        };
        VRModalService.showModal('/Client/Modules/Demo_BestPractices/Elements/Parent/Views/ParentEditor.html', parameters, settings);
    };

    return {
        addParent: addParent,
        editParent: editParent,
    };

}]);