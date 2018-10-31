(function (appControllers) {
    "use strict";
    pageDefinitionSubviewEditorController.$inject = ['$scope', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService', 'Demo_Module_EmployeeService'];

    function pageDefinitionSubviewEditorController($scope, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService, Demo_Module_EmployeeService) {

        var isEditMode;
        var pageDefinitionSubviewEntity = {};
        $scope.scopeModel = {};

        var subviewSettingsDirectiveApi;
        var subviewSettingsReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        var childFieldDirectiveApi;
        var childFieldReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                pageDefinitionSubviewEntity = parameters.pageDefinitionSubviewEntity;
            }
            isEditMode = (pageDefinitionSubviewEntity != undefined);
        }

        function defineScope() {

            $scope.scopeModel.savePageDefinitionSubview = function () {
                if (isEditMode)
                    return updatePageDefinitionSubview();
                else
                    return insertPageDefinitionSubview();
            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };

            $scope.scopeModel.onSubviewSettingsDirectiveReady = function (api) {

                subviewSettingsDirectiveApi = api;
                subviewSettingsReadyPromiseDeferred.resolve();
            };

            $scope.scopeModel.onSubviewSettingsDirectiveReady = function (api) {

                subviewSettingsDirectiveApi = api;
                subviewSettingsReadyPromiseDeferred.resolve();
            };
        }

        function load() {
            $scope.scopeModel.isLoading = true;

            if (isEditMode) {
                loadAllControls().finally(function () {
                    pageDefinitionSubviewEntity = undefined;
                });
            }
            else
                loadAllControls();
        }

        function loadAllControls()
        {
            function loadSubviewSettingsDirective() {
                var subviewSettingsLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                subviewSettingsReadyPromiseDeferred.promise.then(function () {
                    var subviewSettingsPayload = {};
                    if (pageDefinitionSubviewEntity != undefined && pageDefinitionSubviewEntity.PageDefinitionSubViewSettings != undefined) {
                        subviewSettingsPayload = {
                            subviewSettingsEntity: pageDefinitionSubviewEntity.PageDefinitionSubViewSettings
                        };
                    } 
                    VRUIUtilsService.callDirectiveLoad(subviewSettingsDirectiveApi, subviewSettingsPayload, subviewSettingsLoadPromiseDeferred);
                });
                return subviewSettingsLoadPromiseDeferred.promise;
            }

            function setTitle() {
                if (isEditMode && pageDefinitionSubviewEntity != undefined)
                    $scope.title = UtilsService.buildTitleForUpdateEditor(pageDefinitionSubviewEntity.Name, "Subview");
                else
                    $scope.title = UtilsService.buildTitleForAddEditor("Page Definition Subview");
            }

            function loadStaticData() {

                if (pageDefinitionSubviewEntity != undefined) {
                    $scope.scopeModel.name = pageDefinitionSubviewEntity.Name;
                    $scope.scopeModel.title = pageDefinitionSubviewEntity.Title;
                    
                }
            }

            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadSubviewSettingsDirective]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            })
           .finally(function () {
               $scope.scopeModel.isLoading = false;
           });
        }

        function buildPageDefinitionSubviewObjectFromScope() {
            return {
                Name: $scope.scopeModel.name,
                Title: $scope.scopeModel.title,
                PageDefinitionSubViewSettings: subviewSettingsDirectiveApi.getData()
            };
        }

        function insertPageDefinitionSubview() {

            if ($scope.onPageDefinitionSubviewAdded != undefined) {
                $scope.onPageDefinitionSubviewAdded(buildPageDefinitionSubviewObjectFromScope());
            }
            $scope.modalContext.closeModal();
        }
 
        function updatePageDefinitionSubview() {
            if ($scope.onPageDefinitionSubviewUpdated != undefined) {
                $scope.onPageDefinitionSubviewUpdated(buildPageDefinitionSubviewObjectFromScope());
            }
            $scope.modalContext.closeModal();
        }

     };
    appControllers.controller('Demo_Module_PageDefinitionSubviewEditorController', pageDefinitionSubviewEditorController);
})(appControllers);