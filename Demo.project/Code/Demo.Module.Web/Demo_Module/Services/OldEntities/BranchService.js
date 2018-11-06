app.service('Demo_Module_BranchService', ['VRModalService', 'Demo_Module_BranchAPIService', 'VRNotificationService',
function (VRModalService, Demo_Module_BranchAPIService, VRNotificationService) {
    return {
        addBranch: addBranch,
        editBranch: editBranch
        
    };

    function addBranch(onBranchAdded) {
        var settings = {};
        var parameters = {};

        settings.onScopeReady = function (modalScope) {
            modalScope.onBranchAdded = onBranchAdded;

        };
        VRModalService.showModal('/Client/Modules/Demo_Module/Views/BranchEditor.html', parameters, settings);
    };
    function editBranch(branchId, onBranchUpdated) {
        var settings = {};
        var parameters = {
            branchId: branchId
        };

        settings.onScopeReady = function (modalScope) {
            modalScope.onBranchUpdated = onBranchUpdated;

        };
        VRModalService.showModal('/Client/Modules/Demo_Module/Views/BranchEditor.html', parameters, settings);
    };



}]);