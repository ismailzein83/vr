
app.service('Demo_Module_StudentService', ['VRModalService', 'Demo_Module_StudentAPIService', 'VRNotificationService',
function (VRModalService, Demo_Module_StudentAPIService, VRNotificationService) {
    var drillDownDefinitions = [];
    return ({
        editStudent: editStudent,
        addStudent: addStudent,
        deleteStudent: deleteStudent

    });
    function editStudent(studentId, onStudentUpdated) {
        var settings = {
        };
        var parameters = {
            studentId: studentId
        };
        settings.onScopeReady = function (modalScope) {
            modalScope.onStudentUpdated = onStudentUpdated;
        };


        VRModalService.showModal('/Client/Modules/Demo_Module/Views/StudentEditor.html', parameters, settings);
    }
    function deleteStudent(scope, dataItem, onStudentDeleted) {
        VRNotificationService.showConfirmation().then(function (confirmed) {
            if (confirmed) {
                return Demo_Module_StudentAPIService.DeleteStudent(dataItem.Entity.StudentId).then(function (responseObject) {
                    var deleted = VRNotificationService.notifyOnItemDeleted('Student', responseObject);

                    if (deleted && onStudentDeleted && typeof onStudentDeleted == 'function') {
                        onStudentDeleted(dataItem);
                    }
                }).catch(function (error) {
                    VRNotificationService.notifyException(error, scope);
                })
            }
        });
    }
    function addStudent(onStudentAdded) {
        var settings = {
        };

        settings.onScopeReady = function (modalScope) {
            modalScope.onStudentAdded = onStudentAdded;
        };
        var parameters = {};


        VRModalService.showModal('/Client/Modules/Demo_Module/Views/StudentEditor.html', parameters, settings);
    }
}]);