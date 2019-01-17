(function (appControllers) { // Edit/Add functions of branch grid

    "use strict";

    branchService.$inject = ['VRModalService'];

    function branchService(VRModalService) {

        function addBranch(onBranchAdded, companyIdItem) {

            var parameters = {
                companyIdItem: companyIdItem
            };

            var settings = {};
            settings.onScopeReady = function (modalScope) {
                modalScope.onBranchAdded = onBranchAdded;
            };

            VRModalService.showModal('/Client/Modules/Demo_Module/Elements/Branch/Views/BranchEditor.html', parameters, settings);
        };

        function editBranch(onBranchUpdated, branchId,companyIdItem) {

            var parameters = {
                branchId: branchId,
                companyIdItem: companyIdItem
            };

            var settings = {};
            settings.onScopeReady = function (modalScope) {
                modalScope.onBranchUpdated = onBranchUpdated;
            };
            VRModalService.showModal('/Client/Modules/Demo_Module/Elements/Branch/Views/BranchEditor.html', parameters, settings);

        };

        function addDepartment(onDepartmentAdded, branchItem) {
            var parameters = {
                branchItem: branchItem
            };

            var settings = {};
            settings.onScopeReady = function (modalScope) {
                modalScope.onDepartmentAdded = onDepartmentAdded;
            };
            VRModalService.showModal('/Client/Modules/Demo_Module/Elements/Branch/Views/DepartmentEditor.html', parameters, settings);
        };

        function editDepartment(onDepartmentUpdated, departmentId, branchItem) {

            var parameters = {
                departmentId: departmentId,
                branchItem: branchItem
            };

            var settings = {};
            settings.onScopeReady = function (modalScope) {
                modalScope.onDepartmentUpdated = onDepartmentUpdated;
            };
            VRModalService.showModal('/Client/Modules/Demo_Module/Elements/Branch/Views/DepartmentEditor.html', parameters, settings);
        };

        function addEmployee(onEmployeeAdded, departmentItem) {
            var parameters = {
                departmentItem: departmentItem
            };

            var settings = {};
            settings.onScopeReady = function (modalScope) {
                modalScope.onEmployeeAdded = onEmployeeAdded;
            };

            VRModalService.showModal('/Client/Modules/Demo_Module/Elements/Branch/Views/EmployeeEditor.html', parameters, settings);
        };

        function editEmployee(onEmployeeUpdated, employeeId, departmentItem) {

            var parameters = {
                employeeId: employeeId,
                departmentItem: departmentItem
            };

            var settings = {};
            settings.onScopeReady = function (modalScope) {
                modalScope.onEmployeeUpdated = onEmployeeUpdated;
            };

            VRModalService.showModal('/Client/Modules/Demo_Module/Elements/Branch/Views/EmployeeEditor.html', parameters, settings);
        };

        return {
            addBranch: addBranch,
            editBranch: editBranch,
            addDepartment: addDepartment,
            editDepartment: editDepartment,
            addEmployee: addEmployee,
            editEmployee: editEmployee
        };
    };

    appControllers.service("Demo_Module_BranchService", branchService);
})(appControllers);