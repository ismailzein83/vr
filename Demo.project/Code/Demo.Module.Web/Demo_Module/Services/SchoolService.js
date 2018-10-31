app.service('Demo_Module_SchoolService', ['VRModalService', 'VRNotificationService',
function (VRModalService, VRNotificationService) {

    function addSchool(onSchoolAdded) {

        var settings = {};
        var parameters = {};

        settings.onScopeReady = function (modalScope) {
            modalScope.onSchoolAdded = onSchoolAdded;

        };
        VRModalService.showModal('/Client/Modules/Demo_Module/Views/SchoolEditor.html', parameters, settings);
    };

    function editSchool(schoolId, onSchoolUpdated) {
        var settings = {};
        var parameters = {
            schoolId: schoolId
        };

        settings.onScopeReady = function (modalScope) {
            modalScope.onSchoolUpdated = onSchoolUpdated;

        };
        VRModalService.showModal('/Client/Modules/Demo_Module/Views/SchoolEditor.html', parameters, settings);
    };

    return {
        addSchool: addSchool,
        editSchool: editSchool,
    };

}]);