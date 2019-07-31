(function (appControllers) {

    "use strict";

    departmentEditorController.$inject = ['$scope', 'Demo_Module_BranchAPIService', 'VRNavigationService', 'UtilsService', 'VRNotificationService', 'VRUIUtilsService'];

    function departmentEditorController($scope, Demo_Module_BranchAPIService, VRNavigationService, UtilsService, VRNotificationService, VRUIUtilsService) {

        var isEditMode;
        var departmentId;
        var branchEntity;
        var departmentEntity;

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
                    editDepartment();
                else
                    addDepartment();
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

        function addDepartment() {
            var departmentObject = buildDepartmentObjectFromScope();
            departmentObject.DepartmentId = UtilsService.guid();

            if ($scope.onDepartmentAdded != undefined) {
                $scope.onDepartmentAdded(departmentObject);
            }
            $scope.modalContext.closeModal();
        }

        function editDepartment() {
            var departmentObject = buildDepartmentObjectFromScope();

            if ($scope.onDepartmentUpdated != undefined) {
                $scope.onDepartmentUpdated(departmentObject);
            }

            $scope.modalContext.closeModal();
        }

        function loadAllControls() {
            if (isEditMode && branchEntity != undefined && branchEntity.Settings != undefined && branchEntity.Settings.Departments != undefined) {
                var index = branchEntity.Settings.Departments.findIndex(department => department.DepartmentId === departmentId);
                if (index != -1) {
                    departmentEntity = branchEntity.Settings.Departments[index];
                }
            }

            function setTitle() {

                if (isEditMode && departmentEntity != undefined) {
                    $scope.title = UtilsService.buildTitleForUpdateEditor(departmentEntity.Name, "Department");
                }
                else {
                    $scope.title = UtilsService.buildTitleForAddEditor("Department");
                }
            };

            function loadStaticData() {
                if (isEditMode && departmentEntity != undefined) {

                    $scope.scopeModel.name = departmentEntity.Name;
                    $scope.scopeModel.floorNumber = departmentEntity.FloorNumber;
                }
            };

            function loadDepartmentSettingsDirective() {
                var departmentSettingsDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

                departmentSettingsGridReadyDeferred.promise.then(function () {
                    var departmentSettingsPayload;

                    if (departmentEntity != undefined && departmentEntity.Settings != undefined) {
                        departmentSettingsPayload = { branchDepartmentSettingsEntity: departmentEntity.Settings };
                    }

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
                DepartmentId: (departmentEntity != undefined) ? departmentEntity.DepartmentId : undefined,
                Name: $scope.scopeModel.name,
                FloorNumber: $scope.scopeModel.floorNumber,
                Settings: departmentSettingsAPI.getData()
            };
            return object;
        }
    };

    appControllers.controller("Demo_Module_DepartmentEditorController", departmentEditorController);
})(appControllers);