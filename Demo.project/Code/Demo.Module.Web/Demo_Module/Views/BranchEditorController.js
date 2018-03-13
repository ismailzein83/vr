(function (appControllers) {

    "use strict";
    branchEditorController.$inject = ['$scope', 'Demo_Module_BranchAPIService', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService'];

    function branchEditorController($scope, Demo_Module_BranchAPIService, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService) {

        var isEditMode;
        var branchId;
        var branchEntity;

        var companyId;
        var companyDirectiveApi;
        var companyReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var settingDirectiveApi;
        var settingReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                branchId = parameters.branchId;
            }
            isEditMode = (branchId != undefined);

        };

        function defineScope() {


            $scope.scopeModel = {};

            $scope.scopeModel.saveBranch = function () {
                if (isEditMode)
                    return updateBranch();
                else
                    return insertBranch();

            };

            $scope.scopeModel.close = function () {

                $scope.modalContext.closeModal();
            };

            $scope.scopeModel.onCompanyDirectiveReady = function (api) {

                companyDirectiveApi = api;
                companyReadyPromiseDeferred.resolve();
            };

            $scope.scopeModel.onSettingDirectiveReady = function (api) {
                settingDirectiveApi = api;
                settingReadyPromiseDeferred.resolve();
            };

        };

            function buildBranchObjectFromScope() {
                
                var object = {
                    BranchId: (branchId != null) ? branchId : 0,
                    Name: $scope.scopeModel.name,
                    CompanyId: companyDirectiveApi.getSelectedIds(),
                    Setting: settingDirectiveApi.getData()
                 
                };
                
                return object;
            };

            function load() {
                $scope.scopeModel.isLoading = true;
                if (isEditMode) {
                    getBranch().then(function () {
                        loadAllControls().finally(function () {
                            branchEntity = undefined;
                        });
                    }).catch(function () {
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                        $scope.scopeModel.isLoading = false;
                    });
                }
                else
                    loadAllControls();
            };
            function getBranch() {
                return Demo_Module_BranchAPIService.GetBranchById(branchId).then(function (branchObject) {
                    branchEntity = branchObject;

                });
            };

            function loadAllControls() {
                function setTitle() {
                    if (isEditMode && branchEntity != undefined)
                        $scope.title = UtilsService.buildTitleForUpdateEditor(branchEntity.Name, "Branch");
                    else
                        $scope.title = UtilsService.buildTitleForAddEditor("Branch");
                };
                function loadStaticData() {
                    if (branchEntity != undefined)
                        $scope.scopeModel.name = branchEntity.Name;
                };
                function loadCompanySelector() {
                    var companyLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                    companyReadyPromiseDeferred.promise.then(function () {
                        var directivePayload = {
                            selectedIds: branchEntity != undefined ? branchEntity.CompanyId : undefined,
                        };

                        VRUIUtilsService.callDirectiveLoad(companyDirectiveApi, directivePayload, companyLoadPromiseDeferred);
                    });
                    return companyLoadPromiseDeferred.promise;
                }
                function loadSettingSelector() {
                    var settingLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                    settingReadyPromiseDeferred.promise.then(function () {
                        
                        var directivePayload = branchEntity != undefined ? branchEntity.Setting : undefined;
                      
                        VRUIUtilsService.callDirectiveLoad(settingDirectiveApi, directivePayload, settingLoadPromiseDeferred);

                    });
                    return settingLoadPromiseDeferred.promise;
                }
                return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadCompanySelector, loadSettingSelector])
                 .catch(function (error) {
                     VRNotificationService.notifyExceptionWithClose(error, $scope);
                 })
                   .finally(function () {
                       $scope.scopeModel.isLoading = false;
                   });
            };

            function insertBranch() {

                $scope.scopeModel.isLoading = true;
                var branchObject = buildBranchObjectFromScope();

                return Demo_Module_BranchAPIService.AddBranch(branchObject)
                .then(function (response) {
                    
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
                Demo_Module_BranchAPIService.UpdateBranch(branchObject).then(function (response) {
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
        };
  
    appControllers.controller('Demo_Module_BranchEditorController', branchEditorController);
})(appControllers);