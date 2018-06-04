app.service('Demo_Module_FamilyService', ['VRModalService', 'VRNotificationService',
function (VRModalService, VRNotificationService) {

    function addFamily(onFamilyAdded) {

        var settings = {};
        var parameters = {};
        console.log("adddd")

        settings.onScopeReady = function (modalScope) {
            modalScope.onFamilyAdded = onFamilyAdded;

        };

        VRModalService.showModal('/Client/Modules/Demo_Module/Views/FamilyEditor.html', parameters, settings);
    };

    function editFamily(familyId, onFamilyUpdated) {
        var settings = {};
        var parameters = {
            familyId: familyId
        };

        settings.onScopeReady = function (modalScope) {
            modalScope.onFamilyUpdated = onFamilyUpdated;
        };
        VRModalService.showModal('/Client/Modules/Demo_Module/Views/FamilyEditor.html', parameters, settings);
    };
 
    return {
        addFamily: addFamily,
        editFamily: editFamily,
    };

}]);