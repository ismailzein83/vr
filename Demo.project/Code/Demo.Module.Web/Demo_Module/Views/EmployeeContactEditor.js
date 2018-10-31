(function (appControllers) {
    "use strict";
    employeeEditorController.$inject = ['$scope', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService', 'Demo_Module_EmployeeService'];

    function employeeEditorController($scope, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService, Demo_Module_EmployeeService) {

        var isEditMode;
        var employeeContactEntity = {};
        $scope.scopeModel = {};

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                employeeContactEntity = parameters.employeeContactEntity;
            }
            isEditMode = (employeeContactEntity != undefined);
        }

        function defineScope() {
            $scope.scopeModel.saveEmployeeContact = function () {
                if (isEditMode)
                    return updateEmployeeContact();
                else
                    return insertEmployeeContact();
            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };
        }

        function load() {
            $scope.scopeModel.isLoading = true;
            if (isEditMode) {
               
                    loadAllControls().finally(function () {console.log("")
                        employeeContactEntity = undefined;
                    });
               
            }
            else
                loadAllControls();
            
        }
        function loadAllControls()
        {
            function setTitle() {
                if (isEditMode && employeeContactEntity != undefined)
                    $scope.title = UtilsService.buildTitleForUpdateEditor(employeeContactEntity.index + 1, "Employee Contact Number");
                else
                    $scope.title = UtilsService.buildTitleForAddEditor("Employee Contact");
            }

            function loadStaticData() {

                if (employeeContactEntity != undefined) {
                    $scope.scopeModel.email = employeeContactEntity.Email;
                    $scope.scopeModel.phoneNumber = employeeContactEntity.PhoneNumber;
                }
            }

            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            })
           .finally(function () {console.log("finallu")
               $scope.scopeModel.isLoading = false;
           });
        }


        function buildEmployeeContactObjectFromScope() {
            return {
                Email: $scope.scopeModel.email,
                PhoneNumber: $scope.scopeModel.phoneNumber
            };
        }

        function insertEmployeeContact() {
            if ($scope.onEmployeeContactAdded != undefined) {
                $scope.onEmployeeContactAdded(buildEmployeeContactObjectFromScope());
            }
            $scope.modalContext.closeModal();
        }
 
        function updateEmployeeContact() {
            if ($scope.onEmployeeContactUpdated != undefined) {
                $scope.onEmployeeContactUpdated(buildEmployeeContactObjectFromScope());
            }
            $scope.modalContext.closeModal();
        }

     };
    appControllers.controller('Demo_Module_EmployeeContactEditorController', employeeEditorController);
})(appControllers);