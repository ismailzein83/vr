(function (appControllers) {
    "use strict";
    employeeFaxEditorController.$inject = ['$scope', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService', 'Demo_Module_EmployeeService'];

    function employeeFaxEditorController($scope, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService, Demo_Module_EmployeeService) {

        var isEditMode;
        $scope.scopeModel = {};
        var employeeContactFax = {};






    };
    appControllers.controller('Demo_Module_EmployeeContactEditorController', employeeFaxEditorController);
})(appControllers);