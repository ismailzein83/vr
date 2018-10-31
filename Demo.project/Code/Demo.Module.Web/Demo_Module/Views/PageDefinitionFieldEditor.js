(function (appControllers) {
    "use strict";
    pageDefinitionFieldEditorController.$inject = ['$scope', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService', 'Demo_Module_EmployeeService'];

    function pageDefinitionFieldEditorController($scope, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService, Demo_Module_EmployeeService) {

        var isEditMode;
        var pageDefinitionFieldEntity = {};
        $scope.scopeModel = {};

        var fieldTypeDirectiveApi;
        var fieldTypeReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        var index;
        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                pageDefinitionFieldEntity = parameters.pageDefinitionFieldEntity;
                index= parameters.index;
            }
            isEditMode = (pageDefinitionFieldEntity != undefined);
        }

        function defineScope() {

            $scope.scopeModel.savePageDefinitionField = function () {
                if (isEditMode)
                    return updatePageDefinitionField();
                else
                    return insertPageDefinitionField();
            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };

            $scope.scopeModel.onFieldTypeDirectiveReady = function (api) {
                fieldTypeDirectiveApi = api;
                fieldTypeReadyPromiseDeferred.resolve();
            };
        }

        function load() {
            $scope.scopeModel.isLoading = true;

            if (isEditMode) {
                loadAllControls().finally(function () {
                    pageDefinitionFieldEntity = undefined;
                });
            }
            else
                loadAllControls();
        }

        function loadAllControls()
        {
            function loadFieldTypeDirective() {
                var fieldTypeLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                fieldTypeReadyPromiseDeferred.promise.then(function () {
                    var fieldTypePayload = {}; 
                    if (pageDefinitionFieldEntity != undefined && pageDefinitionFieldEntity.FieldType != undefined) {
                        fieldTypePayload = {
                            fieldTypeEntity: pageDefinitionFieldEntity.FieldType
                        };
                    } 
                    VRUIUtilsService.callDirectiveLoad(fieldTypeDirectiveApi, fieldTypePayload, fieldTypeLoadPromiseDeferred);
                });
                return fieldTypeLoadPromiseDeferred.promise;
            }

            function setTitle() {
                if (isEditMode && pageDefinitionFieldEntity != undefined)
                    $scope.title = UtilsService.buildTitleForUpdateEditor(pageDefinitionFieldEntity.Name, "Field");
                else
                    $scope.title = UtilsService.buildTitleForAddEditor("Page Definition Field");
            }

            function loadStaticData() {

                if (pageDefinitionFieldEntity != undefined) {
                    $scope.scopeModel.name = pageDefinitionFieldEntity.Name;
                    $scope.scopeModel.title = pageDefinitionFieldEntity.Title;
                    $scope.scopeModel.IsRequired = pageDefinitionFieldEntity.IsRequired;
                    
                }
            }

            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadFieldTypeDirective]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            })
           .finally(function () {
               $scope.scopeModel.isLoading = false;
           });
        }

        function buildPageDefinitionFieldObjectFromScope() {
            return {
                Name: $scope.scopeModel.name,
                Title: $scope.scopeModel.title,
                FieldType: fieldTypeDirectiveApi.getData(),
                IsRequired: $scope.scopeModel.IsRequired
            };
        }

        function insertPageDefinitionField() {
            if ($scope.onPageDefinitionFieldAdded != undefined) {
                $scope.onPageDefinitionFieldAdded(buildPageDefinitionFieldObjectFromScope());
            }
            if ($scope.fieldAdder!=undefined) {
                $scope.fieldAdder(buildPageDefinitionFieldObjectFromScope());
        }
            $scope.modalContext.closeModal();
        }
 
        function updatePageDefinitionField() {

            if ($scope.onPageDefinitionFieldUpdated != undefined) {
                $scope.onPageDefinitionFieldUpdated(buildPageDefinitionFieldObjectFromScope());
            }
            if ($scope.fieldUpdater != undefined) {
                $scope.fieldUpdater(buildPageDefinitionFieldObjectFromScope(),index);
            }
            $scope.modalContext.closeModal();
        }

     };
    appControllers.controller('Demo_Module_PageDefinitionFieldEditorController', pageDefinitionFieldEditorController);
})(appControllers);