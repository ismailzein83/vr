(function (appControllers) {

    "use strict";

    employeeEditorController.$inject = ['$scope', 'Demo_Module_BranchAPIService', 'VRNavigationService', 'UtilsService', 'VRNotificationService', 'VRUIUtilsService'];

    function employeeEditorController($scope, Demo_Module_BranchAPIService, VRNavigationService, UtilsService, VRNotificationService, VRUIUtilsService) {

        var isEditMode;
        var employeeId;
        var departmentEntity;
        var index;

        var employeeSettingsAPI;
        var employeeSettingsGridReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null) {
                departmentEntity = parameters.departmentItem;
                employeeId = parameters.employeeId;
            }

            isEditMode = (employeeId != undefined);
        };

        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.saveEmployee = function () {
                if (isEditMode)
                    EditEmployee();
                else
                    AddEmployee();
            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };

            $scope.scopeModel.onEmployeeSettingsDirectiveReady = function (api) {
                employeeSettingsAPI = api;
                employeeSettingsGridReadyDeferred.resolve();
            };
        };

        function load() {
            $scope.scopeModel.isLoading = true;
            loadAllControls();
        };

        function AddEmployee() {
            var employeeObject = buildEmployeeObjectFromScope();
            employeeObject.ConfigId = UtilsService.guid();

            if ($scope.onEmployeeAdded != undefined) {
                $scope.onEmployeeAdded(employeeObject);
            }
            $scope.modalContext.closeModal();
        }

        function EditEmployee() {
            var employeeObject = buildEmployeeObjectFromScope();

            if ($scope.onEmployeeUpdated != undefined) {
                $scope.onEmployeeUpdated(employeeObject);
            }
            $scope.modalContext.closeModal();
        }

        function loadAllControls() {

            function setTitle() {

                if (isEditMode && departmentEntity != undefined) {
                    index = departmentEntity.Employees.findIndex(employee => employee.ConfigId === employeeId);
                    $scope.title = UtilsService.buildTitleForUpdateEditor(departmentEntity.Employees[index].Name, "Employee");
                }
                else {
                    $scope.title = UtilsService.buildTitleForAddEditor("Employee");
                }
            };

            function loadStaticData() {
                if (isEditMode && departmentEntity != undefined) {

                    $scope.scopeModel.name = departmentEntity.Employees[index].Name;
                    $scope.scopeModel.birthDate = departmentEntity.Employees[index].BirthDate;
                    $scope.scopeModel.hiringDate = departmentEntity.Employees[index].HiringDate;
                }
            };

            function loadEmployeeSettingsDirective() {
                var employeeSettingsDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

                employeeSettingsGridReadyDeferred.promise.then(function () {

                    var employeeSettingsPayload = {};

                    if (departmentEntity != undefined && index != -1)
                        employeeSettingsPayload.employeeSettingsEntity = (departmentEntity.Employees[index] != undefined) ? departmentEntity.Employees[index] : departmentEntity;

                    VRUIUtilsService.callDirectiveLoad(employeeSettingsAPI, employeeSettingsPayload, employeeSettingsDirectiveLoadDeferred);
                });

                return employeeSettingsDirectiveLoadDeferred.promise;
            };

            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadEmployeeSettingsDirective]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        };

        function buildEmployeeObjectFromScope() {
            var employeeSettings = employeeSettingsAPI.getData();
            var object = {
                $type: employeeSettings.$type,
                ConfigId: (departmentEntity != undefined) ? departmentEntity.Employees[index].employeeId : undefined,
                Name: $scope.scopeModel.name,
                BirthDate: $scope.scopeModel.birthDate,
                HiringDate: $scope.scopeModel.hiringDate,
            };

            if (employeeSettings.Salary != undefined) {
                object.Salary = employeeSettings.Salary;
            }
            else {
                if (employeeSettings.NumberOfHourPerMonth != undefined && employeeSettings.HourRate != undefined) {
                    object.NumberOfHourPerMonth = employeeSettings.NumberOfHourPerMonth;
                    object.HourRate = employeeSettings.HourRate;
                }
            }

            return object;
        };

    };

    appControllers.controller("Demo_Module_EmployeeEditorController", employeeEditorController);
})(appControllers);