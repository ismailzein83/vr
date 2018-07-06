app.service('Demo_Module_MasterService', ['VRModalService', 'VRNotificationService',
function (VRModalService, VRNotificationService) {

    function addMaster(onMasterAdded) {

        var settings = {};
        var parameters = {};

        settings.onScopeReady = function (modalScope) {
            modalScope.onMasterAdded = onMasterAdded;

        };
        VRModalService.showModal('/Client/Modules/Demo_Module/Elements/Master/Views/MasterEditor.html', parameters, settings);
    };

    function editMaster(masterId, onMasterUpdated) {
        var settings = {};
        var parameters = {
            masterId: masterId
        };

        settings.onScopeReady = function (modalScope) {
            modalScope.onMasterUpdated = onMasterUpdated;

        };
        VRModalService.showModal('/Client/Modules/Demo_Module/Elements/Master/Views/MasterEditor.html', parameters, settings);
    };

    return {
        addMaster: addMaster,
        editMaster: editMaster,
    };

}]);