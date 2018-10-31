app.service('Demo_Module_StudentService', ['VRModalService', 'VRNotificationService',
function (VRModalService, VRNotificationService) {

    function addStudent(onStudentAdded,myFunction) {

        var settings = {};
        var parameters = {
            
        };

        settings.onScopeReady = function (modalScope) {
            modalScope.onStudentAdded = onStudentAdded;
            modalScope.myFunction = myFunction;

        };
        VRModalService.showModal('/Client/Modules/Demo_Module/Views/StudentEditor.html', parameters, settings);
    };

    function editStudent(studentId, onStudentUpdated) {
        var settings = {};
        var parameters = {
            studentId: studentId
        };

        settings.onScopeReady = function (modalScope) {
            modalScope.onStudentUpdated = onStudentUpdated;

        };
        VRModalService.showModal('/Client/Modules/Demo_Module/Views/StudentEditor.html', parameters, settings);
    };


    return {
        addStudent: addStudent,
        editStudent: editStudent,
    };

}]);