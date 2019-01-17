"use strict";

app.directive("demoModuleBranchDepartmentsGrid", ["VRNotificationService", "Demo_Module_BranchService", "Demo_Module_BranchAPIService", "VRUIUtilsService", "VRModalService", "UtilsService",
    function (VRNotificationService, Demo_Module_BranchService, Demo_Module_BranchAPIService, VRUIUtilsService, VRModalService, UtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var branchDepartmentsGrid = new BranchDepartmentsGrid($scope, ctrl, $attrs);
                branchDepartmentsGrid.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: "/Client/Modules/Demo_Module/Elements/Branch/Directives/Templates/BranchDepartmentsGridTemplate.html"
        };

        function BranchDepartmentsGrid($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var branchEntity;

            var gripdAPI;

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.departments = [];

                $scope.scopeModel.onGridReady = function (api) {
                    gripdAPI = api;
                    defineAPI();
                };

                $scope.scopeModel.onDepartmentSettingsDirectiveReady = function (api) {
                    departmentSettingsAPI = api;
                    departmentSettingsGridReadyDeferred.resolve();
                };

                $scope.scopeModel.addDepartment = function () {

                    var onDepartmentAdded = function (department) {
                        $scope.scopeModel.departments.push(department);

                        if (branchEntity == undefined) 
                            branchEntity = {};
                        
                        if (branchEntity.Settings == undefined)
                            branchEntity.Settings = {};

                        if (branchEntity.Settings.Departments == undefined)
                            branchEntity.Settings.Departments = [];

                        branchEntity.Settings.Departments.push(department);
                    };

                    var branchItem = (branchEntity != undefined) ? branchEntity : undefined
                    Demo_Module_BranchService.addDepartment(onDepartmentAdded, branchItem);
                };

                $scope.scopeModel.onDeleteRow = function (deletedItem) {
                    var index = UtilsService.getItemIndexByVal($scope.scopeModel.departments, deletedItem.DepartmentId, 'DepartmentId');
                    if (index != -1)
                        $scope.scopeModel.departments.splice(index, 1);
                };
                defineMenuActions();
            };

            function defineMenuActions() {
                $scope.scopeModel.gridMenuActions = [{
                    name: "Edit",
                    clicked: editDepartment
                }];
            };
            function editDepartment(department) {

                var onDepartmentUpdated = function (departmentObject) {
                    var index = $scope.scopeModel.departments.indexOf(department);
                    $scope.scopeModel.departments[index] = departmentObject;
                    branchEntity.Settings.Departments[index] = departmentObject;
                };

                Demo_Module_BranchService.editDepartment(onDepartmentUpdated, department.DepartmentId, branchEntity);

            };


            function loadStaticData() {
                var loadDepartmentDeferred = UtilsService.createPromiseDeferred();
                loadDepartmentDeferred.resolve();
                if (branchEntity != undefined) {
                    if (branchEntity.Settings.Departments != undefined) {
                        for (var i = 0; i < branchEntity.Settings.Departments.length; i++) {
                            $scope.scopeModel.departments.push(branchEntity.Settings.Departments[i]);
                        }
                    }
                }
                return loadDepartmentDeferred.promise;
            }

            function defineAPI() {

                var api = {};
                api.load = function (payload) {
                    $scope.scopeModel.isLoading = true;

                    if (payload != undefined) {
                        branchEntity = payload.branchEntity;
                    }

                    return UtilsService.waitMultipleAsyncOperations([loadStaticData]).catch(function (error) {
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                    }).finally(function () {
                        $scope.scopeModel.isLoading = false;
                    });

                };

                api.getData = function () {
                    var departments = [];
                    for (var i = 0; i < $scope.scopeModel.departments.length; i++) {
                        departments.push($scope.scopeModel.departments[i]);
                    }
                    
                    return departments;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(api);
            };
        };

        return directiveDefinitionObject;
    }]);