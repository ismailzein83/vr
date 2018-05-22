(function (appControllers) {
    "use strict";
    advancedExcelFileGeneratorEditorController.$inject = ['$scope', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService' ];
    function advancedExcelFileGeneratorEditorController($scope, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService ) {


        var isEditMode;
        var tableDefinitionEntity;


        loadParameters();
        defineScope();
        load();

        function loadParameters() {

            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                tableDefinitionEntity = parameters.Entity;
            }
            isEditMode = (tableDefinitionEntity != undefined);
        }

        function defineScope() {
            $scope.scopeModel = {};

            $scope.saveTableDefinition = function () {
                if (isEditMode)
                    return updateTableDefinition();
                else
                    return insertTableDefinition();
            };


            $scope.close = function () {
                $scope.modalContext.closeModal();
            };
        }
        function load() {
            $scope.isLoading = true;
            loadAllControls();
        }

        function loadAllControls() {

            function setTitle() {
                if (isEditMode && tableDefinitionEntity != undefined)
                    $scope.title = UtilsService.buildTitleForUpdateEditor(tableDefinitionEntity.Name, "Table Definition");
                else
                    $scope.title = UtilsService.buildTitleForAddEditor("Table Definition");
            }

            function loadStaticData() {

                if (tableDefinitionEntity == undefined)
                    return;
                $scope.scopeModel.QueryName = tableDefinitionEntity.QueryName;
                $scope.scopeModel.ListName = tableDefinitionEntity.ListName;
                $scope.scopeModel.SheetName = tableDefinitionEntity.SheetName;
                $scope.scopeModel.RowIndex = tableDefinitionEntity.RowIndex;
            }

                return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData]) 
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.isLoading = false;
              });
        }

        function buildObjFromScope() {
            var obj = {
                QueryName: $scope.scopeModel.QueryName,
                ListName: $scope.scopeModel.ListName,
                SheetName: $scope.scopeModel.SheetName,
                RowIndex: $scope.scopeModel.RowIndex,
            };
            return obj;
        }

        function insertTableDefinition() {
            var Object = buildObjFromScope();
            if ($scope.onTableDefinitionAdded != undefined && typeof ($scope.onTableDefinitionAdded) == 'function')
                $scope.onTableDefinitionAdded(Object);
            $scope.modalContext.closeModal();
        }

        function updateTableDefinition() {
            var Object = buildObjFromScope();
            if ($scope.onTableDefinitionUpdated != undefined && typeof ($scope.onTableDefinitionUpdated) == 'function')
                $scope.onTableDefinitionUpdated(Object);
            $scope.modalContext.closeModal();
        }
    }
    appControllers.controller('VRAnalytic_AdvancedExcelFileGeneratorEditorController', advancedExcelFileGeneratorEditorController);
})(appControllers);