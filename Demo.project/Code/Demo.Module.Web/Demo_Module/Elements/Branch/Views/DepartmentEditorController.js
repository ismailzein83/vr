(function (appControllers) {

    "use strict";

    departmentEditorController.$inject = ['$scope', 'Demo_Module_BranchAPIService', 'VRNavigationService', 'UtilsService', 'VRNotificationService', 'VRUIUtilsService'];

    function departmentEditorController($scope, Demo_Module_BranchAPIService, VRNavigationService, UtilsService, VRNotificationService, VRUIUtilsService) {

        var isEditMode;
        var departmentId;
        var branchEntity;
        var index;

        var departmentSettingsAPI;
        var departmentSettingsGridReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null) {
                branchEntity = parameters.branchItem;
                departmentId = parameters.departmentId;
            }

            isEditMode = (departmentId != undefined);
        };

        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.saveDepartment = function () {
                if (isEditMode)
                    EditDepartment();
                else
                    AddDepartment();
            };

            $scope.scopeModel.onDepartmentSettingsDirectiveReady = function (api) {
                departmentSettingsAPI = api;
                departmentSettingsGridReadyDeferred.resolve();
            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };
        };

        function load() {
            $scope.scopeModel.isLoading = true;

            loadAllControls();
        };

        function AddDepartment() {
            var departmentObject = buildDepartmentObjectFromScope();
            departmentObject.DepartmentId = UtilsService.guid();

            if ($scope.onDepartmentAdded != undefined) {
                $scope.onDepartmentAdded(departmentObject);
            }
            $scope.modalContext.closeModal();
        }

        function EditDepartment() {
            var departmentObject = buildDepartmentObjectFromScope();

            if ($scope.onDepartmentUpdated != undefined) {
                $scope.onDepartmentUpdated(departmentObject);
            }

            $scope.modalContext.closeModal();
        }

        function loadAllControls() {

            function setTitle() {

                if (isEditMode && branchEntity != undefined) {
                    index = branchEntity.Settings.Departments.findIndex(department => department.DepartmentId === departmentId);
                    $scope.title = UtilsService.buildTitleForUpdateEditor(branchEntity.Settings.Departments[index].Name, "Department");
                }
                else {
                    $scope.title = UtilsService.buildTitleForAddEditor("Department");
                }
            };

            function loadStaticData() {
                if (isEditMode && branchEntity != undefined) {

                    $scope.scopeModel.name = branchEntity.Settings.Departments[index].Name;
                    $scope.scopeModel.floorNumber = branchEntity.Settings.Departments[index].FloorNumber;
                }
            };

            function loadDepartmentSettingsDirective() {
                var departmentSettingsDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

                departmentSettingsGridReadyDeferred.promise.then(function () {
                    var departmentSettingsPayload = {};

                    if (branchEntity != undefined && index != -1) // -1: adding a new department
                        departmentSettingsPayload.branchDepartmentSettingsEntity = (branchEntity.Settings.Departments[index].Settings != undefined) ? branchEntity.Settings.Departments[index].Settings : branchEntity.Settings;

                    VRUIUtilsService.callDirectiveLoad(departmentSettingsAPI, departmentSettingsPayload, departmentSettingsDirectiveLoadDeferred);
                });

                return departmentSettingsDirectiveLoadDeferred.promise;
            };

            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadDepartmentSettingsDirective]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function buildDepartmentObjectFromScope() {
            var object = {
                $type: "Demo.Module.Entities.Department, Demo.Module.Entities",
                DepartmentId: (branchEntity != undefined) ? branchEntity.Settings.Departments[index].DepartmentId : undefined,
                Name: $scope.scopeModel.name,
                FloorNumber: $scope.scopeModel.floorNumber,
                Settings: departmentSettingsAPI.getData()
            };
            return object;
        }
    };

    appControllers.controller("Demo_Module_DepartmentEditorController", departmentEditorController);
})(appControllers);