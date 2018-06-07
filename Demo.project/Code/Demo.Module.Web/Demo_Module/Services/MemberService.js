app.service('Demo_Module_MemberService', ['VRModalService', 'VRNotificationService',
function (VRModalService, VRNotificationService) {

    function addMember(onMemberAdded, familyIdItem) {

        var settings = {};
        var parameters = {
            familyIdItem: familyIdItem
        };

        settings.onScopeReady = function (modalScope) {
            modalScope.onMemberAdded = onMemberAdded;

        };
        VRModalService.showModal('/Client/Modules/Demo_Module/Views/MemberEditor.html', parameters, settings);
    };

    function editMember(memberId, onMemberUpdated, familyIdItem) {
        var settings = {};
        var parameters = {
            memberId: memberId,
            familyIdItem: familyIdItem
        };

        settings.onScopeReady = function (modalScope) {
            modalScope.onMemberUpdated = onMemberUpdated;

        };
        VRModalService.showModal('/Client/Modules/Demo_Module/Views/MemberEditor.html', parameters, settings);
    };

    return {
        addMember: addMember,
        editMember: editMember,
    };

}]);