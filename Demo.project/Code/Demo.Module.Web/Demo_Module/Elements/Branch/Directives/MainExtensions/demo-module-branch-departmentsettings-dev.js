"use strict"

app.directive("demoModuleBranchDepartmentsettingsDev", ["UtilsService", "VRNotificationService", "VRUIUtilsService","Demo_Module_BranchService",
    function (UtilsService, VRNotificationService, VRUIUtilsService, Demo_Module_BranchService) {

        var directiveDefinitionObject = {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new DevDepartment($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Demo_Module/Elements/Branch/Directives/MainExtensions/Templates/DevloppersDepartmentTemplate.html"
        }

        function DevDepartment($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var gripdAPI;
            var departmentEntity;

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.employees = [];

                $scope.scopeModel.onGridReady = function (api) {
                    gripdAPI = api;
                    defineAPI();
                };

                $scope.scopeModel.addEmployee = function () {
                    var onEmployeeAdded = function (employee) {
                        $scope.scopeModel.employees.push(employee);

                        if (departmentEntity == undefined)
                            departmentEntity = {};

                        if (departmentEntity.Employees == undefined)
                            departmentEntity.Employees = [];

                        departmentEntity.Employees.push(employee);
                    };

                    var departmentItem = (departmentEntity != undefined) ? departmentEntity : undefined
                    Demo_Module_BranchService.addEmployee(onEmployeeAdded, departmentItem);
                };

                $scope.scopeModel.onDeleteRow = function (deletedItem) {
                    var index = UtilsService.getItemIndexByVal($scope.scopeModel.employees, deletedItem.ConfigId, 'ConfigId');
                    if (index != -1)
                        $scope.scopeModel.employees.splice(index, 1);
                };
                defineMenuActions();
            };

            function defineMenuActions() {
                $scope.scopeModel.gridMenuActions = [{
                    name: "Edit",
                    clicked: editEmployee
                }];
            };

            function editEmployee(employee) {
                var onEmployeeUpdated = function (employeeObject) {
                    var index = $scope.scopeModel.employees.indexOf(employee);
                    $scope.scopeModel.employees[index] = employeeObject;
                    departmentEntity.Employees[index] = employeeObject;
                };
                
                Demo_Module_BranchService.editEmployee(onEmployeeUpdated, employee.ConfigId, departmentEntity);

            };

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var loadEmployeeDeferred = UtilsService.createPromiseDeferred();
                    loadEmployeeDeferred.resolve();

                    if (payload != undefined) {
                        departmentEntity = payload.branchDepartmentSettingsEntity;
                        if (departmentEntity.Employees != undefined) {
                            for (var i = 0; i < departmentEntity.Employees.length; i++) {
                                $scope.scopeModel.employees.push(departmentEntity.Employees[i]);
                            }
                        }
                    }
                    return loadEmployeeDeferred.promise;
                };

                api.getData = function () {
                    var employees = [];
                    for (var i = 0; i < $scope.scopeModel.employees.length; i++) {
                        employees.push($scope.scopeModel.employees[i]);
                    }
                    return {
                        $type: "Demo.Module.MainExtension.Department.DevDepartment , Demo.Module.MainExtension",
                        Employees: employees
                    }
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(api);
            }
        }
        return directiveDefinitionObject;
    }
])