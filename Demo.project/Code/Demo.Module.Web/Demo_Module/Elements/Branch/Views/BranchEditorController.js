(function (appControllers) {

    "use strict";

    branchEditorController.$inject = ['$scope', 'Demo_Module_BranchAPIService', 'VRNavigationService', 'UtilsService', 'VRNotificationService', 'VRUIUtilsService'];

    function branchEditorController($scope, Demo_Module_BranchAPIService, VRNavigationService, UtilsService, VRNotificationService, VRUIUtilsService) {

        var isEditMode;
        var branchId;
        var branchEntity;
        var companyIdItem;

        var companySelectorApi;
        var companySelectorReadyDeferred = UtilsService.createPromiseDeferred(); // $q.defer();

        var departmentsAPI;
        var departmentsGridReadyDeferred = UtilsService.createPromiseDeferred();

        var branchTypeDirectiveAPI;
        var branchTypeDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null) {
                branchId = parameters.branchId;
                companyIdItem = parameters.companyIdItem;
            }

            isEditMode = (branchId != undefined);
        };

        function defineScope() {

            $scope.scopeModel = {};
            $scope.scopeModel.departments = [];
            $scope.scopeModel.disableCompany = (companyIdItem != undefined); // in case of drill down

            $scope.scopeModel.saveBranch = function () {
                if (isEditMode)
                    return updateBranch();
                else
                    return insertBranch();
            };

            $scope.scopeModel.onCompanySelectorReady = function (api) {
                companySelectorApi = api;
                companySelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.onBranchTypeDirectiveReady = function (api) {
                branchTypeDirectiveAPI = api;
                branchTypeDirectiveReadyDeferred.resolve();
            };

            $scope.scopeModel.onBranchDepartmentsdirectiveReady = function (api) {
                departmentsAPI = api;
                departmentsGridReadyDeferred.resolve();
            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };
        };

        function load() {
            $scope.scopeModel.isLoading = true;

            if (isEditMode) {
                getBranch().then(function () {
                    loadAllControls();
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.scopeModel.isLoading = false;
                });
            } else {
                loadAllControls();
            }
        };

        function getBranch() {
            return Demo_Module_BranchAPIService.GetBranchById(branchId).then(function (response) {
                branchEntity = response;
            });
        };

        function loadAllControls() {

            function setTitle() {
                if (isEditMode && branchEntity != undefined) {
                    $scope.title = UtilsService.buildTitleForUpdateEditor(branchEntity.Name, "Branch");
                }
                else {
                    $scope.title = UtilsService.buildTitleForAddEditor("Branch");
                }
            };

            function loadStaticData() {
                if (branchEntity != undefined) {
                    $scope.scopeModel.name = branchEntity.Name;
                    $scope.scopeModel.address = branchEntity.Settings.Address;
                }
            };

            function loadCompanySelector() {
                var companySelectorLoadDeferred = UtilsService.createPromiseDeferred();

                companySelectorReadyDeferred.promise.then(function () {
                    var companyPayload = {};

                    if (companyIdItem != undefined)
                        companyPayload.selectedIds = companyIdItem.CompanyId;

                    if (branchEntity != undefined)
                        companyPayload.selectedIds = branchEntity.CompanyId;

                    VRUIUtilsService.callDirectiveLoad(companySelectorApi, companyPayload, companySelectorLoadDeferred);
                });
                return companySelectorLoadDeferred.promise;
            };

            function loadBranchTypeDirective() {
                var branchTypeDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

                branchTypeDirectiveReadyDeferred.promise.then(function () {
                    var branchTypePayload = {};

                    if (branchEntity != undefined)
                        branchTypePayload.branchTypeEntity = branchEntity.Settings.BranchType;

                    VRUIUtilsService.callDirectiveLoad(branchTypeDirectiveAPI, branchTypePayload, branchTypeDirectiveLoadDeferred);
                });

                return branchTypeDirectiveLoadDeferred.promise;
            };

            function loadDepartmentsList() {
                var deparmtentsDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

                departmentsGridReadyDeferred.promise.then(function () {
                    var departmentsPayload;

                    if (branchEntity != undefined) {
                        departmentsPayload = {};
                        departmentsPayload.departments = branchEntity.Settings.Departments;
                        departmentsPayload.branchEntity = branchEntity;
                    }
                    VRUIUtilsService.callDirectiveLoad(departmentsAPI, departmentsPayload, deparmtentsDirectiveLoadDeferred);

                });
                return deparmtentsDirectiveLoadDeferred.promise;
            }

            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadCompanySelector, loadBranchTypeDirective, loadDepartmentsList]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        };

        function insertBranch() {
            $scope.scopeModel.isLoading = true;

            var branchObject = buildBranchObjectFromScope();
            return Demo_Module_BranchAPIService.AddBranch(branchObject).then(function (response) {
                if (VRNotificationService.notifyOnItemAdded("Branch", response, "Name")) {
                    if ($scope.onBranchAdded != undefined) {
                        $scope.onBranchAdded(response.InsertedObject);
                    }
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        };

        function updateBranch() {
            $scope.scopeModel.isLoading = true;

            var branchObject = buildBranchObjectFromScope();
            return Demo_Module_BranchAPIService.UpdateBranch(branchObject).then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated("Branch", response, "Name")) {
                    if ($scope.onBranchUpdated != undefined) {
                        $scope.onBranchUpdated(response.UpdatedObject);
                    }
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        };

        function buildBranchObjectFromScope() {
            var object = {
                BranchId: (branchId != undefined) ? branchId : undefined,
                Name: $scope.scopeModel.name,
                CompanyId: companySelectorApi.getSelectedIds(),
                Settings: {
                    Address: $scope.scopeModel.address,
                    BranchType: branchTypeDirectiveAPI.getData(),
                    Departments: departmentsAPI.getData()
                }
            };
            return object;
        };

    };

    appControllers.controller("Demo_Module_BranchEditorController", branchEditorController);
})(appControllers);