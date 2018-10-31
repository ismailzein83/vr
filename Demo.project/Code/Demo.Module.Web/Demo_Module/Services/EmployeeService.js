app.service('Demo_Module_EmployeeService', ['VRModalService', 'VRNotificationService',
function (VRModalService, VRNotificationService) {

    function addEmployee(onEmployeeAdded) {

        var settings = {};
        var parameters = {

        };

        settings.onScopeReady = function (modalScope) {
            modalScope.onEmployeeAdded = onEmployeeAdded;

        };
        VRModalService.showModal('/Client/Modules/Demo_Module/Views/EmployeeEditor.html', parameters, settings);
    };

    function editEmployee(employeeId, onEmployeeUpdated) {
        var settings = {};
        var parameters = {
            employeeId: employeeId
        };

        settings.onScopeReady = function (modalScope) {
            modalScope.onEmployeeUpdated = onEmployeeUpdated;

        };
        VRModalService.showModal('/Client/Modules/Demo_Module/Views/EmployeeEditor.html', parameters, settings);
    };

    function addEmployeeContact(onEmployeeContactAdded) {

        var settings = {};
        var parameters = {

        };
        settings.onScopeReady = function (modalScope) {
            modalScope.onEmployeeContactAdded = onEmployeeContactAdded;

        };

        VRModalService.showModal('/Client/Modules/Demo_Module/Views/EmployeeContactEditor.html', parameters, settings);

    }

    function editEmployeeContact(onEmployeeContactUpdated, employeeContactEntity) {

        var settings = {};
        var parameters = { employeeContactEntity: employeeContactEntity };
        settings.onScopeReady = function (modalScope) {
            modalScope.onEmployeeContactUpdated = onEmployeeContactUpdated;

        };
        VRModalService.showModal('/Client/Modules/Demo_Module/Views/EmployeeContactEditor.html', parameters, settings);

    }
   
    
    return {
        addEmployee: addEmployee,
        editEmployee: editEmployee,
        addEmployeeContact: addEmployeeContact,
        editEmployeeContact: editEmployeeContact
        
    };

}]);